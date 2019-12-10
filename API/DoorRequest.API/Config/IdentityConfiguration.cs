using System.Collections.Generic;

namespace DoorRequest.API.Config
{
    public class IdentityConfiguration
    {
        public string Authority { get;set; }
        public string ApiName { get; set; }

        public IReadOnlyList<string> AllowedOrigins { get; set; }
    }
}