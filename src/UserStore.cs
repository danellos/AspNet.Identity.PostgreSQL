using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AspNet.Identity.PostgreSQL
{
    /// <summary>
    /// Class that implements the key ASP.NET Identity user store interfaces
    /// </summary>
    public class UserStore<TUser> : IUserLoginStore<TUser>,
        IUserClaimStore<TUser>,
        IUserRoleStore<TUser>,
        IUserPasswordStore<TUser>,
        IUserSecurityStampStore<TUser>,
        IQueryableUserStore<TUser>,
        IUserEmailStore<TUser>,
        IUserStore<TUser>,
        IUserLockoutStore<TUser,string>,
        IUserTwoFactorStore<TUser, string>
        where TUser : IdentityUser
    {
        private UserTable<TUser> userTable;
        private RoleTable roleTable;
        private UserRolesTable userRolesTable;
        private UserClaimsTable userClaimsTable;
        private UserLoginsTable userLoginsTable;
   
        public PostgreSQLDatabase Database { get; private set; }

        public IQueryable<TUser> Users
        {
            get
            {
                //ToDo: Best performance
                return userTable.GetAllUsers().AsQueryable();
                //throw new NotImplementedException();
            }
        }


        /// <summary>
        /// Default constructor that initializes a new PostgreSQLDatabase instance using the Default Connection string.
        /// </summary>
        public UserStore()
        {
            new UserStore<TUser>(new PostgreSQLDatabase());
        }

        /// <summary>
        /// Constructor that takes a PostgreSQLDatabase as argument.
        /// </summary>
        /// <param name="database"></param>
        public UserStore(PostgreSQLDatabase database)
        {
            Database = database;
            userTable = new UserTable<TUser>(database);
            roleTable = new RoleTable(database);
            userRolesTable = new UserRolesTable(database);
            userClaimsTable = new UserClaimsTable(database);
            userLoginsTable = new UserLoginsTable(database);
        }

        /// <summary>
        /// Insert a new TUser in the AspNetUserTable.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task CreateAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            userTable.Insert(user);

            return Task.FromResult<object>(null);
        }

        /// <summary>
        /// Returns an TUser instance based on a userId query.
        /// </summary>
        /// <param name="userId">The user's Id.</param>
        /// <returns></returns>
        public Task<TUser> FindByIdAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentException("Null or empty argument: userId");
            }

            TUser result = userTable.GetUserById(userId) as TUser;
            if (result != null)
            {
                return Task.FromResult<TUser>(result);
            }

            return Task.FromResult<TUser>(null);
        }

        /// <summary>
        /// Returns an TUser instance based on a userName query.
        /// </summary>
        /// <param name="userName">The user's name.</param>
        /// <returns></returns>
        public Task<TUser> FindByNameAsync(string userName)
        {
            if (string.IsNullOrEmpty(userName))
            {
                throw new ArgumentException("Null or empty argument: userName");
            }

            List<TUser> result = userTable.GetUserByName(userName) as List<TUser>;


            if (result != null)
            {
                if (result.Count == 1)
                {
                    return Task.FromResult<TUser>(result[0]);
                }
                else if (result.Count > 1)
                {
                    //todo: exception for release mode?
#if DEBUG
                    throw new ArgumentException("More than one user record returned.");
#endif
                }
            }

            return Task.FromResult<TUser>(null);
        }

        /// <summary>
        /// Updates the AspNetUsersTable with the TUser instance values.
        /// </summary>
        /// <param name="user">TUser to be updated.</param>
        /// <returns></returns>
        public Task UpdateAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            userTable.Update(user);

            return Task.FromResult<object>(null);
        }

        public void Dispose()
        {
            if (Database != null)
            {
                Database.Dispose();
                Database = null;
            }
        }

        /// <summary>
        /// Inserts a claim to the AspNetUserClaims table for the given user.
        /// </summary>
        /// <param name="user">User to have claim added.</param>
        /// <param name="claim">Claim to be added.</param>
        /// <returns></returns>
        public Task AddClaimAsync(TUser user, Claim claim)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (claim == null)
            {
                throw new ArgumentNullException("user");
            }

            userClaimsTable.Insert(claim, user.Id);

            return Task.FromResult<object>(null);
        }

        /// <summary>
        /// Returns all claims for a given user.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<IList<Claim>> GetClaimsAsync(TUser user)
        {
            ClaimsIdentity identity = userClaimsTable.FindByUserId(user.Id);

            return Task.FromResult<IList<Claim>>(identity.Claims.ToList());
        }

        /// <summary>
        /// Removes a claim from a user.
        /// </summary>
        /// <param name="user">User to have claim removed.</param>
        /// <param name="claim">Claim to be removed.</param>
        /// <returns></returns>
        public Task RemoveClaimAsync(TUser user, Claim claim)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (claim == null)
            {
                throw new ArgumentNullException("claim");
            }

            userClaimsTable.Delete(user, claim);

            return Task.FromResult<object>(null);
        }

        /// <summary>
        /// Inserts a Login in the AspNetUserLogins table for a given User.
        /// </summary>
        /// <param name="user">User to have login added.</param>
        /// <param name="login">Login to be added.</param>
        /// <returns></returns>
        public Task AddLoginAsync(TUser user, UserLoginInfo login)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (login == null)
            {
                throw new ArgumentNullException("login");
            }

            userLoginsTable.Insert(user, login);

            return Task.FromResult<object>(null);
        }

        /// <summary>
        /// Returns an TUser based on the Login info.
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        public Task<TUser> FindAsync(UserLoginInfo login)
        {
            if (login == null)
            {
                throw new ArgumentNullException("login");
            }

            var userId = userLoginsTable.FindUserIdByLogin(login);
            if (userId != null)
            {
                TUser user = userTable.GetUserById(userId) as TUser;
                if (user != null)
                {
                    return Task.FromResult<TUser>(user);
                }
            }

            return Task.FromResult<TUser>(null);
        }

        /// <summary>
        /// Returns list of UserLoginInfo for a given TUser.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user)
        {
            List<UserLoginInfo> userLogins = new List<UserLoginInfo>();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            List<UserLoginInfo> logins = userLoginsTable.FindByUserId(user.Id);
            if (logins != null)
            {
                return Task.FromResult<IList<UserLoginInfo>>(logins);
            }

            return Task.FromResult<IList<UserLoginInfo>>(null);
        }

        /// <summary>
        /// Deletes a login from AspNetUserLogins table for a given TUser.
        /// </summary>
        /// <param name="user">User to have login removed.</param>
        /// <param name="login">Login to be removed.</param>
        /// <returns></returns>
        public Task RemoveLoginAsync(TUser user, UserLoginInfo login)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (login == null)
            {
                throw new ArgumentNullException("login");
            }

            userLoginsTable.Delete(user, login);

            return Task.FromResult<Object>(null);
        }

        /// <summary>
        /// Inserts a entry in the AspNetUserRoles table.
        /// </summary>
        /// <param name="user">User to have role added.</param>
        /// <param name="roleName">Name of the role to be added to user.</param>
        /// <returns></returns>
        public Task AddToRoleAsync(TUser user, string roleName)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (string.IsNullOrEmpty(roleName))
            {
                throw new ArgumentException("Argument cannot be null or empty: roleName.");
            }

            string roleId = roleTable.GetRoleId(roleName);
            if (!string.IsNullOrEmpty(roleId))
            {
                userRolesTable.Insert(user, roleId);
            }

            return Task.FromResult<object>(null);
        }

        /// <summary>
        /// Returns the roles for a given TUser.
        /// </summary>
        /// <param name="user">TUser object.</param>
        /// <returns></returns>
        public Task<IList<string>> GetRolesAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            List<string> roles = userRolesTable.FindByUserId(user.Id);
            {
                if (roles != null)
                {
                    return Task.FromResult<IList<string>>(roles);
                }
            }

            return Task.FromResult<IList<string>>(null);
        }

        /// <summary>
        /// Verifies if a user is in a role.
        /// </summary>
        /// <param name="user">TUser object.</param>
        /// <param name="role">Role string.</param>
        /// <returns></returns>
        public Task<bool> IsInRoleAsync(TUser user, string role)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (string.IsNullOrEmpty(role))
            {
                throw new ArgumentNullException("role");
            }

            List<string> roles = userRolesTable.FindByUserId(user.Id);
            {
                if (roles != null && roles.Contains(role))
                {
                    return Task.FromResult<bool>(true);
                }
            }

            return Task.FromResult<bool>(false);
        }

        /// <summary>
        /// Removes a user from a role.
        /// 
        /// Created by Slawomir Figiel
        /// </summary>
        /// <param name="user">TUser object.</param>
        /// <param name="role">Role string.</param>
        /// <returns></returns>
        public Task RemoveFromRoleAsync(TUser user, string role)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (role == null)
            {
                throw new ArgumentNullException("login");
            }

            string roleId = roleTable.GetRoleId(role);
            if (!string.IsNullOrEmpty(roleId))
            {
                userRolesTable.Delete(user.Id, roleId);
            }

            return Task.FromResult<object>(null);
        }

        /// <summary>
        /// Deletes a user.
        /// </summary>
        /// <param name="user">TUser object.</param>
        /// <returns></returns>
        public Task DeleteAsync(TUser user)
        {
            if (user != null)
            {
                userTable.Delete(user);
            }

            return Task.FromResult<Object>(null);
        }

        /// <summary>
        /// Returns the PasswordHash for a given TUser.
        /// </summary>
        /// <param name="user">TUser object.</param>
        /// <returns></returns>
        public Task<string> GetPasswordHashAsync(TUser user)
        {
            string passwordHash = userTable.GetPasswordHash(user.Id);

            return Task.FromResult<string>(passwordHash);
        }

        /// <summary>
        /// Verifies if user has password.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<bool> HasPasswordAsync(TUser user)
        {
            var hasPassword = !string.IsNullOrEmpty(userTable.GetPasswordHash(user.Id));

            return Task.FromResult<bool>(Boolean.Parse(hasPassword.ToString()));
        }

        /// <summary>
        /// Sets the password hash for a given TUser.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="passwordHash"></param>
        /// <returns></returns>
        public Task SetPasswordHashAsync(TUser user, string passwordHash)
        {
            user.PasswordHash = passwordHash;

            return Task.FromResult<Object>(null);
        }

        /// <summary>
        ///  Set security stamp.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="stamp"></param>
        /// <returns></returns>
        public Task SetSecurityStampAsync(TUser user, string stamp)
        {
            user.SecurityStamp = stamp;

            return Task.FromResult(0);

        }

        /// <summary>
        /// Get security stamp.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<string> GetSecurityStampAsync(TUser user)
        {
            return Task.FromResult(user.SecurityStamp);
        }

        /// <summary>
        /// Set email on user.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        public Task SetEmailAsync(TUser user, string email)
        {
            user.Email = email;
            userTable.Update(user);

            return Task.FromResult(0);

        }

        /// <summary>
        /// Get email from user.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<string> GetEmailAsync(TUser user)
        {
            return Task.FromResult(user.Email);
        }

        /// <summary>
        /// Get if user email is confirmed.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<bool> GetEmailConfirmedAsync(TUser user)
        {
            return Task.FromResult(user.EmailConfirmed);
        }

        /// <summary>
        /// Set when user email is confirmed.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="confirmed"></param>
        /// <returns></returns>
        public Task SetEmailConfirmedAsync(TUser user, bool confirmed)
        {
            user.EmailConfirmed = confirmed;
            userTable.Update(user);

            return Task.FromResult(0);
        }

        /// <summary>
        /// Get user by email.
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public Task<TUser> FindByEmailAsync(string email)
        {
            if (String.IsNullOrEmpty(email))
            {
                throw new ArgumentNullException("email");
            }

            List<TUser> result = userTable.GetUserByEmail(email) as List<TUser>;
            if (result != null && result.Count > 0)
            {
                return Task.FromResult<TUser>(result[0]);
            }

            return Task.FromResult<TUser>(null);
        }

        public Task<DateTimeOffset> GetLockoutEndDateAsync(TUser user)
        {
            throw new NotImplementedException();
        }

        public Task SetLockoutEndDateAsync(TUser user, DateTimeOffset lockoutEnd)
        {
            throw new NotImplementedException();
        }

        public Task<int> IncrementAccessFailedCountAsync(TUser user)
        {
            return Task<Int32>.FromResult<Int32>(1);
        }

        public Task ResetAccessFailedCountAsync(TUser user)
        {
            return Task.FromResult(false);
           
        }

        public Task<int> GetAccessFailedCountAsync(TUser user)
        {
            return Task<Int32>.FromResult<Int32>(1);
        }

        public Task<bool> GetLockoutEnabledAsync(TUser user)
        {
            return Task<bool>.Run<bool>(() =>
            {
                return false;
            });

        }

        public Task SetLockoutEnabledAsync(TUser user, bool enabled)
        {
            return Task.Run(() =>
            {

            });
        }

        public Task SetTwoFactorEnabledAsync(TUser user, bool enabled)
        {
            return Task.FromResult(false);
        }

        public Task<bool> GetTwoFactorEnabledAsync(TUser user)
        {
            return Task<bool>.Run<bool>(() =>
            {
                return false;
            });
        }
    }
}
