using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using TourmalineCore.AspNetCore.JwtAuthentication.Shared.Models.Responses;

[assembly: InternalsVisibleTo("TourmalineCore.AspNetCore.JwtAuthentication.Identity")]

namespace TourmalineCore.AspNetCore.JwtAuthentication.Shared.Services.Contracts
{
    public interface IRefreshService
    {
        public Task<AuthResponseModel> RefreshAsync(Guid refreshTokenValue, string clientFingerPrint);
    }
}