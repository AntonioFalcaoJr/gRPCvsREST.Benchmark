using GRPCvsREST.Benchmark;
using Microsoft.AspNetCore.Mvc;

public class GrpcSubmitRequest
{
    [FromServices]
    public BenchmarkService.BenchmarkServiceClient Client { get; set; }
}