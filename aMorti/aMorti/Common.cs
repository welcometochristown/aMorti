using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aMorti
{
    public static class Common
    {
        public enum Frequency
        {
            WEEKLY,
            BI_WEEKLY,
            MONTHLY,
            BI_MONTHLY,
            QUARTERLY,
            YEARY,
            BI_YEARLY  
        }

        public static decimal GetMonthlyMultiplier(Frequency frequency)
        {
            switch (frequency)
            {
                case Frequency.WEEKLY: return 0.25m;
                case Frequency.BI_WEEKLY: return 0.5m;
                case Frequency.MONTHLY: return 1.0m;
                case Frequency.BI_MONTHLY: return 2.0m;
                case Frequency.QUARTERLY: return 3.0m;
                case Frequency.YEARY: return 12.0m;
                case Frequency.BI_YEARLY: return 24.0m;
                default: return 0;
            }
        }
    }
}
