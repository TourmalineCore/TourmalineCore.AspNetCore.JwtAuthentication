using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.ErrorHandling;
using TourmalineCore.AspNetCore.JwtAuthentication.Identity.Models;
using TourmalineCore.AspNetCore.JwtAuthentication.Identity.Validators;

namespace Bex.Authentication.Core.Validators.Impl
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