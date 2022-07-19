using Bogus;
using Contracts.Models;

namespace Data;

public static class Seeder
{
    public static List<Product> SeedProducts(int amount)
        => new Faker<Product>()
            .RuleFor(p => p.Id, Guid.NewGuid())
            .RuleFor(p => p.Name, p => p.Commerce.Product())
            .RuleFor(p => p.Price, p => p.Commerce.Price(100, 1000, 2, "U$ "))
            .RuleFor(p => p.Stock, p => p.Random.Int(1, 10))
            .RuleFor(p => p.Status, p => p.PickRandom<ProductStatus>())
            .RuleFor(p => p.Category, p => new() {Id = Guid.NewGuid(), Name = p.Commerce.Department()})
            .RuleFor(p => p.Vendor, p => new() {Id = Guid.NewGuid(), Name = p.Company.CompanyName()})
            .Generate(amount);
}