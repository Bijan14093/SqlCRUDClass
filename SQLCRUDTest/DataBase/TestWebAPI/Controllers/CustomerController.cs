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

    /// <summary>
    /// This Table is identity 
    /// </summary>
    public class CustomerController : ControllerBase
    {
        IDatabase Database;
       public CustomerController(IDatabase database)
        {
            Database = database;
        }
        [HttpPost("Insert")]
        [Log]
        /// <summary>
        /// This Table is identity 
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>

        public string InsertInToCustomerTable([FromBody] CustomerView body)
        {
            Customer o = new Customer();
            o.FirstName = body.FirstName;
            o.LastName = body.LastName;
            Database.TestWebAPI.Save(o);
            return o.ID.ToString();
        }

        [HttpPost("Update")]
        public string UpdateCustomer2Table(int key, [FromBody] CustomerView body)
        {
            if (key == 0)
            {
                throw new Exception("you must be used key that not equal zero on update. this cause insert new row.");
            }
            try
            {
                Database.TestWebAPI.BeginTransaction();
                Customer o = new Customer();
                o.ID = key;
                o.FirstName = body.FirstName;
                o.LastName = body.LastName;
                Database.TestWebAPI.Save(o);

                o = new Customer();
                o.ID = key;
                Database.TestWebAPI.Save(o); //Nothing happens

                Database.TestWebAPI.CommitTransaction();
                return o.ID.ToString();

            }
            catch (Exception)
            {
                Database.TestWebAPI.RollbackTransaction();
                throw;
            }
        }

        [HttpPost("Delete")]
        public bool Delete(int key)
        {
            try
            {
                Database.TestWebAPI.BeginTransaction();
                Customer o = new Customer();
                o.ID = key;
                Database.TestWebAPI.Delete(o);
                Database.TestWebAPI.CommitTransaction();
                return true;
            }
            catch (Exception)
            {

                Database.TestWebAPI.RollbackTransaction();
                return false;

            }
            
        }

        [HttpGet("GetByID")]
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
        public List<Customer> FindWithCustomFields(string FirstName)
        {
            //in this function we want to select only ID And LastName
            var param = new Dictionary<string, string>();
            param.Add("FirstName", FirstName);
            List<Customer> customers = Database.TestWebAPI.Find<Customer>("FirstName=@FirstName", "", false, param, "ID,LastName").ToList();
            return customers;

        }
    }
}
