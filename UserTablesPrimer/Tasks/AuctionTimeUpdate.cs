using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentScheduler;
using UserTablesPrimer.Models;
using System.Data.Entity;
using Microsoft.AspNet.SignalR;
using UserTablesPrimer.Hubs;
using System.Web.Script.Serialization;
using System.Globalization;

namespace UserTablesPrimer.Tasks
{
    public class AuctionTimeUpdate : IJob
    {
        public void Execute()
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<SignalRHub>();
            var db = new Model1();
            var auctions = db.Auctions.Where(a => a.Status == 2);
            var time = DateTime.Now;

            List<Auction> auctionList = new List<Auction>();

            foreach (var auction in auctions)
            {
                var auct = new Auction();
                auct.Id = auction.Id;
                auct.CloseDate = auction.CloseDate;
                auct.Length = auction.Length;
                DateTime closeDate = auct.CloseDate ?? DateTime.Now;
                TimeSpan timeSpan = closeDate.Subtract(DateTime.Now);
                if (timeSpan.TotalSeconds < 0)
                {
                    timeSpan = new TimeSpan(0, 0, 0, 0);
                }

                auct.timeLeft = (timeSpan.Days / 10 == 0 ? "0" : "")
                        + timeSpan.Days + ":"
                        + (timeSpan.Hours / 10 == 0 ? "0" : "")
                        + timeSpan.Hours + ":"
                        + (timeSpan.Minutes / 10 == 0 ? "0" : "")
                        + timeSpan.Minutes + ":"
                        + (timeSpan.Seconds / 10 == 0 ? "0" : "")
                        + timeSpan.Seconds;

                if (auction.CloseDate <= time)
                {
                    auction.Status = 3;

                    var latestBid = db.Bids.Where(a => a.AuctionId == auction.Id).OrderByDescending(a => a.CreatedOn).FirstOrDefault();

                    if (latestBid != null)
                    {
                        auction.WinnerId = latestBid.UserId;
                    }

                    db.Entry(auction).State = EntityState.Modified;

                }

                auct.Status = auction.Status;
                auctionList.Add(auct);
            }

            db.SaveChanges();

            var jsonSerialiser = new JavaScriptSerializer();
            var json = jsonSerialiser.Serialize(auctionList);

            context.Clients.All.updateAuctionTime(json);
            context.Clients.All.updateAuctionDetails(json);
            //throw new NotImplementedException();
        }
    }
}