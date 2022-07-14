using System.IO.Compression;

var builder = WebApplication.CreateBuilder(args);
builder.Services
    .AddGrpc(options => options.ResponseCompressionLevel = CompressionLevel.Fastest);

var app = builder.Build();
app.UseRouting();
app.UseEndpoints(endpoint => endpoint.MapGrpcService<BenchmarkGrpcService>());
await app.RunAsync();