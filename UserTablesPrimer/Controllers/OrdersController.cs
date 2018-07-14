using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using UserTablesPrimer.Models;
using Microsoft.AspNet.Identity;
using PagedList;
using Paymentwall;
using Microsoft.AspNet.SignalR;
using UserTablesPrimer.Hubs;

namespace UserTablesPrimer.Controllers
{
    public class OrdersController : Controller
    {
        private Model1 db = new Model1();

        // GET: Orders
        [System.Web.Mvc.Authorize(Roles = "User")]
        public ActionResult Index(int? page)
        {
            string userId = User.Identity.GetUserId();
            var orders = db.Orders.Where(o => o.UserId == userId).OrderByDescending(o => o.CreatedOn);
            ViewBag.currency = db.Parameters.Find("C").Value;

            foreach (var item in orders)
            {
                if (item.CurrencyId != ViewBag.currency)
                {
                    var k = float.Parse(db.Parameters.Find("T").Value, CultureInfo.InvariantCulture) / item.Currency.Value;
                    item.Price = (float) Math.Round(item.Price * k, 2);
                }
            }

            int pageSize = 10;
            int pageNumber = page ?? 1;

            ViewBag.orderNo = pageNumber * pageSize - 10 + 1;

            return View(orders.ToPagedList(pageNumber, pageSize));
        }

        // GET: Orders/Details/5
        /*public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }*/

        // GET: Orders/Create
        [System.Web.Mvc.Authorize(Roles = "User")]
        public ActionResult Create()
        {
            /*ViewBag.silverTokens = db.Parameters.Find("S").Value;
            ViewBag.goldTokens = db.Parameters.Find("G").Value;
            ViewBag.platinumTokens = db.Parameters.Find("P").Value;

            float tokenPrice = float.Parse(db.Parameters.Find("T").Value, CultureInfo.InvariantCulture);
            var currency = db.Parameters.Find("C").Value;

            ViewBag.silverPrice = tokenPrice * int.Parse(ViewBag.silverTokens);
            ViewBag.goldPrice = tokenPrice * int.Parse(ViewBag.goldTokens);
            ViewBag.platinumPrice = tokenPrice * int.Parse(ViewBag.platinumTokens);

            ViewBag.currency = currency;*/

            using (var db = new Model1())
            {

                /*if (User.Identity.IsAuthenticated)
                {
                    TempData.Remove("Tokens");
                    var id = User.Identity.GetUserId();
                    var result = db.AspNetUsers.Where(u => u.Id == id).First();
                    TempData.Add("Tokens", result.tokens);
                }*/

                Paymentwall_Base.setApiType(Paymentwall_Base.API_VC);
                Paymentwall_Base.setAppKey("b10c3555ee8a2076c998aff76d19b145");
                Paymentwall_Base.setSecretKey("cb2964e08f83150add5d58c3dc93950d");

                var userId = User.Identity.GetUserId();
                var user = db.AspNetUsers.Find(userId);

                Dictionary<string, string> extraParams = new Dictionary<string, string>();
                double unixtime = (TimeZoneInfo.ConvertTimeToUtc(new DateTime(2016, 11, 11, 3, 12, 1, 1, System.DateTimeKind.Utc)) -
                                new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc)).TotalSeconds;
                extraParams.Add("email", user.Email);
                extraParams.Add("registration_date", unixtime.ToString());
                //extraParams.Add("ref", "123");

                Paymentwall_Widget pww = new Paymentwall_Widget(User.Identity.GetUserId(), "p1_1", extraParams);

                ViewBag.widget = pww.getHtmlCode();
                return View();
            }

        }

