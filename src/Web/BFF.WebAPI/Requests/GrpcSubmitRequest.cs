using GRPCvsREST.Benchmark;
using Microsoft.AspNetCore.Mvc;

namespace BFF.WebAPI.Requests;

public class GrpcSubmitRequest
{
    [FromServices]
    public BenchmarkService.BenchmarkServiceClient Client { get; set; }
}