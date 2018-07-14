using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentScheduler;

namespace UserTablesPrimer.Tasks
{
    public class Timer : Registry
    {
        public Timer()
        {
            Schedule<AuctionTimeUpdate>().ToRunNow().AndEvery(1).Seconds();
        }
    }
}