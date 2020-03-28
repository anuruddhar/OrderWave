#region Modification Log
/*------------------------------------------------------------------------------------------------------------------------------------------------- 
   System      -   OrderWave
   Module      -   Core

Modification History:
==================================================================================================================================================
Date              Version      		Modify by              					Description
--------------------------------------------------------------------------------------------------------------------------------------------------
31/05/2015         	  1.0           Anuruddha   					        Initial Version
--------------------------------------------------------------------------------------------------------------------------------------------------*/
#endregion
#region Namespace
using Microsoft.AspNetCore.Http;
using System;
using System.Configuration;
#endregion	  


namespace OrderWave.Core.UserInfo {
    public class UserHelper {
        public static string GetUsername() {
            var user = ConfigurationManager.AppSettings["user"];

            if (!string.IsNullOrEmpty(user)) {
                return user;
            }
            return user;

            //if (HttpContext.Current.User.Identity.Name.IndexOf("\\") > -1) {
            //    return HttpContext.Current.User.Identity.Name.Substring(HttpContext.Current.User.Identity.Name.IndexOf("\\") + 1);
            //}

            //return HttpContext.Current.User.Identity.Name;
        }

        public static string GetUsernameWithDomain() {
            var user = ConfigurationManager.AppSettings["user"];

            if (!string.IsNullOrEmpty(user)) {
                return user;
            }
            return user;
            // return HttpContext.Current.User.Identity.Name;
        }

        public static string GetFullName() {
            return GetUsername();
            //return ConfigurationManager.AppSettings["user"];
            //return "AARON.LAI";
            //return HttpContext.Current.User.Identity.Name;
        }

        public static string GetWorkstation() {

            try {
                string computer_name = string.Empty;
                //if (System.Net.Dns.GetHostEntry(HttpContext.Current.Request.ServerVariables["remote_addr"]) != null) {
                //    var hostName = System.Net.Dns.GetHostEntry(HttpContext.Current.Request.ServerVariables["remote_addr"]).HostName;
                //    //hostName = System.Net.Dns.GetHostName() ?? "";

                //    computer_name = Convert.ToString(hostName.Split('.')[0].ToUpper());
                //}
                return computer_name;
            } catch {
                return "no-hostname";
            }
        }
    }
}
