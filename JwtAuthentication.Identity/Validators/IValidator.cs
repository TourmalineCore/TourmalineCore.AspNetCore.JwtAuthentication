using System.Threading.Tasks;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Identity.Validators
{
    public interface IValidator<in TModel>
        where TModel : class
    {
        Task ValidateAsync(TModel model);
    }
}