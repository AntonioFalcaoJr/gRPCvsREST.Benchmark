namespace BFF.WebAPI.HttpClients;

public interface IRestHttpClient
{
    Task<HttpResponseMessage> RetrieveAsync();
    Task SubmitAsync();
}