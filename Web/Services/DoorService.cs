namespace Web.Services;

public interface IDoorService
{
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
}
