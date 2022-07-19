using Data.Repositories;

namespace REST.WebApi;

public static class Requests
{
    public record struct TakeProductsRequest(IFakeRepository Repository, int Amount);
}