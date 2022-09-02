using BFF.WebAPI.HttpClients;
using GRPCvsREST.Benchmark;

namespace BFF.WebAPI;

public static class Requests
{
    public record struct GrpcHealthRequest(BenchmarkService.BenchmarkServiceClient Client);

    public record struct GrpcRetrieveRequest(BenchmarkService.BenchmarkServiceClient Client, int Amount);

    public record struct GrpcSubmitRequest(BenchmarkService.BenchmarkServiceClient Client);

    public record struct RestHealthRequest(IRestHttpClient Client);

    public record struct RestRetrieveRequest(IRestHttpClient Client, int Amount);

    public record struct RestSubmitRequest(IRestHttpClient Client);
}