using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace refactor_me.AuthorizationClasses
{
    internal class LocalSqlDbAuthorization : SqlAuthorization
    {
        /// <summary>
        /// Initialized the SQL Server to a local data file instance
        /// </summary>
        public LocalSqlDbAuthorization()
        {
            var conectionString = _connectionString.Replace("{DataDirectory}", HttpContext.Current.Server.MapPath("~/App_Data"));
            _connection = new SqlConnection(conectionString);
        }
    }
}