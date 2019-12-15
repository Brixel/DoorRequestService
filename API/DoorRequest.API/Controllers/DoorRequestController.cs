﻿using System;
using System.Threading.Tasks;
using AspNetCore.Totp.Interface;
using DoorRequest.API.Services;
using IdentityServer4.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoorRequest.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DoorRequestController : ControllerBase
    {
        private readonly IDoorRequestService _doorRequestService;
        private readonly ITotpValidator _totpValidator;

        public DoorRequestController(IDoorRequestService doorRequestService, ITotpValidator totpValidator)
        {
            _doorRequestService = doorRequestService;
            _totpValidator = totpValidator;
        }
        [HttpPost("open")]
        public async Task<bool> OpenDoorRequest([FromBody]int validationCode)
        {
            var user = User.GetSubjectId();
            var validationResult = _totpValidator.Validate(AuthenticationService.GetAccountKey(user), validationCode);
            if (!validationResult)
            {
                throw new Exception("Invalid validation token");
            }
            return await _doorRequestService.OpenDoor();

        }
    }
}