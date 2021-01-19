

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Models.IdentityEntities;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Core.Services.Identity
{
    public class CustomRoleStore<TRole> : IRoleStore<TRole> where TRole : Role
    {
        private static readonly Dictionary<long, TRole> Roles = new Dictionary<long, TRole>();

        public Task<IdentityResult> CreateAsync(TRole role, CancellationToken cancellationToken)
        {
            role.Id = Roles.Keys.Count > 0
                ? Roles.Keys.Last() + 1
                : 1;

            var isSuccess = Roles.TryAdd(role.Id, role);

            return isSuccess == false
                ? Task.FromResult(IdentityResult.Failed(
                            new IdentityError
                            {
                                Code = "Add role",
                                Description = "Invalid role id",
                            }
                        )
                    )
                : Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> DeleteAsync(TRole role, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public void Dispose()
        {
        }

        public Task<TRole> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<TRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            return Task.FromResult(Roles
                    .Values
                    .FirstOrDefault(x => x.NormalizedName == normalizedRoleName)
                );
        }

        public Task<string> GetNormalizedRoleNameAsync(TRole role, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<string> GetRoleIdAsync(TRole role, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<string> GetRoleNameAsync(TRole role, CancellationToken cancellationToken)
        {
            return Task.FromResult(role.Name);
        }

        public Task SetNormalizedRoleNameAsync(TRole role, string normalizedName, CancellationToken cancellationToken)
        {
            return Task.FromResult(role.NormalizedName = normalizedName);
        }

        public Task SetRoleNameAsync(TRole role, string roleName, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<IdentityResult> UpdateAsync(TRole role, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}