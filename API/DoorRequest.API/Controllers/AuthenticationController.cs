﻿using AspNetCore.Totp.Interface;
using DoorRequest.API.DTOs;
using DoorRequest.API.Services;
using IdentityServer4.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DoorRequest.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AuthenticationController : ControllerBase
    {

        private readonly ITotpSetupGenerator _totpSetupGenerator;
        private readonly ITotpValidator _totpValidator;
        private readonly ILogger _logger;
        private readonly IAccountKeyService _accountKeyService;

        public AuthenticationController(ITotpSetupGenerator totpSetupGenerator, ITotpValidator totpValidator, 
            ILogger<AuthenticationController> logger, IAccountKeyService accountKeyService)
        {
            _totpSetupGenerator = totpSetupGenerator;
            _totpValidator = totpValidator;
            _logger = logger;
            _accountKeyService = accountKeyService;
        }
        
        
        [HttpGet("setup")]
        public SetupQRCodeDTO Setup()
        {
            var user = User.GetSubjectId();
            var setup = _totpSetupGenerator.Generate("Brixel-Entry", user, 
                _accountKeyService.GetAccountKey(user));

            _logger.LogInformation($"Generate setup code for {user}");

            return new SetupQRCodeDTO()
            {
                ManualSetupKey = setup.ManualSetupKey,
                Image = setup.QrCodeImage
            };
        }

        [HttpPost("validate")]
        public ValidatateSetupDTO Validate([FromBody]int validationCode)
        {
            var user = User.GetSubjectId();
            _logger.LogInformation($"Validating entered code for {user}");
            var validationResult = _totpValidator.Validate(_accountKeyService.GetAccountKey(user), validationCode);
            return new ValidatateSetupDTO()
            { 
                IsSuccess = validationResult
            };
        }
    }

}