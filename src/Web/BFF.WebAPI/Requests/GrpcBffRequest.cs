using GRPCvsREST.Benchmark;
using Microsoft.AspNetCore.Mvc;

public class GrpcBffRequest
{
    [FromServices]
    public BenchmarkService.BenchmarkServiceClient Client { get; set; }
}