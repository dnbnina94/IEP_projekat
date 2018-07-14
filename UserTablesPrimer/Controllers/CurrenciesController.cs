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
    public class CurrenciesController : Controller
    {
        private Model1 db = new Model1();
        // GET: Currencies
        public ActionResult Index(string msg, bool? error, string errors)
        {
            ViewBag.msg = msg;
            ViewBag.error = error;
            ViewBag.errors = errors;
            return View(db.Currencies);
        }

        private bool isValidNumber(string StringToCheck)
        {
            float val = 0;
            float.TryParse(StringToCheck, NumberStyles.Any, CultureInfo.InvariantCulture, out val);
            if (val > 0)
                return true;
            return false;
        }

        [Authorize(Roles = "Admin")]
        public ActionResult UpdateCurrencies(string RSD, string EUR, string USD)
        {

            List<string> errors = new List<string>();
            if (!isValidNumber(RSD))
                errors.Add("Token value for RSD currency must be a positive number greater than 0.");
            if (!isValidNumber(USD))
                errors.Add("Token value for USD currency must be a positive number greater than 0.");
            if (!isValidNumber(EUR))
                errors.Add("Token value for EUR currency must be a positive number greater than 0.");

            if (!errors.Any())
            {

                float RSDFloat = float.Parse(RSD, CultureInfo.InvariantCulture);
                float USDFloat = float.Parse(USD, CultureInfo.InvariantCulture);
                float EURFloat = float.Parse(EUR, CultureInfo.InvariantCulture);

                var currencyRSD = db.Currencies.Find("RSD");
                if (currencyRSD.Value != RSDFloat)
                {
                    if (db.Parameters.Find("C").Value == "RSD")
                    {
                        var tokenValue = new Parameters();
                        tokenValue.Name = "T";
                        tokenValue.Value = RSD.ToString();
                        db.Entry(tokenValue).State = EntityState.Modified;
                    }
                    currencyRSD.Value = RSDFloat;
                    db.Entry(currencyRSD).State = EntityState.Modified;
                }

                var currencyUSD = db.Currencies.Find("USD");
                if (currencyUSD.Value != USDFloat)
                {
                    if (db.Parameters.Find("C").Value == "USD")
                    {
                        var tokenValue = new Parameters();
                        tokenValue.Name = "T";
                        tokenValue.Value = USD.ToString();
                        db.Entry(tokenValue).State = EntityState.Modified;
                    }
                    currencyUSD.Value = USDFloat;
                    db.Entry(currencyUSD).State = EntityState.Modified;
                }

                var currencyEUR = db.Currencies.Find("EUR");
                if (currencyEUR.Value != EURFloat)
                {
                    if (db.Parameters.Find("C").Value == "EUR")
                    {
                        var tokenValue = new Parameters();
                        tokenValue.Name = "T";
                        tokenValue.Value = EUR.ToString();
                        db.Entry(tokenValue).State = EntityState.Modified;
                    }
                    currencyEUR.Value = EURFloat;
                    db.Entry(currencyEUR).State = EntityState.Modified;
                }

                db.SaveChanges();

            }
            string errorsString = String.Join(",", errors.ToArray());
            return RedirectToAction("Index", "Currencies", new { msg = !errors.Any() ? "You have successfully updated currencies." : "", error = !errors.Any() ? false : true, errors = errorsString });
        }
    }
}