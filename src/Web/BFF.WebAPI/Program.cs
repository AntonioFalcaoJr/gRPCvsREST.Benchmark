using GRPCvsREST.Benchmark;
using web.Requests;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddGrpcClient<BenchmarkService.BenchmarkServiceClient>(options => options.Address = new Uri(builder.Configuration["Benchmark:GrpcHost"]));

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/grpc", ([AsParameters] GrpcRetrieveRequest request) => request.Client.RetrieveAsync(new RetrieveRequest()).ResponseAsync);
app.MapPost("/grpc", ([AsParameters] GrpcSubmitRequest request) => request.Client.SubmitAsync(new SubmitRequest()).ResponseAsync);

app.MapGet("/rest", () => { });
app.MapPost("/rest", () => { });

app.UseHttpsRedirection();
await app.RunAsync();