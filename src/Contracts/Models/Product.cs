namespace Contracts.Models;

public record Product
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public string Price { get; init; }
    public int Stock { get; init; }
    public Category Category { get; init; }
    public Vendor Vendor { get; init; }
    public ProductStatus Status { get; init; }

    public static implicit operator GRPCvsREST.Benchmark.Product(Product product)
        => new()
        {
            Category = product.Category,
            Id = product.Id.ToString(),
            Name = product.Name,
            Price = product.Price,
            Status = product.Status.ToString(),
            Stock = product.Stock,
            Vendor = product.Vendor
        };
}