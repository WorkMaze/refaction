using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace refactor_me.Interfaces
{
    /// <summary>
    /// Authosization interface to be implemeneted by individual data sources
    /// </summary>
    public interface IAuthorization
    {
        Task<bool> Authorize(string authorizationToken, string className, string method);
    }   
}