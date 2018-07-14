using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UserTablesPrimer.Models
{
    public class UserPartialViewModel
    {
        private Model1 db = new Model1();
        public string HasNot { get; set; }

        public int Tokens { get; set; }

        public string Currency { get; set; }

        public UserPartialViewModel(string userId)
        {
            if (userId != null)
            {
                var numOfNots = db.Notifications.Where(n => (n.UserId == userId && n.Read == 0)).Count();
                if (numOfNots == 0)
                    HasNot = "";
                else
                    HasNot = "New!";

                Tokens = db.AspNetUsers.Find(userId).Tokens ?? 0 ;

            }

            Currency = db.Parameters.Find("C").Value;

        }
    }

}