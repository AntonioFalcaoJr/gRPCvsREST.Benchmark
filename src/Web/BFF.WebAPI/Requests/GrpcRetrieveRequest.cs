using GRPCvsREST.Benchmark;
using Microsoft.AspNetCore.Mvc;

namespace web.Requests;

public class GrpcRetrieveRequest
{
    [FromServices]
    public BenchmarkService.BenchmarkServiceClient Client { get; set; }
}