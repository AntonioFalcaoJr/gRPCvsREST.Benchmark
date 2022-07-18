namespace BFF.WebAPI.HttpClients;

public class RestHttpClient : IRestHttpClient
{
    private readonly HttpClient _client;

    public RestHttpClient(HttpClient client)
    {
        _client = client;
    }

    public Task<HttpResponseMessage> RetrieveAsync()
        => _client.GetAsync("/retrieve");

    public Task SubmitAsync()
        => _client.PostAsJsonAsync("/submit", new { });
}