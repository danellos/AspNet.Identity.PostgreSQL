using System.Collections.Generic;

namespace AspNet.Identity.PostgreSQL
{
    /// <summary>
    /// Class that represents the AspNetUserRoles table in the PostgreSQL Database.
    /// </summary>
    public class UserRolesTable
    {
        private PostgreSQLDatabase _database;

        /// <summary>
        /// Constructor that takes a PostgreSQLDatabase instance.
        /// </summary>
        /// <param name="database"></param>
        public UserRolesTable(PostgreSQLDatabase database)
        {
            _database = database;
        }

        /// <summary>
        /// Returns a list of user's roles.
        /// 
        /// Correct by Slawomir Figiel
        /// </summary>
        /// <param name="userId">The user's id.</param>
        /// <returns></returns>
        public List<string> FindByUserId(string userId)
        {
            List<string> roles = new List<string>();

            //TODO: This probably does not work, and may need testing.
            //string commandText = "SELECT \"AspNetRoles\".\"Name\" FROM \"AspNetUsers\", \"AspNetRoles\", \"AspNetUserRoles\" ";
            //       commandText += "WHERE \"AspNetUsers\".\"Id\" = \"AspNetUserRoles\".\"UserId\" AND \"AspNetUserRoles\".\"RoleId\" = \"AspNetRoles\".\"Id\";";
            //string commandText = "SELECT \"Name\"" + " FROM \"AspNetRoles\" " + " INNER JOIN \"AspNetUserRoles\" ON \"AspNetUserRoles\".\"RoleId\" = \"AspNetRoles\".\"Id\" " 
            //                        + " WHERE \"UserId\" = @userId";

            string commandText = "SELECT \"AspNetRoles\".\"Name\" FROM \"AspNetRoles\" JOIN \"AspNetUserRoles\" ON \"AspNetUserRoles\".\"RoleId\" = \"AspNetRoles\".\"Id\" WHERE \"AspNetUserRoles\".\"UserId\" = @userId;";

            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@userId", userId);

            var rows = _database.Query(commandText, parameters);
            foreach(var row in rows)
            {
                roles.Add(row["Name"]);
            }

            return roles;
        }

        /// <summary>
        /// Deletes role from a user in the AspNetUserRoles table.
        /// </summary>
        /// <param name="userId">The user's id.</param>
        /// <returns></returns>
        public int Delete(string userId, string role)
        {
            string commandText = "DELETE FROM \"AspNetUserRoles\" WHERE \"UserId\" = @userId AND \"RoleId\" = @Role;";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("UserId", userId);
            parameters.Add("Role", role);

            return _database.Execute(commandText, parameters);
        }

        /// <summary>
        /// Deletes all roles from a user in the AspNetUserRoles table.
        /// 
        /// Corrected by Slawomir Figiel
        /// </summary>
        /// <param name="userId">The user's id.</param>
        /// <returns></returns>
        public int Delete(string userId)
        {
            string commandText = "DELETE FROM \"AspNetUserRoles\" WHERE \"UserId\" = @userId";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("UserId", userId);

            return _database.Execute(commandText, parameters);
        }

        /// <summary>
        /// Inserts a new role record for a user in the UserRoles table.
        /// </summary>
        /// <param name="user">The User.</param>
        /// <param name="roleId">The Role's id.</param>
        /// <returns></returns>
        public int Insert(IdentityUser user, string roleId)
        {
            string commandText = "INSERT INTO \"AspNetUserRoles\" (\"UserId\", \"RoleId\") VALUES (@userId, @roleId)";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("userId", user.Id);
            parameters.Add("roleId", roleId);

            return _database.Execute(commandText, parameters);
        }
    }
}
