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
    public class ParametersController : Controller
    {
        private Model1 db = new Model1();
        // GET: Parameters
        [Authorize(Roles = "Admin")]
        public ActionResult Index(string msg, bool? error, string errors)
        {
            ViewBag.errors = errors;
            ViewBag.error = error;
            ViewBag.msg = msg;
            ViewBag.currencies = db.Currencies;
            return View(db.Parameters);
        }

        private bool isValidNumber(string StringToCheck)
        {
            int val = 0;
            Int32.TryParse(StringToCheck, out val);
            if (val > 0)
                return true;
            return false;
        }

        [Authorize(Roles = "Admin")]
        public ActionResult ChangeParameters(string defaultCurrency, string defaultAuctionLength,
            string silverPackageTokens, string goldPackageTokens, string platinumPackageTokens, string defaultNumOfAuctions)
        {

            List<string> errors = new List<string>();

            if (db.Currencies.Find(defaultCurrency) == null)
                errors.Add("Invalid currency.");

            if (!isValidNumber(defaultAuctionLength))
                errors.Add("Auction length must be a positive integer number greater than 0.");
            if (!isValidNumber(silverPackageTokens))
                errors.Add("No. of tokens for 'Silver' package must be a positive integer number greater than 0.");
            if (!isValidNumber(goldPackageTokens))
                errors.Add("No. of tokens for 'Gold' package must be a positive integer number greater than 0.");
            if (!isValidNumber(platinumPackageTokens))
                errors.Add("No. of tokens for 'Platinum' package must be a positive integer number greater than 0.");
            if (!isValidNumber(defaultNumOfAuctions))
                errors.Add("Default No. of Auctions must be a positive integer number greater than 0.");

            if (!errors.Any())
            {

                var currency = db.Parameters.Find("C");
                if (currency.Value != defaultCurrency)
                {
                    currency.Value = defaultCurrency;
                    db.Entry(currency).State = EntityState.Modified;

                    float tokenValue = db.Currencies.Find(currency.Value).Value;
                    var tokenParameter = db.Parameters.Find("T");
                    tokenParameter.Value = tokenValue.ToString();
                }

                var auctionLength = db.Parameters.Find("D");
                if (auctionLength.Value != defaultAuctionLength)
                {
                    auctionLength.Value = defaultAuctionLength;
                    db.Entry(auctionLength).State = EntityState.Modified;
                }

                var silverTokens = db.Parameters.Find("S");
                if (silverTokens.Value != silverPackageTokens)
                {
                    silverTokens.Value = silverPackageTokens;
                    db.Entry(silverTokens).State = EntityState.Modified;
                }

                var goldTokens = db.Parameters.Find("G");
                if (goldTokens.Value != goldPackageTokens)
                {
                    goldTokens.Value = goldPackageTokens;
                    db.Entry(goldTokens).State = EntityState.Modified;
                }

                var platinumTokens = db.Parameters.Find("P");
                if (platinumTokens.Value != platinumPackageTokens)
                {
                    platinumTokens.Value = platinumPackageTokens;
                    db.Entry(platinumTokens).State = EntityState.Modified;
                }

                var numOfAuctions = db.Parameters.Find("N");
                if (numOfAuctions.Value != defaultNumOfAuctions)
                {
                    numOfAuctions.Value = defaultNumOfAuctions;
                    db.Entry(numOfAuctions).State = EntityState.Modified;
                }

                db.SaveChanges();

            }

            string errorsString = String.Join(",", errors.ToArray());
            return RedirectToAction("Index", "Parameters", new { msg = !errors.Any() ? "You have successfully changed parameters." : "", error = !errors.Any() ? false : true, errors = errorsString });
        }
    }
}