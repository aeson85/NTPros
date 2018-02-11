using System.Collections.Generic;
using IdentityServer4.Models;
using IdentityServer4.Extensions;
using IdentityServer4;
using static IdentityServer4.Models.IdentityResources;

namespace NT_Auth.Infrastructure
{
    public class Config
    {
        public static IEnumerable<ApiResource> GetApiResources()
        {
            yield return new ApiResource("msgpublish_api", "MessagePublishServer Api");
            yield return new ApiResource("cache_api", "CacheServer Api");
        }

        public static IEnumerable<Client> GetClients()
        {
            yield return new Client
            {
                ClientId = "mvc",
                ClientSecrets = new List<Secret>
                {
                    new Secret("secret".Sha256())
                },
                AllowedScopes = new List<string>
                {
                    "msgpublish_api",
                    "cache_api",
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile
                },
                RequireConsent = false,
                AllowOfflineAccess = true,
                AllowedGrantTypes = GrantTypes.HybridAndClientCredentials,
                RedirectUris = new List<string>
                {
                    "http://localhost:5000/signin-oidc"
                }
            };
        }

        public static IEnumerable<IdentityResource> GetIdentityResource()
        {
            yield return new OpenId();
            yield return new Profile();
        }
    }
}