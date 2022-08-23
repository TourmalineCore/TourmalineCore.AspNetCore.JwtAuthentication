using System;
using System.Threading.Tasks;
using TourmalineCore.AspNetCore.JwtAuthentication.Shared.Models.Requests;
using TourmalineCore.AspNetCore.JwtAuthentication.Shared.Models.Responses;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Shared.Services.Contracts
{
    public interface IRegistrationService<TUser, TRegistrationRequestModel>
        where TUser : class
        where TRegistrationRequestModel : RegistrationRequestModel
    {
        Task<AuthResponseModel> RegisterAsync(TRegistrationRequestModel model, Func<TRegistrationRequestModel, TUser> mapping);
    }
}