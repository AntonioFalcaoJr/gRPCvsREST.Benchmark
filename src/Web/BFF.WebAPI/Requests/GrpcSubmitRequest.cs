using GRPCvsREST.Benchmark;
using Microsoft.AspNetCore.Mvc;

namespace web.Requests;

public class GrpcSubmitRequest
{
    [FromServices]
    public BenchmarkService.BenchmarkServiceClient Client { get; set; }
}