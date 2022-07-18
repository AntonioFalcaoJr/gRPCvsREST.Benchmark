using BFF.WebAPI;
using GRPCvsREST.Benchmark;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.Extensions.DependencyInjection.Extensions;
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


// SETUP - 273k request/minuto (1BFF = 1Server)
// builder.Services.AddSingleton(GrpcChannel.ForAddress(builder.Configuration["Benchmark:GrpcClient"]!,
//     new GrpcChannelOptions { HttpHandler = new SocketsHttpHandler { EnableMultipleHttp2Connections = true } }));
// builder.Services.AddTransient(p => new BenchmarkService.BenchmarkServiceClient(p.GetRequiredService<GrpcChannel>()));

// SETUP - 200k request/minuto (1BFF = 1Server)
// builder.Services
//     .AddGrpcClient<BenchmarkService.BenchmarkServiceClient>(c => c.Address = new Uri(builder.Configuration["Benchmark:GrpcClient"]!))
//     .ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler { EnableMultipleHttp2Connections = true });

builder.Services.AddGrpcClientMultiplexed<BenchmarkService.BenchmarkServiceClient>();

builder.Services.AddHttpClient("rest", client
    => client.BaseAddress = new(builder.Configuration["Benchmark:RestClient"]!));

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

app.MapGet("/rest", async ([AsParameters] Requests.RestRetrieveRequest request) =>
{
    var client = request.Factory.CreateClient("rest");
    var response = await client.GetAsync("/retrieve");

    return response.IsSuccessStatusCode
        ? Results.Ok()
        : Results.BadRequest();
});

app.MapPost("/rest", ([AsParameters] Requests.RestSubmitRequest request)
    => request.Factory.CreateClient("rest").PostAsJsonAsync("/submit", new { }));

// app.UseHttpsRedirection();

try
{
    app.UseGrpcClientMultiplexed<BenchmarkService.BenchmarkServiceClient>(builder.Configuration["Benchmark:GrpcClient"]!);
    
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