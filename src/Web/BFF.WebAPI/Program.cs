using BFF.WebAPI;
using BFF.WebAPI.HttpClients;
using Grpc.Core;
using Grpc.Net.Client.Configuration;
using GRPCvsREST.Benchmark;
using Microsoft.AspNetCore.HttpLogging;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration().ReadFrom
    .Configuration(builder.Configuration)
    .CreateLogger();

builder.Logging
    .ClearProviders()
    .AddSerilog();

builder.Host.UseSerilog();

builder.Services.AddHttpLogging(options
    => options.LoggingFields = HttpLoggingFields.All);

builder.Services
    .AddEndpointsApiExplorer()
    .AddSwaggerGen(options => options.CustomSchemaIds(type => type.FullName));

builder.Services.AddGrpcClient<BenchmarkService.BenchmarkServiceClient>(options
        => options.Address = new(builder.Configuration["Benchmark:GrpcClient"]!))
    .ConfigureChannel((provider, options) =>
        {
            options.Credentials = ChannelCredentials.Insecure;
            options.ServiceConfig = new() { LoadBalancingConfigs = { new RoundRobinConfig() } };
            options.ServiceProvider = provider;
        }
    ).ConfigurePrimaryHttpMessageHandler(() =>
        new SocketsHttpHandler
        {
            EnableMultipleHttp2Connections = true,
            PooledConnectionLifetime = Timeout.InfiniteTimeSpan,
            PooledConnectionIdleTimeout = Timeout.InfiniteTimeSpan,
            KeepAlivePingDelay = TimeSpan.FromSeconds(60),
            KeepAlivePingTimeout = TimeSpan.FromSeconds(30)
        }).EnableCallContextPropagation(options
        => options.SuppressContextNotFoundErrors = true);

builder.Services.AddHttpClient<IRestHttpClient, RestHttpClient>(client =>
        client.BaseAddress = new(builder.Configuration["Benchmark:RestClient"]!))
    .SetHandlerLifetime(Timeout.InfiniteTimeSpan);

var app = builder.Build();

if (builder.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();

if (builder.Environment.IsDevelopment() || builder.Environment.IsStaging())
{
    app.UseSwagger();
    app.UseSwaggerUI(options => options.EnableTryItOutByDefault());
}

app.MapGet("/grpc/health", ([AsParameters] Requests.GrpcHealthRequest request)
    => request.Client.HealthAsync(new()).ResponseAsync);

app.MapGet("/grpc", ([AsParameters] Requests.GrpcRetrieveRequest request)
    => request.Client.RetrieveAsync(new() { Amount = request.Amount }).ResponseAsync);

app.MapPost("/grpc", ([AsParameters] Requests.GrpcSubmitRequest request)
    => request.Client.SubmitAsync(new()).ResponseAsync);

app.MapGet("/rest/health", ([AsParameters] Requests.RestHealthRequest request)
    => request.Client.HealthAsync());

app.MapGet("/rest", ([AsParameters] Requests.RestRetrieveRequest request)
    => request.Client.RetrieveAsync(request.Amount));

app.MapPost("/rest", ([AsParameters] Requests.RestSubmitRequest request)
    => request.Client.SubmitAsync());

try
{
    await app.RunAsync();
    Log.Information("Stopped cleanly");
}
catch (Exception ex)
{
    Log.Fatal(ex, "An unhandled exception occured during bootstrapping");
    await app.StopAsync();
}
finally
{
    Log.CloseAndFlush();
    await app.DisposeAsync();
}