using System;
using System.Threading.Tasks;
using AspNetCore.Totp.Interface;
using DoorRequest.API.Config;
using DoorRequest.API.Services;
using IdentityServer4.Extensions;
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
        private readonly ITotpValidator _totpValidator;
        private readonly IAccountKeyService _accountKeyService;
        private readonly LockConfiguration _lockConfiguration;

        public DoorRequestController(IDoorRequestService doorRequestService, ITotpValidator totpValidator, 
            IAccountKeyService accountKeyService, IOptions<LockConfiguration> lockConfiguration)
        {
            _doorRequestService = doorRequestService;
            _totpValidator = totpValidator;
            _accountKeyService = accountKeyService;
            _lockConfiguration = lockConfiguration.Value;
        }

        [HttpPost("open")]
        public async Task<bool> OpenDoorRequest([FromBody]int validationCode)
        {
            var user = User.GetSubjectId().Trim();
            var validationResult = _totpValidator.Validate(_accountKeyService.GetAccountKey(user), validationCode);
            if (!validationResult)
            {
                throw new Exception("Invalid validation token");
            }
            return await _doorRequestService.OpenDoor();
        }

        [HttpGet("code")]
        public int GetLockCode()
        {
            return _lockConfiguration.Code;
        }
    }
}