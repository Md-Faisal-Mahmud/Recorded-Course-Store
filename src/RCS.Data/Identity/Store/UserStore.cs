﻿using Microsoft.AspNetCore.Identity;
using NHibernate;
using NHibernate.Linq;
using RCS.Data.Identity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace RCS.Data.Identity.Store
{
    public class UserStore<TUser, TRole> :
    UserStoreBase<TUser, TRole, Guid, ApplicationUserClaim, ApplicationUserRole, ApplicationUserLogin, ApplicationUserToken, ApplicationRoleClaim>,
    IProtectedUserStore<TUser> where TUser : IdentityUser<Guid> where TRole : ApplicationRole
    {
        private readonly ISession _session;

        public UserStore(ISession session, IdentityErrorDescriber errorDescriber) : base(errorDescriber) =>
            _session = session ?? throw new ArgumentNullException(nameof(session));

        public override IQueryable<TUser> Users => _session.Query<TUser>();

        private IQueryable<TRole> Roles => _session.Query<TRole>();

        private IQueryable<ApplicationUserClaim> UserClaims => _session.Query<ApplicationUserClaim>();

        private IQueryable<ApplicationUserRole> UserRoles => _session.Query<ApplicationUserRole>();

        private IQueryable<ApplicationUserLogin> UserLogins => _session.Query<ApplicationUserLogin>();

        private IQueryable<ApplicationUserToken> UserTokens => _session.Query<ApplicationUserToken>();

        public override async Task<IdentityResult> CreateAsync(TUser user,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            await _session.SaveAsync(user, cancellationToken);
            await FlushChangesAsync(cancellationToken);

            return IdentityResult.Success;
        }

        public override async Task<IdentityResult> UpdateAsync(TUser user,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            var exists = await Users.AnyAsync(
                u => u.Id.Equals(user.Id),
                cancellationToken
            );

            if (!exists)
            {
                return IdentityResult.Failed(
                    new IdentityError
                    {
                        Code = "UserNotExist",
                        Description = $"User with id {user.Id} does not exists!"
                    }
                );
            }

            user.ConcurrencyStamp = Guid.NewGuid().ToString("N");
            await _session.MergeAsync(user, cancellationToken);
            await FlushChangesAsync(cancellationToken);

            return IdentityResult.Success;
        }

        public override async Task<IdentityResult> DeleteAsync(TUser user,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            await _session.DeleteAsync(user, cancellationToken);
            await FlushChangesAsync(cancellationToken);

            return IdentityResult.Success;
        }

        public override async Task<TUser?> FindByIdAsync(string userId,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            var id = ConvertIdFromString(userId);
            var user = await _session.GetAsync<TUser>(id, cancellationToken);

            return user;
        }

        public virtual async Task<TUser?> FindByIdAsync(Guid userId,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            var user = await _session.GetAsync<TUser>(userId, cancellationToken);

            return user;
        }

        public override async Task<TUser?> FindByNameAsync(string normalizedUserName,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            var user = await Users.FirstOrDefaultAsync(
                u => u.NormalizedUserName == normalizedUserName,
                cancellationToken
            );

            return user;
        }

        protected override async Task<TRole?> FindRoleAsync(string normalizedRoleName,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            var role = await Roles.FirstOrDefaultAsync(
                r => r.NormalizedName == normalizedRoleName,
                cancellationToken
            );

            return role;
        }

        protected override async Task<ApplicationUserRole?> FindUserRoleAsync(Guid userId, Guid roleId,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            var userRole = await UserRoles.FirstOrDefaultAsync(
                ur => ur.UserId.Equals(userId) && ur.RoleId.Equals(roleId),
                cancellationToken
            );

            return userRole;
        }

        protected override async Task<TUser?> FindUserAsync(Guid userId,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            var user = await Users.FirstOrDefaultAsync(
                u => u.Id.Equals(userId),
                cancellationToken
            );

            return user;
        }

        protected override async Task<ApplicationUserLogin?> FindUserLoginAsync(Guid userId,
            string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            var userLogin = await UserLogins.FirstOrDefaultAsync(
                ul => ul.UserId.Equals(userId) && ul.LoginProvider == loginProvider
                    && ul.ProviderKey == providerKey,
                cancellationToken
            );

            return userLogin;
        }

        protected override async Task<ApplicationUserLogin?> FindUserLoginAsync(string loginProvider,
            string providerKey, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            var userLogin = await UserLogins.FirstOrDefaultAsync(
                ul => ul.LoginProvider == loginProvider
                    && ul.ProviderKey == providerKey,
                cancellationToken
            );

            return userLogin;
        }

        public override async Task AddToRoleAsync(TUser user, string normalizedRoleName,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (string.IsNullOrWhiteSpace(normalizedRoleName))
                throw new ArgumentNullException(nameof(normalizedRoleName));

            var role = await FindRoleAsync(
                normalizedRoleName,
                cancellationToken
            );

            if (role == null)
                throw new InvalidOperationException($"Role {normalizedRoleName} not found!");

            await _session.SaveAsync(CreateUserRole(user, role), cancellationToken);
            await FlushChangesAsync(cancellationToken);
        }

        public override async Task RemoveFromRoleAsync(TUser user, string normalizedRoleName,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (string.IsNullOrWhiteSpace(normalizedRoleName))
                throw new ArgumentNullException(nameof(normalizedRoleName));

            var role = await FindRoleAsync(
                normalizedRoleName,
                cancellationToken
            );

            if (role != null)
            {
                var userRole = await FindUserRoleAsync(user.Id, role.Id, cancellationToken);

                if (userRole != null)
                {
                    await _session.DeleteAsync(userRole, cancellationToken);
                    await FlushChangesAsync(cancellationToken);
                }
            }
        }

        public override async Task<IList<string>> GetRolesAsync(TUser user,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            var userId = user.Id;
            var query = from r in Roles
                        join ur in UserRoles on r.Id equals ur.RoleId
                        where ur.UserId == userId
                        select r.Name;

            var roles = await query.ToListAsync(cancellationToken);
            return roles;
        }

        public override async Task<bool> IsInRoleAsync(TUser user, string normalizedRoleName,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (string.IsNullOrWhiteSpace(normalizedRoleName))
                throw new ArgumentNullException(nameof(normalizedRoleName));

            var role = await FindRoleAsync(normalizedRoleName, cancellationToken);
            if (role != null)
            {
                var userRole = await FindUserRoleAsync(
                    user.Id,
                    role.Id,
                    cancellationToken
                );

                return userRole != null;
            }

            return false;
        }

        public override async Task<IList<Claim>> GetClaimsAsync(TUser user,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            var claims = await UserClaims.Where(
                    uc => uc.UserId.Equals(user.Id)
                )
                .Select(c => c.ToClaim())
                .ToListAsync(cancellationToken);

            return claims;
        }

        public override async Task AddClaimsAsync(TUser user, IEnumerable<Claim> claims,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (claims == null)
                throw new ArgumentNullException(nameof(claims));

            foreach (var claim in claims)
                await _session.SaveAsync(CreateUserClaim(user, claim), cancellationToken);

            await FlushChangesAsync(cancellationToken);
        }

        public override async Task ReplaceClaimAsync(TUser user, Claim claim, Claim newClaim,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (claim == null)
                throw new ArgumentNullException(nameof(claim));

            if (newClaim == null)
                throw new ArgumentNullException(nameof(newClaim));

            var matchedClaims = await UserClaims.Where(
                    uc => uc.UserId.Equals(user.Id) &&
                        uc.ClaimValue == claim.Value
                        && uc.ClaimType == claim.Type
                ).ToListAsync(cancellationToken);

            foreach (var matchedClaim in matchedClaims)
            {
                matchedClaim.ClaimType = newClaim.Type;
                matchedClaim.ClaimValue = newClaim.Value;
                await _session.UpdateAsync(matchedClaim, cancellationToken);
            }

            await FlushChangesAsync(cancellationToken);
        }

        public override async Task RemoveClaimsAsync(TUser user, IEnumerable<Claim> claims,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (claims == null)
                throw new ArgumentNullException(nameof(claims));

            foreach (var claim in claims)
            {
                var matchedClaims = await UserClaims.Where(
                        uc => uc.UserId.Equals(user.Id) &&
                            uc.ClaimValue == claim.Value
                            && uc.ClaimType == claim.Type
                    ).ToListAsync(cancellationToken);

                foreach (var matchedClaim in matchedClaims)
                    await _session.DeleteAsync(matchedClaim, cancellationToken);
            }

            await FlushChangesAsync(cancellationToken);
        }

        public override async Task AddLoginAsync(TUser user, UserLoginInfo login,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (login == null)
                throw new ArgumentNullException(nameof(login));

            await _session.SaveAsync(CreateUserLogin(user, login), cancellationToken);

            await FlushChangesAsync(cancellationToken);
        }

        public override async Task RemoveLoginAsync(TUser user, string loginProvider,
            string providerKey, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            var login = await FindUserLoginAsync(user.Id, loginProvider, providerKey, cancellationToken);

            if (login != null)
                await _session.DeleteAsync(login, cancellationToken);
        }

        public override async Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            var userId = user.Id;
            var logins = await UserLogins.Where(l => l.UserId.Equals(userId))
                .Select(
                    l => new UserLoginInfo(
                        l.LoginProvider,
                        l.ProviderKey,
                        l.ProviderDisplayName
                    )
                ).ToListAsync(cancellationToken);

            return logins;
        }

        public override async Task<TUser?> FindByLoginAsync(string loginProvider, string providerKey,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            var userLogin = await FindUserLoginAsync(loginProvider, providerKey, cancellationToken);
            if (userLogin != null)
                return await FindUserAsync(userLogin.UserId, cancellationToken);

            return null;
        }

        public override async Task<TUser?> FindByEmailAsync(string normalizedEmail,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            return await Users.FirstOrDefaultAsync(u => u.NormalizedEmail == normalizedEmail, cancellationToken);
        }

        public override async Task<IList<TUser>> GetUsersForClaimAsync(Claim claim,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (claim == null)
                throw new ArgumentNullException(nameof(claim));

            var query = from userClaim in UserClaims
                        join user in Users on userClaim.UserId equals user.Id
                        where userClaim.ClaimValue == claim.Value
                            && userClaim.ClaimType == claim.Type
                        select user;

            return await query.ToListAsync(cancellationToken);
        }

        public override async Task<IList<TUser>> GetUsersInRoleAsync(string normalizedRoleName,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (string.IsNullOrEmpty(normalizedRoleName))
                throw new ArgumentNullException(normalizedRoleName);

            var role = await FindRoleAsync(normalizedRoleName, cancellationToken);

            if (role != null)
            {
                var query = from userRole in UserRoles
                            join user in Users on userRole.UserId equals user.Id
                            where userRole.RoleId.Equals(role.Id)
                            select user;

                return await query.ToListAsync(cancellationToken);
            }

            return new List<TUser>();
        }

        protected override async Task<ApplicationUserToken?> FindTokenAsync(TUser user,
            string loginProvider, string name, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            var token = await UserTokens.FirstOrDefaultAsync(
                ut => ut.UserId.Equals(user.Id) &&
                    ut.LoginProvider == loginProvider
                    && ut.Name == name,
                cancellationToken);

            return token;
        }

        public override async Task SetTokenAsync(TUser user, string loginProvider,
            string name, string? value, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            var userToken = await FindTokenAsync(user, loginProvider, name, cancellationToken);

            if (userToken == null)
            {
                userToken = CreateUserToken(user, loginProvider, name, value);
                await AddUserTokenAsync(userToken);
            }
            else
            {
                userToken.Value = value;
            }

            await FlushChangesAsync(cancellationToken);
        }

        protected override async Task AddUserTokenAsync(ApplicationUserToken token)
        {
            ThrowIfDisposed();

            if (token == null)
                throw new ArgumentNullException(nameof(token));

            await _session.SaveAsync(token);
            await FlushChangesAsync();
        }

        protected override async Task RemoveUserTokenAsync(ApplicationUserToken token)
        {
            ThrowIfDisposed();
            if (token == null)
                throw new ArgumentNullException(nameof(token));

            await _session.DeleteAsync(token);
            await FlushChangesAsync();
        }

        private async Task FlushChangesAsync(CancellationToken cancellationToken = default)
        {
            await _session.FlushAsync(cancellationToken);
            _session.Clear();
        }
    }
}
