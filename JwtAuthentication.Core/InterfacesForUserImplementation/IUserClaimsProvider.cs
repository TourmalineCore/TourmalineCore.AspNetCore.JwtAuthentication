using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Core.InterfacesForUserImplementation
{
    public interface IUserClaimsProvider
    {
        Task<List<Claim>> GetUserClaimsAsync(string username);
    }
}