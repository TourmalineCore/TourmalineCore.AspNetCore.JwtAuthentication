using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("JwtAuthentication.Identity")]
namespace TourmalineCore.AspNetCore.JwtAuthentication.Core.Models
{
    internal class TokenModel
    {
        public string Value { get; set; }

        public DateTime ExpiresInUtc { get; set; }
    }
}