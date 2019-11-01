using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.Extensions.Configuration;

namespace DoorRequest.API
{
    public class LDAPProfileService : IProfileService
    {
        private readonly IConfiguration _configuration;

        public LDAPProfileService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var userId = context.Subject.GetSubjectId();
            context.IssuedClaims.Add(new Claim("FullName", userId));
            return Task.CompletedTask;
        }

        public Task IsActiveAsync(IsActiveContext context)
        {
            context.IsActive = true;
            return Task.CompletedTask;
        }
    }
}