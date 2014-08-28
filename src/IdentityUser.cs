using Microsoft.AspNet.Identity;
using System;

namespace AspNet.Identity.PostgreSQL
{
    /// <summary>
    /// Class that implements the ASP.NET Identity IUser interface.
    /// </summary>
    public class IdentityUser : IUser
    {
        /// <summary>
        /// Default constructor. 
        /// </summary>
        public IdentityUser()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Constructor that takes user name as argument.
        /// </summary>
        /// <param name="userName"></param>
        public IdentityUser(string userName)
            : this()
        {
            this.UserName = userName;
        }

        /// <summary>
        /// User ID.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// User's name.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Email.
        /// </summary>
        public virtual string Email { get; set; }

        /// <summary>
        /// True if the email is confirmed, default is false.
        /// </summary>
        public virtual bool EmailConfirmed { get; set; }

        /// <summary>
        /// The salted/hashed form of the user password.
        /// </summary>
        public virtual string PasswordHash { get; set; }

        /// <summary>
        /// A random value that should change whenever a users credentials have changed (password changed, login removed).
        /// </summary>
        public virtual string SecurityStamp { get; set; }

    
    }
}
