using GRPCvsREST.Benchmark;
using Microsoft.AspNetCore.Mvc;

public class GrpcRetrieveRequest
{
    [FromServices]
    public BenchmarkService.BenchmarkServiceClient Client { get; set; }
}