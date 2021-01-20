using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Core.Models.IdentityEntities
{
    public class Role : IdentityRole<string>
    {
        public IEnumerable<IdentityRoleClaim<long>> Claims { get; protected set; }

        public DateTime? DeletedAtUtc { get; set; }
    }
}