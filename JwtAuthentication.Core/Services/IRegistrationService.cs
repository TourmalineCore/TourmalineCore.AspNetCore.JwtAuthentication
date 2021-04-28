using System;
using System.Threading.Tasks;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Models.Request;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Models.Response;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Core.Services
{
    internal interface IRegistrationService<TUser, TRegistrationRequestModel> 
        where TUser : class 
        where TRegistrationRequestModel : RegistrationRequestModel
    {
        Task<AuthResponseModel> RegisterAsync(TRegistrationRequestModel model, Func<TRegistrationRequestModel, TUser> mapping);
    }
}