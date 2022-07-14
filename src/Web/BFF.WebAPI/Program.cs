using GRPCvsREST.Benchmark;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddGrpcClient<BenchmarkService.BenchmarkServiceClient>(options => options.Address = new Uri("https://localhost:7110"));

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/grpc", ([AsParameters] GrpcBffRequest request) 
    => request.Client.RetrieveAsync(new RetrieveRequest()).ResponseAsync);
app.MapPost("/grpc", () => { });

app.MapGet("/rest", () => { });
app.MapPost("/rest", () => { });

app.UseHttpsRedirection();
await app.RunAsync();