using System;
using System.Collections.Generic;

namespace AspNet.Identity.PostgreSQL
{
    /// <summary>
    /// Class that represents the AspNetUsers table in the PostgreSQL database.
    /// </summary>
    public class UserTable<TUser>
        where TUser :IdentityUser
    {
        private PostgreSQLDatabase _database;

        /// <summary>
        /// Constructor that takes a PostgreSQL database instance. 
        /// </summary>
        /// <param name="database"></param>
        public UserTable(PostgreSQLDatabase database)
        {
            _database = database;
        }

        /// <summary>
        /// Gets the user's name, provided with an ID.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public string GetUserName(string userId)
        {
            string commandText = "SELECT \"UserName\" FROM \"AspNetUsers\" WHERE \"Id\" = @id";
            Dictionary<string, object> parameters = new Dictionary<string, object>() { { "@id", userId } };

            return _database.GetStrValue(commandText, parameters);
        }

        /// <summary>
        /// Gets the user's ID, provided with a user name.
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public string GetUserId(string userName)
        {
            //Due to PostgreSQL's case sensitivity, we have another column for the user name in lowercase.
            if (userName != null)
                userName = userName.ToLower();

            string commandText = "SELECT \"Id\" FROM \"AspNetUsers\" WHERE LOWER(\"UserName\") = @name";
            Dictionary<string, object> parameters = new Dictionary<string, object>() { { "@name", userName } };

            return _database.GetStrValue(commandText, parameters);
        }

        /// <summary>
        /// Returns all users. Created by: Slawomir Figiel
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// 
        public List<TUser> GetAllUsers()
        {
            List<TUser> users = new List<TUser>();

            string commandText = "SELECT * FROM \"AspNetUsers\"";
            var rows = _database.Query(commandText, new Dictionary<string, object>());

            foreach (var row in rows)
            {
                TUser user = (TUser)Activator.CreateInstance(typeof(TUser));
                user.Id = row["Id"];
                user.UserName = row["UserName"];
                user.PasswordHash = string.IsNullOrEmpty(row["PasswordHash"]) ? null : row["PasswordHash"];
                user.SecurityStamp = string.IsNullOrEmpty(row["SecurityStamp"]) ? null : row["SecurityStamp"];
                user.Email = string.IsNullOrEmpty(row["Email"]) ? null : row["Email"];
                user.EmailConfirmed = row["EmailConfirmed"] == "True";
                users.Add(user);
            }

            return users;
        }

        public TUser GetUserById(string userId)
        {
            TUser user = null;
            string commandText = "SELECT * FROM \"AspNetUsers\" WHERE \"Id\" = @id";
            Dictionary<string, object> parameters = new Dictionary<string, object>() { { "@id", userId } };

            var rows = _database.Query(commandText, parameters);
            if (rows != null && rows.Count == 1)
            {
                var row = rows[0];
                user = (TUser)Activator.CreateInstance(typeof(TUser));
                user.Id = row["Id"];
                user.UserName = row["UserName"];
                user.PasswordHash = string.IsNullOrEmpty(row["PasswordHash"]) ? null : row["PasswordHash"];
                user.SecurityStamp = string.IsNullOrEmpty(row["SecurityStamp"]) ? null : row["SecurityStamp"];
                user.Email = string.IsNullOrEmpty(row["Email"]) ? null : row["Email"];
                user.EmailConfirmed = row["EmailConfirmed"] == "True";
            }

            return user;
        }

        /// <summary>
        /// Returns a list of TUser instances given a user name.
        /// </summary>
        /// <param name="userName">User's name.</param>
        /// <returns></returns>
        public List<TUser> GetUserByName(string userName)
        {
            //Due to PostgreSQL's case sensitivity, we have another column for the user name in lowercase.
            if (userName != null)
                userName = userName.ToLower();

            List<TUser> users = new List<TUser>();
            string commandText = "SELECT * FROM \"AspNetUsers\" WHERE LOWER(\"UserName\") = @name";
            Dictionary<string, object> parameters = new Dictionary<string, object>() { { "@name", userName } };

            var rows = _database.Query(commandText, parameters);
            foreach(var row in rows)
            {
                TUser user = (TUser)Activator.CreateInstance(typeof(TUser));
                user.Id = row["Id"];
                user.UserName = row["UserName"];
                user.PasswordHash = string.IsNullOrEmpty(row["PasswordHash"]) ? null : row["PasswordHash"];
                user.SecurityStamp = string.IsNullOrEmpty(row["SecurityStamp"]) ? null : row["SecurityStamp"];
                user.Email = string.IsNullOrEmpty(row["Email"]) ? null : row["Email"];
                user.EmailConfirmed = row["EmailConfirmed"] == "True";
                users.Add(user);
            }

            return users;
        }

        /// <summary>
        /// Returns a list of TUser instances given a user email.
        /// </summary>
        /// <param name="email">User's email address.</param>
        /// <returns></returns>
        public List<TUser> GetUserByEmail(string email)
        {
            //Due to PostgreSQL's case sensitivity, we have another column for the user name in lowercase.
            if (email != null)
                email = email.ToLower();

            List<TUser> users = new List<TUser>();
            string commandText = "SELECT * FROM \"AspNetUsers\" WHERE LOWER(\"Email\") = @email";
            Dictionary<string, object> parameters = new Dictionary<string, object>() { { "@email", email } };

            var rows = _database.Query(commandText, parameters);
            foreach (var row in rows)
            {
                TUser user = (TUser)Activator.CreateInstance(typeof(TUser));
                user.Id = row["Id"];
                user.UserName = row["UserName"];
                user.PasswordHash = string.IsNullOrEmpty(row["PasswordHash"]) ? null : row["PasswordHash"];
                user.SecurityStamp = string.IsNullOrEmpty(row["SecurityStamp"]) ? null : row["SecurityStamp"];
                user.Email = string.IsNullOrEmpty(row["Email"]) ? null : row["Email"];
                user.EmailConfirmed = row["EmailConfirmed"] == "True";
                users.Add(user);
            }

            return users;
        }

        /// <summary>
        /// Return the user's password hash.
        /// </summary>
        /// <param name="userId">The user's id.</param>
        /// <returns></returns>
        public string GetPasswordHash(string userId)
        {
            string commandText = "SELECT \"PasswordHash\" FROM \"AspNetUsers\" WHERE \"Id\" = @id";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@id", userId);

            var passHash = _database.GetStrValue(commandText, parameters);
            if(string.IsNullOrEmpty(passHash))
            {
                return null;
            }

            return passHash;
        }

        /// <summary>
        /// Sets the user's password hash.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="passwordHash"></param>
        /// <returns></returns>
        public int SetPasswordHash(string userId, string passwordHash)
        {
            string commandText = "UPDATE \"AspNetUsers\" SET \"PasswordHash\" = @pwdHash WHERE \"Id\" = @id";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@pwdHash", passwordHash);
            parameters.Add("@id", userId);

            return _database.Execute(commandText, parameters);
        }

        /// <summary>
        /// Returns the user's security stamp.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public string GetSecurityStamp(string userId)
        {
            string commandText = "SELECT \"SecurityStamp\" FROM \"AspNetUsers\" WHERE \"Id\" = @id";
            Dictionary<string, object> parameters = new Dictionary<string, object>() { { "@id", userId } };
            var result = _database.GetStrValue(commandText, parameters);

            return result;
        }

        /// <summary>
        /// Inserts a new user in the AspNetUsers table.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public int Insert(TUser user)
        {
            var lowerCaseEmail = user.Email == null ? null : user.Email.ToLower();

            string commandText = @"
            INSERT INTO ""AspNetUsers""(""Id"", ""UserName"", ""PasswordHash"", ""SecurityStamp"", ""Email"", 
                                        ""EmailConfirmed"")
            VALUES (@id, @name, @pwdHash, @SecStamp, @email, @emailconfirmed);";

            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@name", user.UserName);
            parameters.Add("@id", user.Id);
            parameters.Add("@pwdHash", user.PasswordHash);
            parameters.Add("@SecStamp", user.SecurityStamp);
            parameters.Add("@email", user.Email);
            parameters.Add("@emailconfirmed", user.EmailConfirmed);

            return _database.Execute(commandText, parameters);
        }

        /// <summary>
        /// Deletes a user from the AspNetUsers table.
        /// </summary>
        /// <param name="userId">The user's id.</param>
        /// <returns></returns>
        private int Delete(string userId)
        {
            string commandText = "DELETE FROM \"AspNetUsers\" WHERE \"Id\" = @userId";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@userId", userId);

            return _database.Execute(commandText, parameters);
        }

        /// <summary>
        /// Deletes a user from the AspNetUsers table.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public int Delete(TUser user)
        {
            return Delete(user.Id);
        }

        /// <summary>
        /// Updates a user in the AspNetUsers table.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public int Update(TUser user)
        {
            var lowerCaseEmail = user.Email == null ? null : user.Email.ToLower();

//            string commandText = @"
//                UPDATE ""AspNetUsers""
//                   SET ""UserName"" = @userName, ""PasswordHash"" = @pswHash, ""SecurityStamp"" = @secStamp, ""Email""= @email, 
//                       ""EmailConfirmed"" = @emailconfirmed
//                 WHERE ""Id"" = @userId;";

            string commandText = "UPDATE \"AspNetUsers\" SET \"UserName\" = @userName, \"PasswordHash\" = @pswHash, \"SecurityStamp\" = @secStamp, \"Email\"= @email, \"EmailConfirmed\" = @emailconfirmed WHERE \"Id\" = @userId;";

            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@userName", user.UserName);
            parameters.Add("@pswHash", user.PasswordHash);
            parameters.Add("@secStamp", user.SecurityStamp);
            parameters.Add("@userId", user.Id);
            parameters.Add("@email", user.Email);
            parameters.Add("@emailconfirmed", user.EmailConfirmed);

            return _database.Execute(commandText, parameters);
        }
    }
}
