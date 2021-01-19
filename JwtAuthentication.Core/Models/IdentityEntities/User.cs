using System;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Identity;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Core.Models.IdentityEntities
{
    public class User : IdentityUser<long>, IUser<long>
    {
        public DateTime? DeletedAtUtc { get; set; }
    }
}