using System.Threading.Tasks;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.ErrorHandling;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Models.Request;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Identity.Validators
{
    public class RefreshTokenValidator : IValidator<RefreshTokenRequestModel>
    {
        public Task ValidateAsync(RefreshTokenRequestModel model)
        {
            return Task.Run(() => InternalValidate(model));
        }

        private void InternalValidate(RefreshTokenRequestModel model)
        {
            if (model == null || model.RefreshTokenValue == default)
            {
                throw new AuthenticationException(ErrorTypes.RefreshTokenOrFingerprintNotFound);
            }
        }
    }
}