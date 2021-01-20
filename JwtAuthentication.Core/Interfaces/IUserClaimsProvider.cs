using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Core.Interfaces
{
    public interface IUserClaimsProvider
    {
        Task<List<Claim>> GetUserClaimsAsync(string username);
    }
}