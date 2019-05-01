// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;
using IdentityServer4;
using IdentityServer4.Models;
using Microsoft.Extensions.Configuration;

namespace MicroServicesOnDocker.Services.TokenServiceApi
{
    public class Config
    {
        //public static Dictionary<string, string> ClientUrls { get; private set; }
        public static Dictionary<string, string> GetUrls(IConfiguration configuration)
        {
            Dictionary<string, string> urls = new Dictionary<string, string>();

            urls.Add("Mvc", configuration.GetValue<string>("MvcClient"));
            urls.Add("CartApi", configuration.GetValue<string>("CartApiClient"));
            urls.Add("OrderApi", configuration.GetValue<string>("OrderApiClient"));
            return urls;
        }
        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                 new ApiResource("cart", "Shopping Cart Api"),
                 new ApiResource("orders", "Ordering Api"),
            };
        }



        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>()
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
               // new IdentityResources.Email()
            };
        }
        public static IEnumerable<Client> GetClients(Dictionary<string, string> clientUrls)
        {
            //known client of the TokenServiceApi
            return new List<Client>()
            {
                new Client
                {
                    ClientId = "mvc",
                    ClientSecrets = new [] { new Secret("secret".Sha256())},
                    AllowedGrantTypes = GrantTypes.Hybrid,

                    RedirectUris = {$"{clientUrls["Mvc"]}/signin-oidc"},
                    PostLogoutRedirectUris = {$"{clientUrls["Mvc"]}/signout-callback-oidc"},
                    AllowAccessTokensViaBrowser = false,
                    AllowOfflineAccess = true,
                    RequireConsent = false,
                    AlwaysIncludeUserClaimsInIdToken = true,
                    AllowedScopes = new List<string>
                    {

                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                      //  IdentityServerConstants.StandardScopes.Email,
                        "orders",
                        "cart",
                    }
                },
                new Client
                {
                    ClientId = "cartswaggerui",
                    ClientName = "Shopping Cart HTTP API",
                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowAccessTokensViaBrowser = true,

                    //RedirectUris = {$"{clientUrls["CartApi"]}/swagger/o2c.html"},
                    RedirectUris = {$"{clientUrls["CartApi"]}/swagger/oauth2-redirect.html" },
                    PostLogoutRedirectUris = {$"{clientUrls["CartApi"]}/swagger/" },
                    AllowedScopes = new List<string>
                    {
                        "cart"
                    }
                },
                new Client
                {
                    ClientId = "orderswaggerui",
                    ClientName = "Order Swagger UI",
                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowAccessTokensViaBrowser = true,

                    RedirectUris = {$"{clientUrls["OrderApi"]}/swagger/oauth2-redirect.html" },
                    PostLogoutRedirectUris = {$"{clientUrls["OrderApi"]}/swagger/" },
                    AllowedScopes = new List<string>
                    {
                        "order"
                    }
                }
            };
        }

    }
}
