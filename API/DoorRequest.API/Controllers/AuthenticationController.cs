using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNetCore.Totp.Interface;
using DoorRequest.API.DTOs;
using DoorRequest.API.Services;
using IdentityServer4.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DoorRequest.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AuthenticationController : ControllerBase
    {

        private readonly ITotpSetupGenerator _totpSetupGenerator;
        private readonly ITotpGenerator _totpGenerator;
        private readonly ITotpValidator _totpValidator;

        public AuthenticationController(ITotpSetupGenerator totpSetupGenerator, ITotpGenerator totpGenerator, ITotpValidator totpValidator)
        {
            _totpSetupGenerator = totpSetupGenerator;
            _totpGenerator = totpGenerator;
            _totpValidator = totpValidator;
        }
        [HttpGet("qr")]
        public QRCodeDTO GetQRCode()
        {
            var image = _totpGenerator.Generate(AuthenticationService.UniqueKey);
            return new QRCodeDTO()
            {
                Image = "bla"
            };
        }

        
        [HttpGet("setup")]
        public SetupQRCodeDTO Setup()
        {
            var user = User.GetSubjectId();
            var setup = _totpSetupGenerator.Generate("DoorRequestService", user, AuthenticationService.UniqueKey);

            return new SetupQRCodeDTO()
            {
                ManualSetupKey = setup.ManualSetupKey,
                Image = setup.QrCodeImage
            };
        }

        [HttpPost("validate")]
        public ValidatateSetupDTO Validate([FromBody]int validationCode)
        {
            var validationResult = _totpValidator.Validate(AuthenticationService.UniqueKey, validationCode);
            return new ValidatateSetupDTO()
            { 
                IsSuccess = validationResult
            };
        }
    }

}