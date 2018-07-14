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
using PagedList;

namespace UserTablesPrimer.Controllers
{
    public class AuctionsController : Controller
    {
        private Model1 db = new Model1();

        // GET: Auctions
        public ActionResult Index(String auctionName, float? minPrice, float? maxPrice, int? auctionStatus, string message, bool? error, int? page)
        {
            ViewBag.currentAuctionName = auctionName;
            ViewBag.currentMinPrice = minPrice;
            ViewBag.currentMaxPrice = maxPrice;
            ViewBag.currentAuctionStatus = auctionStatus;

            var auctions = from m in db.Auctions select m;

            if (!String.IsNullOrEmpty(auctionName))
            {
                auctions = auctions.Where(a => a.Name.Contains(auctionName));
            }

            if (minPrice != null)
            {
                auctions = auctions.Where(a => a.CurrentPrice >= minPrice);
            }

            if (maxPrice != null)
            {
                auctions = auctions.Where(a => a.CurrentPrice <= maxPrice);
            }

            if (auctionStatus != null)
            {
                if (auctionStatus != 0)
                {
                    auctions = auctions.Where(a => a.Status == auctionStatus);
                }
            }

            var currency = db.Parameters.Find("C");
            var tokenValue = db.Parameters.Find("T");
            ViewBag.currency = currency.Value;

            foreach (var auction in auctions)
            {
                bool hasBids = db.Bids.Where(a => a.AuctionId == auction.Id).FirstOrDefault() == null ? false : true;

                if (auction.CurrencyId != currency.Value)
                {
                    var k = float.Parse(tokenValue.Value, CultureInfo.InvariantCulture) / auction.Currency.Value;
                    auction.CurrentPrice = (float) Math.Round(auction.CurrentPrice * k, 2);
                }

                if (hasBids)
                {
                    var lastBid = db.Bids.Where(b => b.AuctionId == auction.Id).OrderByDescending(b => b.CreatedOn).First();
                    auction.minBid = lastBid.Bids + 1;
                    auction.LastBidder = db.AspNetUsers.Find(lastBid.UserId).Name + " " + db.AspNetUsers.Find(lastBid.UserId).Surname;
                } else
                {
                    auction.minBid = (int) Math.Ceiling(auction.CurrentPrice / float.Parse(tokenValue.Value, CultureInfo.InvariantCulture));
                }
            }

            ViewBag.message = message;
            ViewBag.error = error;

            ViewBag.tokenValue = tokenValue.Value;

            auctions = auctions.OrderByDescending(a => a.CreateDate);

            int pageSize = Int32.Parse(db.Parameters.Find("N").Value);
            int pageNumber = (page ?? 1);
            return View(auctions.ToPagedList(pageNumber, pageSize));
        }

        [Authorize(Roles = "User")]
        // GET: Auctions/MyAuctions/id
        public ActionResult MyAuctions(string id, string message, bool? error, int? page)
        {
            var auctions = db.Auctions.Where(a => a.WinnerId == id).OrderByDescending(a => a.CloseDate);

            var currency = db.Parameters.Find("C");
            var tokenValue = db.Parameters.Find("T");
            ViewBag.currency = currency.Value;

            foreach (var auction in auctions)
            {
                if (auction.CurrencyId != currency.Value)
                {
                    var k = float.Parse(tokenValue.Value, CultureInfo.InvariantCulture) / auction.Currency.Value;
                    auction.CurrentPrice = (float)Math.Round(auction.CurrentPrice * k, 2);
                }
            }

            ViewBag.message = message;
            ViewBag.error = error;

            int pageSize = Int32.Parse(db.Parameters.Find("N").Value);
            int pageNumber = (page ?? 1);
            return View(auctions.ToPagedList(pageNumber, pageSize));
        }

