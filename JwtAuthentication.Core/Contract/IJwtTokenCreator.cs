using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Models;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Core.Contract
{
    internal interface IJwtTokenCreator
    {
        Task<TokenModel> CreateAsync(string tokenType, List<Claim> claims, DateTime tokenExpiresIn);
    }
}
