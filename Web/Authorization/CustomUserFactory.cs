﻿using System.Security.Claims;
using System.Text.Json;

using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication.Internal;

namespace Web.Authorization;

public class CustomUserFactory : AccountClaimsPrincipalFactory<RemoteUserAccount>
{
    public CustomUserFactory(IAccessTokenProviderAccessor accessor)
        : base(accessor)
    {
    }

    public async override ValueTask<ClaimsPrincipal> CreateUserAsync(
        RemoteUserAccount account,
        RemoteAuthenticationUserOptions options)
    {
        var user = await base.CreateUserAsync(account, options);
        var claimsIdentity = (ClaimsIdentity)(user.Identity
            ?? throw new Exception("Could not get user's Identity property"));

        if (account != null)
        {
            MapArrayClaimsToMultipleSeparateClaims(account, claimsIdentity);
        }

        return user;
    }

    private static void MapArrayClaimsToMultipleSeparateClaims(RemoteUserAccount account, ClaimsIdentity claimsIdentity)
    {
        foreach (var prop in account.AdditionalProperties)
        {
            var key = prop.Key;
            var value = prop.Value;
            if (value != null &&
                (value is JsonElement element && element.ValueKind == JsonValueKind.Array))
            {
                claimsIdentity.RemoveClaim(claimsIdentity.FindFirst(prop.Key));
                var claims = element.EnumerateArray()
                    .Select(x => new Claim(prop.Key, x.ToString()));
                claimsIdentity.AddClaims(claims);
            }
        }
    }
}