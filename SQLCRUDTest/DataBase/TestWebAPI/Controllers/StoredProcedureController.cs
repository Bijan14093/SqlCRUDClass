using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TestWebAPI.Domain.Enum;
using TestWebAPI.Domain.Model;
using TestWebAPI.Domain.ViewModel;
using TestWebAPI.Log;

namespace TestWebAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class StoredProcedureController : ControllerBase
    {
        IDatabase Database;
        public StoredProcedureController(IDatabase database)
        {
            Database = database;
        }
        [HttpPost("Execute_SP_GetCustomerDetail")]
        [Log]
        /// <summary>
        /// we are create __KeyGenerator Stored Procedure 
        /// Please overwrite your own key generation policy in this procedure in sql server.
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        public List<Customer> Execute_SP_GetCustomerDetail([FromBody] CustomerView body)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("FirstName", body.FirstName);
            //you can not use writeonly object in execute procedure
            var result= Database.TestWebAPI.Execute_StoredProcedure<Customer,Customer,Customer2,ReadonlyCustomer>("SP_GetCustomerDetail", parameters);
            return (List<Customer>)result.First();
        }
    }
}
