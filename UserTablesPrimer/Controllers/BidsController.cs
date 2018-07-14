using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using UserTablesPrimer.Models;
using System.Globalization;
using System.Data.Entity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.AspNet.SignalR;
using UserTablesPrimer.Hubs;
using System.Web.Script.Serialization;
using Newtonsoft.Json;

namespace UserTablesPrimer.Controllers
{
    public class BidsController : Controller
    {
        private Model1 db = new Model1();
        // GET: Bids
        public ActionResult Index()
        {
            return View();
        }

        private bool isValidNumber(string StringToCheck)
        {
            int val = 0;
            Int32.TryParse(StringToCheck, out val);
            if (val > 0)
                return true;
            return false;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [System.Web.Mvc.Authorize(Roles = "User")]
        public ActionResult Bid(string numberOfTokens, string AuctionId)
        {
            lock (Models.lockObjects.dblock)
            {
                bool err = false;
                string msg = "";
                if (isValidNumber(numberOfTokens))
                {
                    var auction = db.Auctions.Find(AuctionId);
                    if (auction != null)
                    {
                        if (auction.Status != 2)
                        {
                            err = true;
                            msg = "You can't bid at an auction that is not opened.";
                        }
                        else
                        {
                            if (auction.UserId == User.Identity.GetUserId())
                            {
                                err = true;
                                msg = "You can't bid at your own auction.";
                            }
                            else
                            {
                                int numOfTokens = Int32.Parse(numberOfTokens);
                                var userId = User.Identity.GetUserId();
                                if (numOfTokens <= 0 || numOfTokens > db.AspNetUsers.Find(userId).Tokens)
                                {
                                    err = true;
                                    msg = "You don't have enough tokens. Please, try again later.";
                                }
                                else
                                {
                                    float tokenValue = float.Parse(db.Parameters.Find("T").Value, CultureInfo.InvariantCulture);
                                    var currency = db.Parameters.Find("C").Value;

                                    float currentPrice = auction.CurrentPrice;

                                    if (currency != auction.CurrencyId)
                                    {
                                        var k = tokenValue / auction.Currency.Value;
                                        currentPrice = (float)Math.Round(currentPrice * k, 2);
                                    }

                                    float bidPrice = Int32.Parse(numberOfTokens) * tokenValue;

                                    if (currentPrice > bidPrice)
                                    {
                                        err = true;
                                        msg = "You can't bid less than minimum amount.";
                                    }
                                    else
                                    {
                                        if (currentPrice == bidPrice && db.Bids.Where(a => a.AuctionId == AuctionId).FirstOrDefault() != null)
                                        {
                                            err = true;
                                            msg = "You can't bid less than minimum amount.";
                                        }
                                        else
                                        {
                                            var latestBid = db.Bids.Where(b => b.AuctionId == AuctionId).OrderByDescending(b => b.CreatedOn).FirstOrDefault();
                                            string latestBidderId = latestBid == null ? "" : latestBid.UserId;
                                            if (latestBidderId == User.Identity.GetUserId())
                                            {
                                                err = true;
                                                msg = "You already bid on this auction.";
                                            }
                                            else
                                            {
                                                var context = GlobalHost.ConnectionManager.GetHubContext<SignalRHub>();
                                                var latestBidder = db.AspNetUsers.Find(latestBidderId);
                                                if (latestBidder != null)
                                                {
                                                    latestBidder.Tokens = latestBidder.Tokens + latestBid.Bids;
                                                    db.Entry(latestBidder).State = EntityState.Modified;
                                                    context.Clients.User(latestBidder.Email).updateTokens(latestBidder.Tokens);
                                                }

                                                Bid bid = new Bid();
                                                bid.Id = Guid.NewGuid().ToString();
                                                bid.AuctionId = AuctionId;
                                                bid.UserId = User.Identity.GetUserId();
                                                bid.Bids = numOfTokens;
                                                bid.CreatedOn = DateTime.Now;
                                                db.Bids.Add(bid);

                                                var newPrice = auction.CurrentPrice;
                                                newPrice = numOfTokens * auction.Currency.Value;
                                                auction.CurrentPrice = newPrice;
                                                db.Entry(auction).State = EntityState.Modified;

                                                msg = "You have successfully bid at an auction.";
                                                err = false;

                                                Auction auct = new Auction();
                                                auct.Id = auction.Id;

                                                if (auction.CurrencyId == currency)
                                                {
                                                    auct.CurrentPrice = auction.CurrentPrice;
                                                }
                                                else
                                                {
                                                    var k = tokenValue / auction.Currency.Value;
                                                    auct.CurrentPrice = (float)Math.Round(auction.CurrentPrice * k, 2);
                                                }

                                                auct.Bids = new List<Bid>();

                                                var auctionBids = db.Bids.Where(a => a.AuctionId == auct.Id).OrderByDescending(a => a.CreatedOn);

                                                //var userId = User.Identity.GetUserId();
                                                var user = db.AspNetUsers.Find(userId);
                                                user.Tokens = user.Tokens - numOfTokens;
                                                db.Entry(user).State = EntityState.Modified;

                                                var followers = auctionBids.Where(a => a.UserId != user.Id).GroupBy(a => a.UserId).Select(a => a.FirstOrDefault());

                                                foreach (var follower in followers)
                                                {
                                                    Notification notification = new Notification();
                                                    notification.Id = Guid.NewGuid().ToString();
                                                    notification.Message = "User " + user.Name + " " + user.Surname + " has bid on auction you follow: " + auction.Name;
                                                    notification.Read = 0;
                                                    notification.UserId = follower.UserId;
                                                    notification.CreatedOn = DateTime.Now;
                                                    db.Notifications.Add(notification);

                                                    context.Clients.User(db.AspNetUsers.Find(follower.UserId).Email).showMessage();
                                                }

                                                db.SaveChanges();

                                                auct.LastBidder = user.Name + " " + user.Surname;

                                                foreach (var auctionBid in auctionBids)
                                                {
                                                    Bid newBid = new Bid();
                                                    newBid.UserName = db.AspNetUsers.Find(auctionBid.UserId).Name + " " + db.AspNetUsers.Find(auctionBid.UserId).Surname;
                                                    newBid.Bids = auctionBid.Bids;
                                                    auct.Bids.Add(newBid);
                                                }

                                                context.Clients.All.updateBidListAndPrice(JsonConvert.SerializeObject(auct), currency, tokenValue);
                                                context.Clients.All.updateAuctionPrice(JsonConvert.SerializeObject(auct), currency, tokenValue);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        err = true;
                        msg = "Invalid input. Please, try again later.";
                    }
                }
                else
                {
                    err = true;
                    msg = "Invalid input. Please, try again later.";
                }
                return RedirectToAction("Details", "Auctions", new { id = AuctionId, message = msg, error = err });
            }
        }
    }
}