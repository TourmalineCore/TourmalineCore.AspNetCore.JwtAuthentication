using System;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Shared.Models
{
    public class BaseTokenModel
    {
        public string Value { get; set; }

        public DateTime ExpiresInUtc { get; set; }
    }
}