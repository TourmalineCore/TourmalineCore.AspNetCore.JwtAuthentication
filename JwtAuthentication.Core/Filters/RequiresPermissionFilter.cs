using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Utils;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Core.Filters
{
    public class RequiresPermissionFilter : Attribute, IAuthorizationFilter
    {
        private readonly List<string> _permissions;

        public RequiresPermissionFilter(params object[] permissions)
        {
            _permissions = permissions
                .Select(x => x.ToString())
                .ToList();

            var a = _permissions;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            if (_permissions.Any(x => user.HasPermission(x)))
            {
                return;
            }

            context.Result = new ForbidResult();
        }
    }
}