using Newtonsoft.Json;
using refactor_me.Interfaces;
using refactor_me.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace refactor_me.AuthorizationClasses
{
    /// <summary>
    /// SQL Server implementation for IAuthorization
    /// </summary>
    internal class SqlAuthorization : IAuthorization
    {
        protected string _connectionString;

        protected SqlConnection _connection;

        /// <summary>
        /// Reads the connection string from a JSON file
        /// </summary>
        public SqlAuthorization()
        {
            var sqlconfigText = File.ReadAllText(HttpContext.Current.Server.MapPath("~/") + "sqlconfig.json");

            if (string.IsNullOrEmpty(sqlconfigText))
                throw new Exception("Sql Server configuration not found.");

            var sqlConfiguration = JsonConvert.DeserializeObject<SqlConfiguration>(sqlconfigText);

            _connectionString = sqlConfiguration.ConnectionString;

            
        }

        /// <summary>
        /// Authorizes the base64 encoded basic authorization header
        /// </summary>
        /// <param name="authorizationToken"></param>
        /// <param name="className"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        public async Task<bool> Authorize(string authorizationToken, string className, string method)
        {
            var authorized = false;

            try
            {
                // Extract the Authorization header
                var authHeader = HttpContext.Current.Request.Headers["Authorization"];

                if (!string.IsNullOrEmpty(authHeader))
                {
                    // Extract the Basic Authorization token
                    var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();

                    // Decode the base64 encoded token
                    var base64EncodedBytes = System.Convert.FromBase64String(encodedUsernamePassword);
                    var decodedUsernamePassword = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);

                    // Extract userame and password
                    int seperatorIndex = decodedUsernamePassword.IndexOf(':');
                    var username = decodedUsernamePassword.Substring(0, seperatorIndex);
                    var password = decodedUsernamePassword.Substring(seperatorIndex + 1);


                    // Authorize against the DB
                    SqlCommand command = new SqlCommand("sp_Authorize", _connection);
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Id", username);
                    command.Parameters.AddWithValue("@Password", password);
                    command.Parameters.AddWithValue("@Class", className);
                    command.Parameters.AddWithValue("@Method", method);

                    _connection.Open();
                    object authorizedResult = await command.ExecuteScalarAsync();

                    authorized = authorizedResult != null && (int)authorizedResult == 1;
                }

            }
            catch
            {
                throw;
            }
            finally
            {
                if (_connection.State == System.Data.ConnectionState.Open)
                    _connection.Close();
            }

            return authorized;
        }
    }
}