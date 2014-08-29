using System.Collections.Generic;
using System.Security.Claims;

namespace AspNet.Identity.PostgreSQL
{
    /// <summary>
    /// Class that represents the AspNetUserClaims table in the PostgreSQL Database.
    /// </summary>
    public class UserClaimsTable
    {
        private PostgreSQLDatabase _database;

        /// <summary>
        /// Constructor that takes a PostgreSQLDatabase instance.
        /// </summary>
        /// <param name="database"></param>
        public UserClaimsTable(PostgreSQLDatabase database)
        {
            _database = database;
        }

        /// <summary>
        /// Returns a ClaimsIdentity instance given a userId.
        /// </summary>
        /// <param name="userId">The user's id.</param>
        /// <returns></returns>
        public ClaimsIdentity FindByUserId(string userId)
        {
            ClaimsIdentity claims = new ClaimsIdentity();
            string commandText = "SELECT * FROM \"AspNetUserClaims\" WHERE \"UserId\" = @userId";
            Dictionary<string, object> parameters = new Dictionary<string, object>() { { "@UserId", userId } };

            var rows = _database.Query(commandText, parameters);
            foreach (var row in rows)
            {
                Claim claim = new Claim(row["ClaimType"], row["ClaimValue"]);
                claims.AddClaim(claim);
            }

            return claims;
        }

        /// <summary>
        /// Deletes all claims from a user given a userId.
        /// </summary>
        /// <param name="userId">The user's id.</param>
        /// <returns></returns>
        public int Delete(string userId)
        {
            string commandText = "DELETE FROM \"AspNetUserClaims\" WHERE \"UserId\" = @userId";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("userId", userId);

            return _database.Execute(commandText, parameters);
        }

        /// <summary>
        /// Inserts a new claim record in AspNetUserClaims table.
        /// </summary>
        /// <param name="userClaim">User's claim to be added.</param>
        /// <param name="userId">User's Id.</param>
        /// <returns></returns>
        public int Insert(Claim userClaim, string userId)
        {
            string commandText = "INSERT INTO \"AspNetUserClaims\" (\"ClaimValue\", \"ClaimType\", \"UserId\") VALUES (@value, @type, @userId)";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("value", userClaim.Value);
            parameters.Add("type", userClaim.Type);
            parameters.Add("userId", userId);

            return _database.Execute(commandText, parameters);
        }

        /// <summary>
        /// Deletes a claim record from a user.
        /// </summary>
        /// <param name="user">The user to have a claim deleted.</param>
        /// <param name="claim">A claim to be deleted from user.</param>
        /// <returns></returns>
        public int Delete(IdentityUser user, Claim claim)
        {
            string commandText = "DELETE FROM \"AspNetUserClaims\" WHERE \"UserId\" = @userId AND @ClaimValue = @value AND ClaimType = @type";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("userId", user.Id);
            parameters.Add("value", claim.Value);
            parameters.Add("type", claim.Type);

            return _database.Execute(commandText, parameters);
        }
    }
}
