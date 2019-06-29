using System.Threading.Tasks;
using MQTTnet;

namespace DoorRequest.API.Services
{
    public class DoorRequestService : IDoorRequestService
    {
        private readonly IBrixelOpenDoorClient _brixelOpenDoorClient;

        public DoorRequestService(IBrixelOpenDoorClient brixelOpenDoorClient)
        {
            _brixelOpenDoorClient = brixelOpenDoorClient;
        }
        public async Task<bool> OpenDoor()
        {
            await _brixelOpenDoorClient.OpenDoor();
            return true;
        }
    }
}
