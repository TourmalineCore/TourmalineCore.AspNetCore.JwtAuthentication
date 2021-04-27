using System;
using System.Threading.Tasks;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.ErrorHandling;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Models.Request;
using TourmalineCore.AspNetCore.JwtAuthentication.Identity.Models;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Identity.Validators
{
    public class RefreshTokenValidator : IValidator<RefreshTokenRequestModel>
    {
        public RefreshTokenValidator()
        {
        }

        public Task ValidateAsync(RefreshTokenRequestModel model)
        {
            return Task.Run(() => InternalValidate(model));
        }

        private void InternalValidate(RefreshTokenRequestModel model)
        {
            if (model == null || model.RefreshTokenValue == default(Guid))
            {
                throw new AuthenticationException(ErrorTypes.RefreshTokenOrFingerprintNotFound);
            }
        }
    }
}