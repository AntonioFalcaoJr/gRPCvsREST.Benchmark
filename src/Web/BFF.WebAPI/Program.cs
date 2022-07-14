var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/grpc/retrieve", () => { });
app.MapPost("/grpc/submit", () => { });

app.MapGet("/rest/retrieve", () => { });
app.MapPost("/rest/submit", () => { });

app.UseHttpsRedirection();
await app.RunAsync();
