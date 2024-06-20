namespace Web.Services;

public class ConnectionStatusService : IConnectionStatusService
{
    private readonly IApiService _apiService;
    public bool ApiReachable { get; private set; } = true;

    private readonly List<Func<CancellationToken, Task>> _onDisconnectedActions = new();
    private readonly List<Func<CancellationToken, Task>> _onConnectedActions = new();

    public ConnectionStatusService(IApiService apiService)
    {
        _apiService = apiService ?? throw new ArgumentNullException(nameof(apiService));
    }

    public async Task StartPeriodicChecks(CancellationToken ct)
    {
        var timer = new PeriodicTimer(TimeSpan.FromSeconds(1));
        while (!ct.IsCancellationRequested
                && await timer.WaitForNextTickAsync(ct))
        {
            var reachable = await _apiService.IsHealthy(ct);
            if (reachable != ApiReachable)
            {
                ApiReachable = reachable;
                if (reachable)
                    await OnConnected(ct);
                else
                    await OnDisconnected(ct);
            }
        }
    }

    public void AddOnDisconnectedAction(Func<CancellationToken, Task> action)
    {
        _onDisconnectedActions.Add(action);
    }

    public void AddOnConnectedAction(Func<CancellationToken, Task> action)
    {
        _onConnectedActions.Add(action);
    }

    private async Task OnDisconnected(CancellationToken ct)
    {
        foreach (var task in _onDisconnectedActions)
        {
            await task(ct);
        }
    }

    private async Task OnConnected(CancellationToken ct)
    {
        foreach (var task in _onConnectedActions)
        {
            await task(ct);
        }
    }
}