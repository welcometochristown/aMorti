using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmortSchedule
{
    public class Schedule : System.ComponentModel.IListSource
    {
        public int Version { get; }

        /// <summary>
        /// Allow a user to define their own interest calculation function.
        /// <param>DateFrom</param>
        /// <param>DateTo</param>
        /// <param>Balance</param>
        /// <param>IntRate</param>
        /// <return>decimal</return>decimal>
        /// </summary>
        public Func<DateTime, DateTime, decimal, decimal, int, decimal> InterestCalcFunc => CalcInterest;

        public DateTime StartDate { get; }

        public decimal InterestRate { get; }

        public List<ScheduleEntry> ScheduleEntries { get; } = new List<ScheduleEntry>();

        public bool ContainsListCollection => false;

        public static IEnumerable<ScheduleParameter.ParameterType> RequiredParams = new[] { ScheduleParameter.ParameterType.VERSION, ScheduleParameter.ParameterType.STARTDATE, ScheduleParameter.ParameterType.INTERESTRATE, ScheduleParameter.ParameterType.PRINCIPLEAMOUNT };

        /// <summary>
        /// Schedule Constructor. Must take start date and interest rate.
        /// </summary>
        /// <param name="startdate"></param>
        /// <param name="interestrate"></param>
        /// <param name="version"></param>
        public Schedule(DateTime startdate, decimal interestrate, int version = 1)
        {
            this.Version = version;
            this.InterestRate = interestrate;
            this.StartDate = startdate;
        }

        /// <summary>
        /// Add a new entry to the schedule
        /// </summary>
        /// <param name="type"></param>
        /// <param name="entrydate"></param>
        /// <returns></returns>
        public ScheduleEntry AddEntry(ScheduleEntry.ScheduleEntryTypeEnum type, DateTime entrydate)
        {
            var entry = new ScheduleEntry(this, type, entrydate);
            ScheduleEntries.Add(entry);
            return entry;
        }

        /// <summary>
        /// Add multiple entries to the schedule
        /// </summary>
        /// <param name="entries"></param>
        /// <returns></returns>
        public IEnumerable<ScheduleEntry> AddEntries(IEnumerable<Tuple<ScheduleEntry.ScheduleEntryTypeEnum, DateTime>> entries)
        {
            foreach (var i in entries)
                yield return AddEntry(i.Item1, i.Item2);
        }

        /// <summary>
        /// generate a schedule
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static Schedule Create(IEnumerable<ScheduleParameter> parameters)
        {
            return CreateAndFill(parameters, null, null);
        }

        /// <summary>
        /// generate a schedule. with payments only
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static Schedule CreateAndFill(IEnumerable<ScheduleParameter> parameters, IEnumerable<SchedulePaymentParameter> paymentParameters)
        {
            return CreateAndFill(parameters, paymentParameters, null);
        }

        /// <summary>
        /// generate a schedule. with repayments only
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static Schedule CreateAndFill(IEnumerable<ScheduleParameter> parameters, IEnumerable<ScheduleRepaymentParameter> repaymentParameters)
        {
            return CreateAndFill(parameters, null, repaymentParameters);
        }

        /// <summary>
        /// generate a schedule with payments and repayments
        /// </summary>
        /// <param name="details"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        public static Schedule CreateAndFill(IEnumerable<ScheduleParameter> parameters, IEnumerable<SchedulePaymentParameter> paymentParameters, IEnumerable<ScheduleRepaymentParameter> repaymentParameters)
        {
            //make sure paramaters were given
            if (parameters == null)
                throw new Exception("Not parameters have been provided");

            //make sure required parameters are given
            if (parameters.Any(n => !RequiredParams.Contains(n.Type)))
                throw new Exception("Not all required parameters have been provided");

            //check for dupes
            if (parameters.GroupBy(n => n.Type).Any(n => n.Count() > 1))
                throw new Exception("Multiple parameters of the same type provided");

            //create schedule
            var schedule = new Schedule(ParseParameter<DateTime>(parameters, ScheduleParameter.ParameterType.STARTDATE),
                                        ParseParameter<decimal>(parameters, ScheduleParameter.ParameterType.INTERESTRATE),
                                        ParseParameter<int>(parameters, ScheduleParameter.ParameterType.VERSION));

            //fil schedule
            schedule.Fill(paymentParameters, repaymentParameters);

            //return the finished schedule
            return schedule;
        }

        /// <summary>
        /// parse parameters from string values
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="prms"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private static T ParseParameter<T>(IEnumerable<ScheduleParameter> prms, ScheduleParameter.ParameterType type)
        {
            var r = prms.SingleOrDefault(n => n.Type == type);

            if(r == null)
                throw new Exception($"Missing {type.ToString()} parameter");

            var converter = TypeDescriptor.GetConverter(typeof(T));

            if (converter != null)
                return (T)converter.ConvertFromString(r.Value);

            throw new Exception($"invalid {nameof(T)} value");
        }


        /// <summary>
        /// fill a schedule with payments
        /// </summary>
        /// <param name="s"></param>
        /// <param name="paymentParameters"></param>
        /// <param name="repaymentParameters"></param>
        public static void Fill(Schedule s, IEnumerable<SchedulePaymentParameter> paymentParameters)
        {
            s.Fill(paymentParameters, null);
        }

        /// <summary>
        /// fill a schedule with payments
        /// </summary>
        /// <param name="s"></param>
        /// <param name="paymentParameters"></param>
        /// <param name="repaymentParameters"></param>
        public void Fill(IEnumerable<SchedulePaymentParameter> paymentParameters)
        {
            this.Fill(paymentParameters, null);
        }

        /// <summary>
        /// fill a schedule with repayments
        /// </summary>
        /// <param name="s"></param>
        /// <param name="repaymentParameters"></param>
        public static void Fill(Schedule s, IEnumerable<ScheduleRepaymentParameter> repaymentParameters)
        {
            s.Fill(null, repaymentParameters);
        }

        /// <summary>
        /// fill a schedule with repayments
        /// </summary>
        /// <param name="s"></param>
        /// <param name="repaymentParameters"></param>
        public void Fill(IEnumerable<ScheduleRepaymentParameter> repaymentParameters)
        {
            this.Fill(null, repaymentParameters);
        }

        /// <summary>
        /// fill a schedule with payment and repayments
        /// </summary>
        /// <param name="s"></param>
        /// <param name="paymentParameters"></param>
        /// <param name="repaymentParameters"></param>
        public static void Fill(Schedule s, IEnumerable<SchedulePaymentParameter> paymentParameters, IEnumerable<ScheduleRepaymentParameter> repaymentParameters)
        {
            s.Fill(paymentParameters, repaymentParameters);
        }

        /// <summary>
        /// fill a schedule with payment and repayments
        /// </summary>
        /// <param name="s"></param>
        /// <param name="paymentParameters"></param>
        /// <param name="repaymentParameters"></param>
        public void Fill(IEnumerable<SchedulePaymentParameter> paymentParameters, IEnumerable<ScheduleRepaymentParameter> repaymentParameters)
        {
            if(paymentParameters != null && paymentParameters.Any())
            {
                //get all payment entry dates
               // IEnumerable<Entry> pEntries = BuildDateTable().Select(n => new Entry { Date = n });

            }

            if (repaymentParameters != null && repaymentParameters.Any())
            {
                //get all repayment entry dates
                IEnumerable<Entry> rEntries = BuildDateTable(1, StartDate, StartDate.AddYears(1), ParseParameter<int>(repaymentParameters, ScheduleParameter.ParameterType.REPAYMENT_DAYOFMONTH)).Select(n => new Entry { Date = n }).ToList();

                rEntries = GenerateRepayments(ParseParameter<DateTime>(repaymentParameters, ScheduleParameter.ParameterType.REPAYMENT_DATEFIRST),
                    rEntries, 1,
                    ParseParameter<decimal>(repaymentParameters, ScheduleParameter.ParameterType.REPAYMENT_CAPITALOUTSTANDING),
                    ParseParameter<decimal>(repaymentParameters, ScheduleParameter.ParameterType.REPAYMENT_INTERESTOUTSTANDING));

                foreach(var e in rEntries)
                {
                    var entry = new ScheduleEntry(this, ScheduleEntry.ScheduleEntryTypeEnum.Repay, e.Date);
                    entry.AddScheduleEntryTransaction(ScheduleEntryTransaction.TransactionType.Capital, e.ValueCapital);
                    entry.AddScheduleEntryTransaction(ScheduleEntryTransaction.TransactionType.Capital, e.ValueInterest);
                    ScheduleEntries.Add(entry);
                }
            }





            ////first thing we need to do is to work out a list of all the entries (this is both payment and repayment entries)


            //var entries = GenerateDateRepayments(ParseParameter<DateTime>(repaymentParameters, ScheduleParameter.ParameterType.REPAYMENT_DATEFIRST),
            //    null,
            //    0,
            //    ParseParameter<decimal>(repaymentParameters, ScheduleParameter.ParameterType.REPAYMENT_CAPITALOUTSTANDING),
            //    InterestRate);

            ////TODO: Fill here
            //REPAYMENT_CAPITALOUTSTANDING,    //any other amount to be repaid that is not going to be included in the payments in this schedule
            //REPAYMENT_DATEFIRST,             //the first repayment date in this schedule
            //REPAYMENT_DAYOFMONTH,            //day to repay, will use last day if the month has less days (e.g 31st will use 30th for April)
            //REPAYMENT_DAYENDOFMONTH,         //true or false whether to use last day of the month always (overrides [REPAYMENTDAYOFMONTH])
            //REPAYMENT_DURATION               //daily, weekly,bi-weekly, monthly, bi-monthly, yearly, bi-yearly
        }

        private class Entry
        {
            public DateTime Date { get; set; }
            public decimal ValueCapital { get; set; }
            public decimal ValueInterest { get; set; }
        }

        private IEnumerable<DateTime> BuildDateTable(int period, DateTime start, DateTime end, int day, bool useStartAsFirst = true)
        {
            if (day < 1)
                throw new Exception("Repayment day can't be less than 1");

            if (day > 31)
                throw new Exception("Repayment day can't be more than 31");

            List<DateTime> list = new List<DateTime>();
            DateTime dtLast = start;

            while (true)
            {
                DateTime? dt = null;

                //do we use the start date as the first entry, are we on the first entry?
                if (useStartAsFirst && !list.Any())
                    dt = start;
                else
                    dt = NextDateUsingFrequency(period, dtLast, day);

                //if we have gone past the end date then edn
                if (dt.Value.Date > end)
                    break;

                //add day to list
                list.Add(dt.Value.Date);

                //this is now the current date
                dtLast = dt.Value;

            }

            return list;
        }

        private DateTime NextDateUsingFrequency(int period, DateTime last, int day)
        {
            ///TODO:Update this
            var n = last.AddMonths(period);
            return new DateTime(n.Year, n.Month, day);
        }

        /// <summary>
        /// Returns each repayment date, type of repayment and value
        /// </summary>
        /// param name="dateStart" Start repayments from this date forward.
        /// param name="capitalBalance"
        /// param name="interestBalance"
        /// param name="monthlyMultiplier"
        /// param name="repaymentdates"
        /// <returns></returns>
        private IEnumerable<Entry> GenerateRepayments(DateTime dateStart, IEnumerable<Entry> entries, decimal monthlyMultiplier, decimal capitalBalance, decimal interestBalance)
        {
            //how many do we have?
            int instances = entries.Count();

            //if there are none just return them back
            if (instances == 0)
                return entries;

            //this is the ideal repayment across all schedule repayment entries
            decimal ideal_total_repayment = 0;

            if (monthlyMultiplier == 0)
                throw new InvalidOperationException("No monthly multiplier value supplied, unable to calculate ideal repayment value");

            /*  some maths. See this mathexchange post for our understanding of this: https://math.stackexchange.com/questions/2521313/mathmatical-formula-possible/2521351?noredirect=1#comment5206959_2521351
                More specifically, this uses a geometric series to calculate the initial starting point. This is much more efficient when compared with the old trial and error approach!
                It means that the tweaking that follows to get the exact value is much more accurate. 
            */

            if (this.InterestRate == 0)
                throw new InvalidOperationException("interest rate can't be 0");

            decimal yearly_month_ratio = 1200 / monthlyMultiplier; //Although we use a daily interest calculation, when scheduling we assume that things are a little more fixed. Here we use a monthly interest calculation, and need to use a ratio to deal with none monthly loans.

            decimal rate_of_interest = (1 + InterestRate / yearly_month_ratio); //using the above, we calculate the proper interest rate. note this is not the actual rate that will be used (because we use daily), but this gets us REAL close.
            decimal rate_of_interest_pow_months = Convert.ToDecimal(Math.Pow(Convert.ToDouble(rate_of_interest), Convert.ToDouble(instances))); //To get a provisional rate for our period, we take the rate above then raise it to the power of the number of specific repayment inctances (e.g. 12 months monthly would be raised to the power of 12) 

            decimal x = capitalBalance * rate_of_interest * ((1 / rate_of_interest) - 1); //the next three lines are our implementation of the geometric series described in the maths link above.
            decimal y = ((1 / rate_of_interest_pow_months) - 1);
            decimal result = x / y; //this result is effectively an ideal repayment value covering capital and interest but it will not work in reality. Why? Because effectively we are assuming fixed month lengths (e.g. every month is 30 days).

            ideal_total_repayment = result;
       

            decimal totalRepaymentsValue = 0;
            DateTime dtLast = dateStart.Date;
            int attemptCounter = 0;

            /*Thanks to the above calculation we are likely really close to the final value. We do need to adjust a little bit to make sure we are accounting for things like Feb being a short month. We are going to need to have a day levle calc anyway, because customers don't always repay on time.
              In our  tests we have seen that often you only need to enter this loop once. The maths is accurate enough that we sometimes just put any overage on the last payment as its tiny anyway. If its bigger than £1 we iterate.
              Thats actually super conservative, we could easily make that something like 50% of the normal repayment and customers will be quite happy with that. £1 works fine though, so who cares.
             */
            while (totalRepaymentsValue != capitalBalance) //so we loop while we haven't fully paid off the loan, adjusting things until we have and have a fixed value.
            {
                //keep track of how many attempts we make
                attemptCounter++;

                //make sure the ideal is a rounded amount
                ideal_total_repayment = Math.Round(ideal_total_repayment, 2, MidpointRounding.ToEven);

               // log.Info($"Attempting repayment set with {ideal_total_repayment} as ideal repayment value");

                //need to keep track of the last date for generating interest
                dtLast = dateStart.Date;

                decimal interestDue = interestBalance;

                //log.Info($"Iterating over each repayment entry...");

                //iterate over each schedule entry instance 
                foreach (var entry in entries)
                {
                    //log.Info($"Creating entry for {entry.Date.ToString("yyyy-MM-dd")}");

                    //how many repayments have we got already before this date
                    decimal previousCapitalRepayments = entries
                                                           .Where(n => n.Date < entry.Date)
                                                           .Sum(n => Math.Abs(n.ValueCapital));

                    //get balance at this point given any repayments made
                    decimal currentBalance = capitalBalance - previousCapitalRepayments;

                    //get interest from the core microservice.
                    decimal interestAccrued = interestDue;

                    //when should the interest accrual end?
                    var interest_accrual_end = entry.Date.AddDays(-1);

                    if (dtLast != entry.Date)
                    {
                        interestAccrued += InterestCalcFunc(dtLast, interest_accrual_end, currentBalance, InterestRate, 364);
                    }

                    interestDue = 0;
                    entry.ValueInterest = Math.Round(interestAccrued, 2, MidpointRounding.ToEven);

                    decimal capitalValue = 0;

                    if (entry.ValueInterest > ideal_total_repayment)
                        capitalValue = 0;
                    else if (!entries.Any(n => n.Date > entry.Date))
                        capitalValue = currentBalance <= ideal_total_repayment ? currentBalance : ideal_total_repayment;
                    else
                        capitalValue = ideal_total_repayment - entry.ValueInterest;

                    entry.ValueCapital = Math.Round(capitalValue, 2);

                    //  log.Info($"Entry has {entry.ValueCapital} as capital and {entry.ValueInterest} as interest");

                    dtLast = entry.Date;

                }

                //how much of the loan are we currently paying back?
                totalRepaymentsValue = entries.Sum(n => Math.Abs(n.ValueCapital));

                //how much difference between the principle amount and the total we have got now?
                decimal difference = capitalBalance - totalRepaymentsValue;

               // log.Info($"Total repayments value comes to {totalRepaymentsValue}, leaving a difference of {difference}");


                //if we are less than 1 then just add it to the last repayment
                if (Math.Abs(difference) < 1)
                {
                    //add the difference to the last repayment
                    var last = entries.Last();

                    var newCapitalValue = -Math.Round(Math.Abs(last.ValueCapital) + difference, 2);

                   // log.Info($"Updating last repayment capital value from {last.ValueInterest} to {newCapitalValue}");

                    last.ValueCapital = newCapitalValue;

                    //recalculate total repayments value this should now be the loan amount
                    totalRepaymentsValue = entries.Sum(n => Math.Abs(n.ValueCapital));

                    if (totalRepaymentsValue != capitalBalance)
                        throw new Exception("Failure to complete schedule, the total repayment amount generated did not equal the capital balance, this should never happen.");

                    //should be good to exit the loop
                    break;
                }

                //log.Info($"Starting new repayment generation set...");

                //change the ideal total repayment
                ideal_total_repayment += (difference / instances);

            }

           // log.Info($"Repayment entry generation took {attemptCounter} attempts");
            return entries;
        }


        private decimal CalcInterest(DateTime dtFrom, DateTime dtTo, decimal balance, decimal intRate, int YEARDAYCOUNT)
        {
            var iRate = intRate / 100;
            var span = dtTo.Date.Subtract(dtFrom.Date);
            var days = Math.Abs(span.Days);
            var i = (balance * iRate) / YEARDAYCOUNT;
            var ir = i * days;
            return ir;
        }


        /// <summary>
        /// Total amount outstanding on this schedule.
        /// </summary>
        /// <returns></returns>
        public decimal Outstanding()
        {
            return ScheduleEntries.Sum(n => n.OutstandingTotal);
        }

        /// <summary>
        /// Total amount outstanding on this schedule for this transaction type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public decimal Outstanding(ScheduleEntryTransaction.TransactionType type)
        {
            return ScheduleEntries.Sum(n => n.Outstanding(type));
        }

        /// <summary>
        /// The datasource list
        /// </summary>
        /// <returns></returns>
        public IList GetList()
        {
            return ScheduleEntries;
        }
    }
}
