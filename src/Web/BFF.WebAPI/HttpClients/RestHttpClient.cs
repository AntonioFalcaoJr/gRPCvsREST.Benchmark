using Contracts.Models;

namespace BFF.WebAPI.HttpClients;

public class RestHttpClient : IRestHttpClient
{
    private readonly HttpClient _client;

    public RestHttpClient(HttpClient client)
    {
        _client = client;
    }

    public Task HealthAsync()
        => _client.GetAsync("/health");

    public Task<IEnumerable<Product>> RetrieveAsync(int amount)
        => _client.GetFromJsonAsync<IEnumerable<Product>>("/retrieve?amount=" + amount);

    public Task SubmitAsync()
        => _client.PostAsJsonAsync("/submit", new { });
}