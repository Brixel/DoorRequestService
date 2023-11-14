using System.Threading.Tasks;

namespace DoorRequest.API.Services;

public interface IDoorService
{
    Task<bool> OpenDoor();
}