using System.Threading.Tasks;
using DoorRequest.API.Config;
using DoorRequest.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DoorRequest.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DoorRequestController : ControllerBase
    {
        private readonly IDoorRequestService _doorRequestService;
        private readonly LockConfiguration _lockConfiguration;

        public DoorRequestController(IDoorRequestService doorRequestService, IOptions<LockConfiguration> lockConfiguration)
        {
            _doorRequestService = doorRequestService;
            _lockConfiguration = lockConfiguration.Value;
        }

        [HttpPost("open")]
        [Authorize(Roles = Authorization.Roles.TwentyFourSevenAccess)]
        public async Task<bool> OpenDoorRequest()
        {
            return await _doorRequestService.OpenDoor();
        }

        [HttpGet("code")]
        [Authorize(Roles = Authorization.Roles.KeyVaultCodeAccess)]
        public int GetLockCode()
        {
            return _lockConfiguration.Code;
        }
    }
}