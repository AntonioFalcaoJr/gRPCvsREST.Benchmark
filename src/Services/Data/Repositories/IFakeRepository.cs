using Contracts.Models;

namespace Data.Repositories;

public interface IFakeRepository
{
    IEnumerable<Product> TakeProducts(int amount);
}