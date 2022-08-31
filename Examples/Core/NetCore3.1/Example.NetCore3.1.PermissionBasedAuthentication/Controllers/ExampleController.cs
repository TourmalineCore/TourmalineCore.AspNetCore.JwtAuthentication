using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Filters;

namespace Example.NetCore3._1.PermissionBasedAuthentication.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ExampleController : ControllerBase
    {
        private static readonly string[] Summaries =
        {
            "Freezing",
            "Bracing",
            "Chilly",
            "Cool",
            "Mild",
            "Warm",
            "Balmy",
            "Hot",
            "Sweltering",
            "Scorching",
        };

        [Authorize]
        [RequiresPermission(CustomUserClaimsProvider.FirstExampleClaimName)]
        [HttpGet]
        public IEnumerable<object> Get()
        {
            return Summaries;
        }
    }
}