using System.Threading.Tasks;

namespace DoorRequest.API.Services
{
    public interface IDoorRequestService
    {
        Task<bool> OpenDoor();
    }
}