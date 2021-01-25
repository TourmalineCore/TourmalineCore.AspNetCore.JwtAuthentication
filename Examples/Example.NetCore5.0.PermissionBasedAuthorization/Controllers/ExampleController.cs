using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Filters;

namespace Example.NetCore.PermissionBasedAuthorization.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ExampleController : ControllerBase
    {
        private static readonly string[] Summaries =
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching",
        };

        [Authorize]
        [RequiresPermission(UserClaimsProvider.FirstExampleClaimName)]
        [HttpGet]
        public IEnumerable<object> Get()
        {
            return Summaries;
        }
    }
}