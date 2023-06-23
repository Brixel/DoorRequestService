using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace DoorRequest.API.Services;

public class DoorRequestService : IDoorRequestService
{
    private readonly IBrixelOpenDoorClient _brixelOpenDoorClient;
    private readonly ILogger _logger;

    public DoorRequestService(IBrixelOpenDoorClient brixelOpenDoorClient, ILogger<DoorRequestService> logger)
    {
        _brixelOpenDoorClient = brixelOpenDoorClient;
        _logger = logger;
    }
    public async Task<bool> OpenDoor()
    {
        _logger.LogInformation("Sending request to open the door via MQTT");
        return await _brixelOpenDoorClient.OpenDoor();
    }
}
