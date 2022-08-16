using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Core.Contract
{
    internal interface IJwtTokenGenerator
    {
        string Generate(
            string issuer, 
            string audience, 
            List<Claim> claims, 
            DateTime expires, 
            SigningCredentials credentials, 
            string tokenType
        );
    }
}
