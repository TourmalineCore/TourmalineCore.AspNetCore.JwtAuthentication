using System;
using Microsoft.AspNetCore.Identity;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Identity
{
    public class GenericIdentityUser : IdentityUser<TKey> where TKey : IEquatable<TKey>
    {
        
    }
}