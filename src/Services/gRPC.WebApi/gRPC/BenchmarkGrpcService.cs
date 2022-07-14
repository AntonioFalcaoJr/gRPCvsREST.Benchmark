﻿using Grpc.Core;
using GRPCvsREST.Benchmark;

namespace gRPC.WebApi.gRPC;

public class BenchmarkGrpcService : BenchmarkService.BenchmarkServiceBase
{
    public override Task<RetrieveResponse> Retrieve(RetrieveRequest request, ServerCallContext context)
    {
        return Task.FromResult(new RetrieveResponse());
    }

    public override Task<SubmitResponse> Submit(SubmitRequest request, ServerCallContext context)
    {
        return Task.FromResult(new SubmitResponse());
    }
}