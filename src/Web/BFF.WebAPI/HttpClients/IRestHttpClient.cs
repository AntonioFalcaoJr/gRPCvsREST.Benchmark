using Contracts.Models;

namespace BFF.WebAPI.HttpClients;

public interface IRestHttpClient
{
    Task HealthAsync();
    Task<IEnumerable<Product>> RetrieveAsync(int amount);
    Task SubmitAsync();
}