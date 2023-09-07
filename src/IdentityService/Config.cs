﻿using Duende.IdentityServer.Models;

namespace IdentityService;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        new IdentityResource[] { new IdentityResources.OpenId(), new IdentityResources.Profile(), };

    public static IEnumerable<ApiScope> ApiScopes =>
        new ApiScope[] { new("auctionApp", "Auction app full access"), };

    public static IEnumerable<Client> Clients =>
        new Client[]
        {
            new()
            {
                ClientId = "postman",
                ClientName = "postman",
                AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                AllowedScopes = { "openid", "profile", "auctionApp" },
                RedirectUris = { "https://www.getpostman.com/oauth2/callback" },
                ClientSecrets = { new Secret("secret".Sha256()) },
            }
        };
}