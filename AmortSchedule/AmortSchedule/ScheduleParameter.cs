using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmortSchedule
{
    public class ScheduleParameter
    {
        public enum ParameterType
        {
            VERSION,
            STARTDATE,
            INTERESTRATE,
            PRINCIPLEAMOUNT,

            //repayment params
            REPAYMENT_CAPITALOUTSTANDING,    //any other amount to be repaid that is not going to be included in the payments in this schedule
            REPAYMENT_INTERESTOUTSTANDING,    //any other amount to be repaid that is not going to be included in the payments in this schedule
            REPAYMENT_DATEFIRST,             //the first repayment date in this schedule
            REPAYMENT_DAYOFMONTH,            //day to repay, will use last day if the month has less days (e.g 31st will use 30th for April)
            REPAYMENT_DAYENDOFMONTH,         //true or false whether to use last day of the month always (overrides [REPAYMENTDAYOFMONTH])
            REPAYMENT_DURATION               //daily, weekly,bi-weekly, monthly, bi-monthly, yearly, bi-yearly
        }

        public ScheduleParameter(ParameterType type, string value)
        {
            this.Type = type;
            this.Value = value;
        }

        public ParameterType Type { get; }

        public string Value { get; }
    }
}
