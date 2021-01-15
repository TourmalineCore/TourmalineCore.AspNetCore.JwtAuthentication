using System;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Core.Models
{
    internal class TokenModel
    {
        public string Value { get; set; }

        public DateTime ExpiresInUtc { get; set; }
    }
}