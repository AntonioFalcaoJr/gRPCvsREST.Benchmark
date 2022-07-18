using BFF.WebAPI;
using BFF.WebAPI.HttpClients;
using Grpc.Net.Client;
using GRPCvsREST.Benchmark;
using Microsoft.AspNetCore.HttpLogging;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.ConfigureLogging((context, loggingBuilder) =>
{
    Log.Logger = new LoggerConfiguration().ReadFrom
        .Configuration(context.Configuration)
        .CreateLogger();

    loggingBuilder.ClearProviders();
    loggingBuilder.AddSerilog();
    builder.Host.UseSerilog();
});

builder.Services.AddHttpLogging(options
    => options.LoggingFields = HttpLoggingFields.All);

builder.Services
    .AddEndpointsApiExplorer()
    .AddSwaggerGen();

// SETUP - 273k request/min (1BFF = 1Server)
// builder.Services.AddSingleton(GrpcChannel.ForAddress(builder.Configuration["Benchmark:GrpcClient"]!,
//     new GrpcChannelOptions {HttpHandler = new SocketsHttpHandler {EnableMultipleHttp2Connections = true}}));
// builder.Services.AddTransient(p => new BenchmarkService.BenchmarkServiceClient(p.GetRequiredService<GrpcChannel>()));

// 200k
// builder.Services
//     .AddGrpcClient<BenchmarkService.BenchmarkServiceClient>(c => c.Address = new Uri(builder.Configuration["Benchmark:GrpcClient"]!))
//     .ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler { EnableMultipleHttp2Connections = true });

// SETUP - 310k request/min (1BFF = 1Server)
builder.Services.AddSingleton(GrpcChannel.ForAddress(builder.Configuration["Benchmark:GrpcClient"]!,
    new GrpcChannelOptions
    {
        HttpHandler = new SocketsHttpHandler
        {
            EnableMultipleHttp2Connections = true,
            PooledConnectionLifetime = Timeout.InfiniteTimeSpan,
            PooledConnectionIdleTimeout = Timeout.InfiniteTimeSpan,
            
            KeepAlivePingDelay = TimeSpan.FromSeconds(30),
            KeepAlivePingTimeout = TimeSpan.FromSeconds(10)
        }
    }));

builder.Services.AddScoped(p => new BenchmarkService.BenchmarkServiceClient(p.GetRequiredService<GrpcChannel>()));

// 298
// builder.Services.AddGrpcClientMultiplexed<BenchmarkService.BenchmarkServiceClient>();

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

app.MapGet("/grpc", ([AsParameters] Requests.GrpcRetrieveRequest request)
    => request.Client.RetrieveAsync(new()).ResponseAsync);

app.MapPost("/grpc", ([AsParameters] Requests.GrpcSubmitRequest request)
    => request.Client.SubmitAsync(new()).ResponseAsync);

app.MapGet("/rest", ([AsParameters] Requests.RestRetrieveRequest request)
    => request.Client.RetrieveAsync());

app.MapPost("/rest", ([AsParameters] Requests.RestSubmitRequest request)
    => request.Client.SubmitAsync());

try
{
    //app.UseGrpcClientMultiplexed<BenchmarkService.BenchmarkServiceClient>(builder.Configuration["Benchmark:GrpcClient"]!);
    
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