        // GET: Auctions/Details/5
        public ActionResult Details(string id, string message, bool? error)
        {
            ViewBag.message = message;
            ViewBag.error = error;

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Auction auction = db.Auctions.Find(id);
            if (auction == null)
            {
                return HttpNotFound();
            }

            var currency = db.Parameters.Find("C");
            var tokenValue = db.Parameters.Find("T");
            ViewBag.currency = currency.Value;
            ViewBag.tokenValue = tokenValue.Value;
            
            if (auction.CurrencyId != currency.Value)
            {
                var k = float.Parse(tokenValue.Value, CultureInfo.InvariantCulture) / auction.Currency.Value;
                auction.CurrentPrice = (float)Math.Round(auction.CurrentPrice * k, 2);
            }

            var minBid = db.Bids.Where(b => b.AuctionId == id)
                                   .OrderByDescending(b => b.CreatedOn)
                                   .FirstOrDefault();

            if (minBid != null)
            {
                ViewBag.minBid = minBid.Bids + 1;
            } else
            {
                ViewBag.minBid = Math.Ceiling(auction.CurrentPrice / float.Parse(tokenValue.Value, CultureInfo.InvariantCulture));
            }

            var auctionBids = db.Bids.Where(b => b.AuctionId == auction.Id).OrderByDescending(b => b.CreatedOn);
            auction.Bids = new List<Bid>();

            foreach (var auctionBid in auctionBids)
            {
                Bid newBid = new Bid();
                newBid.UserName = db.AspNetUsers.Find(auctionBid.UserId).Name + " " + db.AspNetUsers.Find(auctionBid.UserId).Surname;
                newBid.Bids = auctionBid.Bids;
                auction.Bids.Add(newBid);
            }

            return View(auction);
        }

        [Authorize(Roles = "User")]
        // GET: Auctions/Create
        public ActionResult Create()
        {
            var currency = db.Parameters.Find("C");
            ViewBag.currency = currency.Value;

            var length = db.Parameters.Find("D");
            ViewBag.length = length.Value;

            return View();
        }

        // POST: Auctions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "User")]
        public ActionResult Create([Bind(Include = "Id,Name,ImageToUpload,Length,StartPrice,CurrentPrice,CreateDate,OpenDate,CloseDate,Status")] Auction auction)
        {
            string msg = "";
            bool err = false;
            if (ModelState.IsValid)
            {
                if (auction.ImageToUpload != null)
                {
                    auction.Img = new byte[auction.ImageToUpload.ContentLength];
                    auction.ImageToUpload.InputStream.Read(auction.Img, 0, auction.Img.Length);
                }

                var currency = db.Parameters.Find("C");
                auction.CurrencyId = currency.Value;

                auction.UserId = User.Identity.GetUserId();
                auction.Id = Guid.NewGuid().ToString();
                auction.CreateDate = DateTime.Now;
                auction.CurrentPrice = auction.StartPrice;
                auction.Status = 1;
                db.Auctions.Add(auction);
                db.SaveChanges();

                msg = "You have successfully created an auction.";
                err = false;

                return RedirectToAction("MyAuctions", "Auctions", new { id = User.Identity.GetUserId(), message = msg, error = err});
            }

            return View(auction);
        }

        // GET: Auctions/Edit/5
        /*public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Auction auction = db.Auctions.Find(id);
            if (auction == null)
            {
                return HttpNotFound();
            }
            return View(auction);
        }*/

        // POST: Auctions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        /*[HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Img,Length,StartPrice,CurrentPrice,CreateDate,OpenDate,CloseDate,Status")] Auction auction)
        {
            if (ModelState.IsValid)
            {
                db.Entry(auction).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(auction);
        }*/

        // GET: Auctions/Delete/5
        /*public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Auction auction = db.Auctions.Find(id);
            if (auction == null)
            {
                return HttpNotFound();
            }
            return View(auction);
        }*/

        // POST: Auctions/Delete/5
        /*[HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Auction auction = db.Auctions.Find(id);
            db.Auctions.Remove(auction);
            db.SaveChanges();
            return RedirectToAction("Index");
        }*/

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Approve(string auctionId)
        {
            string msg = "";
            bool err = false;
            if (auctionId != null)
            {
                var auction = db.Auctions.Find(auctionId);
                if (auction != null)
                {
                    auction.Status = 2;
                    auction.OpenDate = DateTime.Now;
                    auction.CloseDate = DateTime.Now.AddSeconds(auction.Length);
                    db.Entry(auction).State = EntityState.Modified;
                    db.SaveChanges();
                    msg = "You have successfully approved an auction.";
                } else
                {
                    msg = "An error occurred. Please, try again later.";
                    err = true;
                }
            } else
            {
                msg = "An error occurred. Please, try again later.";
                err = true;
            }
            return RedirectToAction("Details", "Auctions", new { id = auctionId, message = msg, error = err});
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