        // POST: Orders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,UserId,Tokens,Price,Status")] Order order, string packageRadio)
        {
            if (ModelState.IsValid)
            {
                order.Id = Guid.NewGuid().ToString();
                order.UserId = User.Identity.GetUserId();
                if (!String.IsNullOrEmpty(packageRadio))
                {
                    if (packageRadio == "1")
                    {
                        order.Tokens = int.Parse(db.Parameters.Find("S").Value);
                    }
                    if (packageRadio == "2")
                    {
                        order.Tokens = int.Parse(db.Parameters.Find("G").Value);
                    }
                    if (packageRadio == "3")
                    {
                        order.Tokens = int.Parse(db.Parameters.Find("P").Value);
                    }
                }
                order.Price = order.Tokens * float.Parse(db.Parameters.Find("T").Value, CultureInfo.InvariantCulture);
                order.Status = 1;
                order.CurrencyId = db.Parameters.Find("C").Value;
                order.CreatedOn = DateTime.Now;
                db.Orders.Add(order);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.UserId = new SelectList(db.AspNetUsers, "Id", "Email", order.UserId);
            return View(order);
        }

        // GET: Orders/Edit/5
        /*public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserId = new SelectList(db.AspNetUsers, "Id", "Email", order.UserId);
            return View(order);
        }*/

        // POST: Orders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        /*[HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,UserId,Tokens,Price,Status")] Order order)
        {
            if (ModelState.IsValid)
            {
                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.UserId = new SelectList(db.AspNetUsers, "Id", "Email", order.UserId);
            return View(order);
        }*/

        // GET: Orders/Delete/5
        /*public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }*/

        // POST: Orders/Delete/5
        /*[HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Order order = db.Orders.Find(id);
            db.Orders.Remove(order);
            db.SaveChanges();
            return RedirectToAction("Index");
        }*/

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult Pingback()
        {
            lock (Models.lockObjects.dblock)
            {
                //Response.Write("IM IN");
                Paymentwall_Base.setApiType(Paymentwall_Base.API_VC);
                Paymentwall_Base.setAppKey("b10c3555ee8a2076c998aff76d19b145");
                Paymentwall_Base.setSecretKey("cb2964e08f83150add5d58c3dc93950d");

                Paymentwall_Pingback pp = new Paymentwall_Pingback(Request.Params, Request.UserHostAddress);

                //string uid = pp.getUserId();
                //var hub = GlobalHost.ConnectionManager.GetHubContext<AuctionHub>();
                //hub.Clients.All.notifyTokenChange(Request.Params.ToString());

                try
                {

                    if (!pp.validate()) throw new Exception(pp.getErrorSummary());

                    using (var db = new Model1())
                    {
                        string uid = pp.getUserId();
                        if (pp.isDeliverable())
                        {
                            Order o = new Order();
                            o.Id = Guid.NewGuid().ToString();
                            o.UserId = uid;
                            o.Tokens = Int32.Parse(pp.getVirtualCurrencyAmount());
                            o.Status = 2;
                            o.Price = o.Tokens * float.Parse(db.Parameters.Find("T").Value, CultureInfo.InvariantCulture);
                            o.CurrencyId = db.Parameters.Find("C").Value;
                            o.CreatedOn = DateTime.Now;
                            db.Orders.Add(o);

                            var user = db.AspNetUsers.Find(uid);
                            user.Tokens += o.Tokens;
                            db.Entry(user).State = System.Data.Entity.EntityState.Modified;

                            var hub = GlobalHost.ConnectionManager.GetHubContext<SignalRHub>();
                            hub.Clients.User(user.Email).updateTokens(user.Tokens);

                            db.SaveChanges();
                        }
                        if (pp.isCancelable())
                        {
                            Order o = new Order();
                            o.Id = Guid.NewGuid().ToString();
                            o.UserId = uid;
                            o.Tokens = Int32.Parse(pp.getVirtualCurrencyAmount());
                            o.Status = 3;
                            o.Price = o.Tokens * float.Parse(db.Parameters.Find("T").Value, CultureInfo.InvariantCulture);
                            o.CurrencyId = db.Parameters.Find("C").Value;
                            o.CreatedOn = DateTime.Now;
                            db.Orders.Add(o);

                            var user = db.AspNetUsers.Find(uid);
                            /*user.tokens += o.Price;
                            db.Entry(user).State = System.Data.Entity.EntityState.Modified;

                            var hub = GlobalHost.ConnectionManager.GetHubContext<SignalRHub>();
                            hub.Clients.User(user.Email).updateTokens(user.Tokens);*/

                            db.SaveChanges();
                        }

                        return new HttpStatusCodeResult(HttpStatusCode.OK);
                    }
                }
                catch (Exception e)
                {
                    return Content("I had an exception!");
                }
            }

        }
    }
}
