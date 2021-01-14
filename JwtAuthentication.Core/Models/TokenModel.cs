using System;

namespace JwtAuthentication.Core.Models
{
    internal class TokenModel
    {
        public string Value { get; set; }

        public DateTime ExpiresInUtc { get; set; }
    }
}