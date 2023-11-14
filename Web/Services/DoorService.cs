using System.Net.Http.Json;

namespace Web.Services;

public interface IDoorService
{
    Task<int> GetCode(CancellationToken ct);
    Task OpenDoor(CancellationToken ct);
}

public class DoorService : IDoorService
{
    private readonly HttpClient _httpClient;

    public DoorService(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public async Task OpenDoor(CancellationToken ct)
    {
        var response = await _httpClient.PostAsync("open", null, ct);
        response.EnsureSuccessStatusCode();
    }

    public Task<int> GetCode(CancellationToken ct)
        => _httpClient.GetFromJsonAsync<int>("code", ct);
}
