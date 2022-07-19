namespace Contracts.Models;

public record Vendor
{
    public Guid Id { get; init; }
    public string Name { get; init; }

    public static implicit operator GRPCvsREST.Benchmark.Vendor(Vendor product)
        => new()
        {
            Id = product.Id.ToString(),
            Name = product.Name
        };
}