using BFF.WebAPI;
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

builder.Services.AddGrpcClient<BenchmarkService.BenchmarkServiceClient>(client
    => client.Address = new(builder.Configuration["Benchmark:GrpcClient"]!));

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