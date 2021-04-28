using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Identity
{
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Adds a user to the database with specified credentials
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <typeparam name="TUser"></typeparam>
        /// <param name="applicationBuilder"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseDefaultDbUser<TContext, TUser>(
            this IApplicationBuilder applicationBuilder, 
            string username, 
            string password
            ) 
            where TContext : JwtAuthIdentityDbContext<TUser> where TUser : IdentityUser
        {
            using var serviceScope = applicationBuilder.ApplicationServices.CreateScope();
            var context = serviceScope.ServiceProvider.GetRequiredService<TContext>();

            var user = Activator.CreateInstance<TUser>();

            user.Id = Guid.NewGuid().ToString();
            user.UserName = username;
            user.NormalizedUserName = username.ToUpper();
            user.EmailConfirmed = true;
            user.SecurityStamp = Guid.NewGuid().ToString();

            user.PasswordHash = new PasswordHasher<TUser>().HashPassword(user, password);

            context.Users.Add(user);
            context.SaveChanges();

            return applicationBuilder
                .UseAuthentication()
                .UseAuthorization();
        }
    }
}