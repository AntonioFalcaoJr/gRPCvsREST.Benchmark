namespace Contracts.Models;

public record Category
{
    public Guid Id { get; init; }
    public string Name { get; init; }

    public static implicit operator GRPCvsREST.Benchmark.Category(Category product)
        => new()
        {
            Id = product.Id.ToString(),
            Name = product.Name
        };
}