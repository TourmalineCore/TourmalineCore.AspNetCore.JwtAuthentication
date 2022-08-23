using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Shared.UserServices.Contracts
{
    public interface IUserClaimsProvider
    {
        Task<List<Claim>> GetUserClaimsAsync(string login);
    }
}