using Data.Repositories;
using Google.Protobuf.Collections;
using Grpc.Core;
using GRPCvsREST.Benchmark;
using Product = Contracts.Models.Product;

namespace gRPC.WebApi.gRPC;

public class BenchmarkGrpcService : BenchmarkService.BenchmarkServiceBase
{
    private readonly IFakeRepository _fakeRepository;

    public BenchmarkGrpcService(IFakeRepository fakeRepository)
    {
        _fakeRepository = fakeRepository;
    }

    public override Task<HealthResponse> Health(HealthRequest request, ServerCallContext context)
    {
        return Task.FromResult(new HealthResponse());
    }

    public override async Task<RetrieveResponse> Retrieve(RetrieveRequest request, ServerCallContext context)
    {
        await Task.Yield();

        var products = _fakeRepository.TakeProducts(request.Amount);

        var response = new RetrieveResponse();

        response.Products.AddRange(products.Select(product => (GRPCvsREST.Benchmark.Product) product));

        return response;
    }

    public override Task<SubmitResponse> Submit(SubmitRequest request, ServerCallContext context)
    {
        return Task.FromResult(new SubmitResponse());
    }
}