using Grpc.Core;
using GRPCvsREST.Benchmark;

public class BenchmarkGrpcService : BenchmarkService.BenchmarkServiceBase
{
    public override Task<RetrieveResponse> Retrieve(RetrieveRequest request, ServerCallContext context)
    {
        return Task.FromResult(new RetrieveResponse());
    }
}