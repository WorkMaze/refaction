using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using refactor_me.Models;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace refactor_me.Factories
{
    /// <summary>
    /// Initialized the SQL Server to a local data file instance
    /// </summary>
    internal class LocalSqlDbProductFactory : SqlProductFactory
    {       
        public LocalSqlDbProductFactory() : base()
        {
            var conectionString = _connectionString.Replace("{DataDirectory}", HttpContext.Current.Server.MapPath("~/App_Data"));
            _connection = new SqlConnection(conectionString);
        }        
    }
}