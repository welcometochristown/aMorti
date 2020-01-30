using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aMorti
{
    public class ScheduleParameter
    {
        public enum ParameterType
        {
            //schedule options
            VERSION,
            STARTDATE,

            //repayment params
            REPAYMENT_CAPITALOUTSTANDING,           //any other amount to be repaid that is not going to be included in the payments in this schedule
            REPAYMENT_DATEFIRST,                    //the first repayment date in this schedule
            REPAYMENT_DAYOFMONTH,                   //day to repay, will use last day if the month has less days (e.g 31st will use 30th for April)
            REPAYMENT_FREQUENCY,                    //daily, weekly,bi-weekly, monthly, bi-monthly, yearly, bi-yearly
            REPAYMENT_CAPITAL_HOLIDAY_END,          //defer capital repayments until this date
            REPAYMENT_DAYS_IN_YEAR,                 //defer capital repayments until this date
            REPAYMENT_INTEREST_RATE, 

            //mutally exclusive repayment options. either you specify duration of schedule or the amount you want to pay each rapyment
            REPAYMENT_OPTION_FREQUENCY_INSTANCES,   //how many months is the term of repayment
            REPAYMENT_OPTION_REPAY_VALUE,           //how much to pay every <REPAYMENT_TERM>
            REPAYMENT_OPTION_MATURITY_DATE,          //when does the term end?

            PAYMENT_CAPITAL                         //capital payment (tuple)
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
