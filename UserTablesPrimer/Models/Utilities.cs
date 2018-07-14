using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace UserTablesPrimer.Models
{
    public static class Utilities
    {
        private static Model1 db = new Model1();
        public static string GetName(this System.Security.Principal.IPrincipal usr)
        {
            var name = ((ClaimsIdentity)usr.Identity).FindFirst("Name");
            if (name != null)
                return name.Value;

            return "";
        }

        public static string GetSurname(this System.Security.Principal.IPrincipal usr)
        {
            var surname = ((ClaimsIdentity)usr.Identity).FindFirst("Surname");
            if (surname != null)
                return surname.Value;

            return "";
        }

        public static int GetTokens(this System.Security.Principal.IPrincipal usr)
        {
            var tokens = ((ClaimsIdentity)usr.Identity).FindFirst("Tokens");
            if (tokens != null)
                return Int32.Parse(tokens.Value);

            return 0;
        }

        public static string GetId(this System.Security.Principal.IPrincipal usr)
        {
            var id = ((ClaimsIdentity)usr.Identity).FindFirst("Id");
            if (id != null)
                return id.Value;

            return "";
        }

        public static string NewNots(this System.Security.Principal.IPrincipal usr)
        {
            var numOfNots = ((ClaimsIdentity)usr.Identity).FindFirst("NumOfNotifications");
            if (numOfNots != null) {
                if (numOfNots.Value == "0")
                {
                    return "";
                }
                else
                {
                    return "New!";
                }
            }

            return "";
        }

    }
}