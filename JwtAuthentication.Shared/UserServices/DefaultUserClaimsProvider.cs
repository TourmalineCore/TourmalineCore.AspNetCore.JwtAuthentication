using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using TourmalineCore.AspNetCore.JwtAuthentication.Shared.UserServices.Contracts;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Shared.UserServices
{
    public class DefaultUserClaimsProvider : IUserClaimsProvider
    {
        public Task<List<Claim>> GetUserClaimsAsync(string login)
        {
            return Task.FromResult(new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, login),
                    }
                );
        }
    }
}