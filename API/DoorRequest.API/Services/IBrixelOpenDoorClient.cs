using System.Threading.Tasks;

namespace DoorRequest.API.Services;

public interface IBrixelOpenDoorClient
{
    Task<bool> OpenDoor();
}