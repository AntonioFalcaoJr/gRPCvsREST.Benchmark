using System.Collections.Concurrent;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace BFF.WebAPI;

public class GrpcChannelMultiplexer : IDisposable
{
    private readonly ConcurrentDictionary<Type, GrpcChannel> _concurrentDictionary;

    public GrpcChannelMultiplexer()
    {
        _concurrentDictionary = new ConcurrentDictionary<Type, GrpcChannel>();
    }

    public void Configure<T>(string address)
        where T : ClientBase =>
        Configure<T>(address, new GrpcChannelOptions());

    public void Configure<T>(string address, GrpcChannelOptions options)
        where T : ClientBase
    {
        options ??= new GrpcChannelOptions();
        options.HttpHandler = new SocketsHttpHandler
        {
            EnableMultipleHttp2Connections = true,
            PooledConnectionLifetime = Timeout.InfiniteTimeSpan,
            PooledConnectionIdleTimeout = Timeout.InfiniteTimeSpan
        };

        var channel = GrpcChannel.ForAddress(address, options);
        _concurrentDictionary.TryAdd(typeof(T), channel);
    }

    public GrpcChannel Get<T>()
    {
        if (!_concurrentDictionary.TryGetValue(typeof(T), out var value))
            throw new Exception();

        return value;
    }

    public void Dispose()
    {
        var list = _concurrentDictionary.ToList();
        _concurrentDictionary.Clear();
        
        foreach (var (type, channel) in list) 
            channel.Dispose();
    }
}

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGrpcClientMultiplexed<T>(this IServiceCollection services)
        where T : ClientBase<T>
    {
        services
            .AddTransient<T>(p =>
            {
                var multiplexer = p.GetRequiredService<GrpcChannelMultiplexer>();
                var channel = multiplexer.Get<T>();

                var client = Activator.CreateInstance(typeof(T), new object[] { channel });
                return (T)client;
            })
            .TryAddSingleton<GrpcChannelMultiplexer>();
        return services;
    }
}

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseGrpcClientMultiplexed<T>(this IApplicationBuilder app, string address)
        where T : ClientBase<T>
    {
        var multiplexer = app.ApplicationServices.GetRequiredService<GrpcChannelMultiplexer>();
        multiplexer.Configure<T>(address);
        return app;
    }
}