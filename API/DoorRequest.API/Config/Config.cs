using System.Collections.Generic;
using System.Linq;
using IdentityServer4.Models;

namespace DoorRequest.API.Config
{
    public class InMemoryInitConfig
    {
        // scopes define the resources in your system
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };
        }

        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("space-auth.api", "My API")
                {
                    Scopes = new List<string>(){"space-auth.api"}
                }
            };
        }

        // clients want to access resources (aka scopes)
        public static IEnumerable<Client> GetClients(IReadOnlyList<string> allowedOrigins)
        {
            // client credentials client
            return new List<Client>
            {
                // resource owner password grant client
                new Client
                {
                    ClientId = "space-auth-client",
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    AllowedScopes = { "space-auth.api" },
                    AllowedCorsOrigins = allowedOrigins.ToList()
                }
            };
        }

        public static IEnumerable<ApiScope> GetApiScopes()
        {
            return new List<ApiScope>()
            {
                new ApiScope("space-auth.api", "SpaceAuth.API scope")
            };
        }
    }
}
