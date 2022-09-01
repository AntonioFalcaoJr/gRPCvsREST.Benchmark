using Data.Repositories;
using Grpc.Core;
using GRPCvsREST.Benchmark;

namespace gRPC.WebApi.gRPC;

public class BenchmarkGrpcService : BenchmarkService.BenchmarkServiceBase
{
    private readonly IFakeRepository _fakeRepository;

    public BenchmarkGrpcService(IFakeRepository fakeRepository)
    {
        _fakeRepository = fakeRepository;
    }

    public override async Task<HealthResponse> Health(HealthRequest request, ServerCallContext context)
    {
        await Task.Yield();
        return new();
    }

    public override async Task<RetrieveResponse> Retrieve(RetrieveRequest request, ServerCallContext context)
    {
        await Task.Yield();

        var products = _fakeRepository.TakeProducts(request.Amount);

        var response = new RetrieveResponse();

        response.Products.AddRange(products.Select(product => (Product)product));

        return response;
    }

    public override async Task<SubmitResponse> Submit(SubmitRequest request, ServerCallContext context)
    {
        await Task.Yield();
        return new();
    }
}