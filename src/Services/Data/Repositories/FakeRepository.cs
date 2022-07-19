using Contracts.Models;

namespace Data.Repositories;

public class FakeRepository : IFakeRepository
{
    private List<Product> Products { get; }

    public FakeRepository()
        => Products = Seeder.SeedProducts(10000);

    public IEnumerable<Product> TakeProducts(int amount)
        => Products.Take(amount);
}