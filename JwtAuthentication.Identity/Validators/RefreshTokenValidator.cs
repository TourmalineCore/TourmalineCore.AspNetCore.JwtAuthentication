using System.Threading.Tasks;
using TourmalineCore.AspNetCore.JwtAuthentication.Shared.Errors;
using TourmalineCore.AspNetCore.JwtAuthentication.Shared.Models.Requests;

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
                throw new RefreshTokenOrFingerprintNotFoundException();
            }
        }
    }
}