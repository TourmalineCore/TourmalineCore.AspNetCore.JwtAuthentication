using System;
using Microsoft.AspNetCore.Identity;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Core.Models.IdentityEntities
{
    public class User : IdentityUser<string>
    {
        public DateTime? DeletedAtUtc { get; set; }
    }
}