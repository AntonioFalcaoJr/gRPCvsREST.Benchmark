using GRPCvsREST.Benchmark;

namespace BFF.WebAPI;

public static class Requests
{
    public record struct GrpcRetrieveRequest(BenchmarkService.BenchmarkServiceClient Client);

    public record struct GrpcSubmitRequest(BenchmarkService.BenchmarkServiceClient Client);
}