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
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]

    public class CustomerController : ControllerBase
    {
        [HttpPost("InsertInToCustomerTable_Secure_Identity_With_Log")]
        [Log]
        [Authorize]
        public string InsertInToCustomerTable([FromBody] CustomerView body)
        {
            Customer o = new Customer();
            o.FirstName = body.FirstName;
            o.LastName = body.LastName;
            Database.TestWebAPI.Save(o);
            return o.ID.ToString();
        }
        [HttpGet("GetFromCustomerTable")]
        public CustomerView GetFromCustomerTable(string ID)
        {
            var customer = Database.TestWebAPI.GetByID<Customer>(ID, false);
            CustomerView result = new CustomerView();
            if (customer == null)
            {
                return null;
            }
            result.FirstName = customer.FirstName;
            result.LastName = customer.LastName;
            return result;
        }
        /// <summary>
        /// we are create __KeyGenerator Stored Procedure 
        /// Please overwrite your own key generation policy in this procedure in sql server.
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        [HttpPost("InsertInToCustomer2Table_NoSecure_NoIdentity_NoLog")]
        public string InsertInToCustomer2Table([FromBody] CustomerView body)
        {
            Customer2 o = new Customer2();
            o.FirstName = body.FirstName;
            o.LastName = body.LastName;
            Database.TestWebAPI.Save(o);
            return o.ID;
        }
        [HttpPost("UpdateCustomer2Table_NoSecure_NoIdentity_NoLog")]
        public string UpdateCustomer2Table(string key, [FromBody] CustomerView body)
        {
            Database.TestWebAPI.BeginTransaction();
            Database.TestWebAPI.BeginTransaction();
            Customer2 o = new Customer2();
            o.ID = key;
            o.FirstName = body.FirstName;
            o.LastName = body.LastName;
            o.Description = null;//null Cause not in Updated columns list.
            Database.TestWebAPI.Save(o);
            Database.TestWebAPI.CommitTransaction();
            Database.TestWebAPI.CommitTransaction();
            return o.ID;
        }
        [HttpGet("GetFromCustomer2Table")]
        public CustomerView GetFromCustomer2Table(string ID)
        {
            var customer = Database.TestWebAPI.GetByID<Customer2>(ID, false);
            if (customer==null)
            {
                return null;
            }
            CustomerView result = new CustomerView();
            result.FirstName = customer.FirstName;
            result.LastName = customer.LastName;
            return result;
        }
        [HttpGet("ParametericFind")]
        public List<Customer> ParametericFind(string FirstName) 
        {
            var param = new Dictionary<string, string>();
            param.Add("FirstName", FirstName);
            List<Customer> customers = Database.TestWebAPI.Find<Customer>("FirstName=@FirstName", "", false, param).ToList();
            return customers;
               
        }
        [HttpGet("SimpleFind")]
        public List<Customer> SimpleFind(string FirstName)
        {
            List<Customer> customers = Database.TestWebAPI.Find<Customer>("FirstName='" + FirstName + "'", "", false, null).ToList();
            return customers;

        }
        [HttpGet("FindWithCustomFields")]
        public List<Customer2> FindWithCustomFields(string FirstName)
        {
            //in this function we want to select only ID And LastName
            var param = new Dictionary<string, string>();
            param.Add("FirstName", FirstName);
            List<Customer2> customers = Database.TestWebAPI.Find<Customer2>("FirstName=@FirstName", "", false, param, "ID,LastName").ToList();
            return customers;

        }
    }
}
