using System;
using Microsoft.AspNetCore.Identity;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Identity.Models
{
    public abstract class RefreshToken<TUser> where TUser : IdentityUser
    {
        public long Id { get; set; }

        public Guid Value { get; set; }

        public DateTime ExpiresIn { get; set; }

        public bool IsActive { get; set; }

        public string ClientFingerPrint { get; set; }

        public string UserId { get; set; }

        public TUser User { get; set; }
    } 
    public class RefreshToken<TUser, TKey> where TUser : IdentityUser<TKey> where TKey : IEquatable<TKey>
    {
        public long Id { get; set; }

        public Guid Value { get; set; }

        public DateTime ExpiresIn { get; set; }

        public bool IsActive { get; set; }

        public string ClientFingerPrint { get; set; }

        public TKey UserId { get; set; }

        public TUser User { get; set; }
    }
}