using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;

namespace Gif.Service
{
    public class AuthConfig
    {
        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
            {
                new Client
                {
                    ClientId = "a3c677e9-1867-4456-883a-4f6fdca9ba6e",

                    // no interactive user, use the clientid/secret for authentication
                    AllowedGrantTypes = GrantTypes.ClientCredentials,

                    // secret for authentication
                    ClientSecrets =
                    {
                        new Secret("5Xe4mJi6Rcb4xL+3y/91XAqPwaUG3Dj9DVkuWE2wZxQ=".Sha256())
                    },

                    // scopes that client has access to
                    AllowedScopes = { "GIFBuyingCatalogue" },
                    AccessTokenLifetime = 7200 //2 hours
                }
            };
        }

        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("GIFBuyingCatalogue", "GIF Buying Catalogue")
            };
        }
    }
}
