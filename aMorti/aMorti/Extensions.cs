using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static aMorti.Common;

namespace aMorti
{
    public static class Extensions
    {
        public static DateTime AddMonths(this DateTime dt, int months, int dayofmonth)
        {
            var n = new DateTime(dt.Year, dt.Month, 1).AddMonths(months);
            return new DateTime(n.Year, n.Month, Math.Min(dayofmonth, DateTime.DaysInMonth(n.Year, n.Month)));
        }

        public static DateTime AddFrequency(this DateTime dt, int increments, Frequency frequency)
        {
            switch (frequency)
            {
                case Frequency.WEEKLY: return dt.AddDays(7* increments);
                case Frequency.BI_WEEKLY: return dt.AddDays(14 * increments);
                case Frequency.MONTHLY: return dt.AddMonths(1 * increments);
                case Frequency.BI_MONTHLY: return dt.AddMonths(2 * increments);
                case Frequency.QUARTERLY: return dt.AddMonths(3 * increments);
                case Frequency.YEARY: return dt.AddYears(1 * increments);
                case Frequency.BI_YEARLY: return dt.AddYears(2 * increments);
                default: throw new Exception($"Invalid Frequency - {frequency.ToString()}");
            }
        }

        public static DateTime AddFrequency(this DateTime dt, Frequency frequency, int dayofmonth)
        {
            switch(frequency)
            {
                case Frequency.WEEKLY:return dt.AddDays(7) ;
                case Frequency.BI_WEEKLY: return dt.AddDays(14);
                case Frequency.MONTHLY: return dt.AddMonths(1, dayofmonth);
                case Frequency.BI_MONTHLY: return dt.AddMonths(2, dayofmonth);
                case Frequency.QUARTERLY: return dt.AddMonths(3, dayofmonth);
                case Frequency.YEARY: return dt.AddYears(1);
                case Frequency.BI_YEARLY: return dt.AddYears(2);
               default: throw new Exception($"Invalid Frequency - {frequency.ToString()}");
            }
        
        }
    }
}
