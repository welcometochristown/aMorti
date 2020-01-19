using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aMorti
{
    public static class Interest
    {
        public static decimal Calculate(DateTime dtFrom, DateTime dtTo, decimal balance, decimal intRate, int YEARDAYCOUNT)
        {
            int numDaysInRange = Math.Abs((dtTo.Date - dtFrom.Date).Days);
            decimal ir = (intRate / 100);
            decimal dailyIR = (ir / YEARDAYCOUNT);
            decimal dailyInterest = (dailyIR * Math.Max(0, balance));
            decimal interest = dailyInterest * numDaysInRange;
            return interest;
        }
    }
}
