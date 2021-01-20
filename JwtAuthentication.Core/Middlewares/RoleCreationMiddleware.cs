using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Models.Request;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Services;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Core.Middlewares
{
    internal class RoleCreationMiddleware : AuthMiddlewareBase<IRoleCreationService, CreateRoleRequestModel, string>
    {
        public RoleCreationMiddleware(RequestDelegate next)
            : base(next)
        {
        }

        public async Task InvokeAsync(HttpContext context, IRoleCreationService roleCreationService)
        {
            await InvokeAsyncBase(context, roleCreationService);
        }

        protected override Task<string> ExecuteServiceMethod(CreateRoleRequestModel model, IRoleCreationService service)
        {
            return service.CreateRoleAsync(model);
        }
    }
}