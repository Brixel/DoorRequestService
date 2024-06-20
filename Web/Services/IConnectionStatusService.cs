namespace Web.Services;

public interface IConnectionStatusService
{
    bool ApiReachable { get; }

    void AddOnConnectedAction(Func<CancellationToken, Task> action);
    void AddOnDisconnectedAction(Func<CancellationToken, Task> action);
    Task StartPeriodicChecks(CancellationToken ct);
}