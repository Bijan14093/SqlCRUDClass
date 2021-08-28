using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using TestWebAPI;
using TestWebAPI.Domain.Model;
using Repository;
using Microsoft.AspNetCore.Components;

namespace TestWebAPI.Log
{
    public class LogAttribute : ActionFilterAttribute
    {
        string UserName;
        string API_Controller;
        string API_Action;
        string Input;
        string output;
        string ClientAddress;
        DateTime StartTime;
        DateTime EndTime;
        IDatabase Database;
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            StartTime = DateTime.Now;
            UserName = filterContext.HttpContext.User.Identity.Name;
            API_Controller = filterContext.RouteData.Values["Controller"].ToString();
            API_Action = filterContext.RouteData.Values["action"].ToString();
            Input = filterContext.ActionArguments.ToJson();
            var ClientIpAddress = filterContext.HttpContext.Connection.RemoteIpAddress.ToString();
            ClientAddress = ClientIpAddress + ";" + GetHostName(ClientIpAddress);
            base.OnActionExecuting(filterContext);
        }
        public override void OnResultExecuted(ResultExecutedContext context)
        {

            IDatabase Database = (IDatabase)context.HttpContext.RequestServices.GetService(typeof(IDatabase));
            if (context.Result is ObjectResult)
            {
                output = ((ObjectResult)context.Result).Value.ToJson();
            }
            EndTime = DateTime.Now;
            //Log Result in Database
            Web_API_Response_Log apiLog = new Web_API_Response_Log();
            apiLog.ID = "";
            apiLog.UserName = UserName;
            apiLog.LogDate = DateTime.Now.ToString();
            apiLog.API_Controller = API_Controller;
            apiLog.API_Action = API_Action;
            apiLog.Input = Input;
            apiLog.Output = output;
            apiLog.ClientAddress = ClientAddress;
            apiLog.Duration = EndTime.Subtract(StartTime).TotalMilliseconds.ToString();
            Database.TestWebAPI_Log.Save(apiLog);
            base.OnResultExecuted(context);
        }
        public string GetHostName(string ipAddress)
        {
            try
            {
                IPHostEntry entry = Dns.GetHostEntry(ipAddress);
                if (entry != null)
                {
                    return entry.HostName;
                }
            }
            catch (SocketException ex)
            {
                //unknown host or
                //not every IP has a name
                //log exception (manage it)
            }

            return null;
        }
    }
}
