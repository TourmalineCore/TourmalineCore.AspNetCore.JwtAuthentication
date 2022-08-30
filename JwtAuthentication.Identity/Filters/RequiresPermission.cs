using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using TourmalineCore.AspNetCore.JwtAuthentication.Shared.Extensions;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Identity.Filters
{
    public class RequiresPermission : Attribute, IAuthorizationFilter
    {
        public static string ClaimType;

        private readonly List<string> _permissions;

        public RequiresPermission(params string[] permissions)
        {
            _permissions = permissions.ToList();
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            if (_permissions.Any(x => user.HasPermission(ClaimType, x)))
            {
                return;
            }

            context.Result = new ForbidResult();
        }
    }
}