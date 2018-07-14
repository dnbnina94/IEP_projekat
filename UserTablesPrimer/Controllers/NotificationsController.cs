using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using UserTablesPrimer.Models;
using Microsoft.AspNet.Identity;
using System.Globalization;

namespace UserTablesPrimer.Controllers
{
    public class NotificationsController : Controller
    {
        private Model1 db = new Model1();
        // GET: Notifications
        [Authorize(Roles = "User")]
        public ActionResult Index()
        {
            string userId = User.Identity.GetUserId();
            var notifications = db.Notifications.Where(n => n.UserId == userId).OrderByDescending(n => n.CreatedOn);

            List<Notification> nots = new List<Notification>();

            foreach (var not in notifications)
            {
                Notification newNot = new Notification();
                newNot.Message = not.Message;
                nots.Add(newNot);
            }

            db.Notifications.RemoveRange(notifications);
            db.SaveChanges();

            return View(nots);
        }
    }
}