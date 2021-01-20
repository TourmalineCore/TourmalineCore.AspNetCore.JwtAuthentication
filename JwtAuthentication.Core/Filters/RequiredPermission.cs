using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Utils;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Core.Filters
{
    public class RequiredPermission : Attribute, IAuthorizationFilter
    {
        internal static string ClaimType;

        private readonly List<string> _permissions;

        public RequiredPermission(params string[] permissions)
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