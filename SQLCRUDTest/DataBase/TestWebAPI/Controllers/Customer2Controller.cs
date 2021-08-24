﻿using System;
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
    public class Customer2Controller : ControllerBase
    {
        [HttpPost("Insert")]
        [Log]
        public string Insert([FromBody] Customer2 body)
        {
            //this code generate exception.
            Customer2 o = new Customer2();
            o.FirstName = body.FirstName;
            o.LastName = body.LastName;
            Database.TestWebAPI.Save(o);
            return o.ID.ToString();
        }

        [HttpPost("Update")]
        public string Update(string key, [FromBody] Customer2 body)
        {
            //this code generate exception.
            Database.TestWebAPI.BeginTransaction();
            Customer2 o = new Customer2();
            o.ID = key;
            o.FirstName = body.FirstName;
            o.LastName = body.LastName;
            Database.TestWebAPI.Save(o);
            Database.TestWebAPI.CommitTransaction();
            return o.ID.ToString();
        }

        [HttpPost("Delete")]
        public bool Delete(string key)
        {
            //this code generate exception.
            try
            {
                Database.TestWebAPI.BeginTransaction();
                Customer2 o = new Customer2();
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
        public Customer2 GetFromCustomer2Table(string ID)
        {
            var Customer2 = Database.TestWebAPI.GetByID<Customer2>(ID, false);
            Customer2 result = new Customer2();
            if (Customer2 == null)
            {
                return null;
            }
            result.FirstName = Customer2.FirstName;
            result.LastName = Customer2.LastName;
            return result;
        }



        [HttpGet("ParametericFind")]
        public List<Customer2> ParametericFind(string FirstName) 
        {
            var param = new Dictionary<string, string>();
            param.Add("FirstName", FirstName);
            List<Customer2> Customer2s = Database.TestWebAPI.Find<Customer2>("FirstName=@FirstName", "", false, param).ToList();
            return Customer2s;
               
        }

        [HttpGet("SimpleFind")]
        public List<Customer2> SimpleFind(string FirstName)
        {
            List<Customer2> Customer2s = Database.TestWebAPI.Find<Customer2>("FirstName='" + FirstName + "'", "", false, null).ToList();
            return Customer2s;

        }

        [HttpGet("FindWithCustomFields")]
        public List<Customer2> FindWithCustomFields(string FirstName)
        {
            //in this function we want to select only ID And LastName
            var param = new Dictionary<string, string>();
            param.Add("FirstName", FirstName);
            List<Customer2> Customers = Database.TestWebAPI.Find<Customer2>("FirstName=@FirstName", "", false, param, "ID,LastName").ToList();
            return Customers;

        }
    }
}