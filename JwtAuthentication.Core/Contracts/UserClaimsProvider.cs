using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using TourmalineCore.AspNetCore.JwtAuthentication.Shared.UserServices.Contracts;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Core.Contracts
{
    public abstract class UserClaimsProvider : IUserClaimsProvider
    {
        public abstract Task<List<Claim>> GetUserClaimsAsync(string login);
    }
}
