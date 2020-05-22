using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DoorRequest.API
{
    public class FileBasedResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<FileBasedResourceOwnerPasswordValidator> _logger;

        public FileBasedResourceOwnerPasswordValidator(IConfiguration configuration, ILogger<FileBasedResourceOwnerPasswordValidator> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }
        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            var fileBasedAuthConfiguration = 
                _configuration
                    .GetSection("Authentication:FileBasedAuthentication")
                    .Get<FileBasedAuthenticationOptions>();
            if (!File.Exists(fileBasedAuthConfiguration.Path))
            {
                throw new Exception($"File not found: '{fileBasedAuthConfiguration.Path}'");
            }

            var linesInFile = await File.ReadAllLinesAsync(fileBasedAuthConfiguration.Path);
            FileUser user = null;
            foreach (var line in linesInFile)
            {
                var splitLine = line.Split(";");
                if (splitLine.Length != 2)
                {
                    continue;
                }

                user = FileUser.Create(splitLine[0], splitLine[1]);
                if (user.Username != context.UserName)
                {
                    user = null;
                    continue;
                }

                user.Validate(context.Password);
            }

            if (user == null)
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant);
                return;
            }
            var claims = new List<Claim>()
            {
                new Claim(JwtClaimTypes.Subject, context.UserName)
            };

            context.Result = new GrantValidationResult(subject: context.UserName,
                OidcConstants.AuthenticationMethods.Password, claims);
        }
    }
}