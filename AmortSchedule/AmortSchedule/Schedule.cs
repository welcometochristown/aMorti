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

                rEntries = rEntries.Skip(1).Take(5).ToList();

                rEntries = GenerateRepayments(ParseParameter<DateTime>(repaymentParameters, ScheduleParameter.ParameterType.REPAYMENT_DATEFIRST),
                    rEntries, 1,
                    ParseParameter<decimal>(repaymentParameters, ScheduleParameter.ParameterType.REPAYMENT_CAPITALOUTSTANDING),
                    ParseParameter<decimal>(repaymentParameters, ScheduleParameter.ParameterType.REPAYMENT_INTERESTOUTSTANDING));

                foreach(var e in rEntries)
                {
                    var entry = new ScheduleEntry(this, ScheduleEntry.ScheduleEntryTypeEnum.Repay, e.Date);
                    entry.AddScheduleEntryTransaction(ScheduleEntryTransaction.TransactionType.Capital, e.ValueCapital);
                    entry.AddScheduleEntryTransaction(ScheduleEntryTransaction.TransactionType.Interest, e.ValueInterest);
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

            if (InterestRate == 0)
            {
                //no interest just use the capital balance
                ideal_total_repayment = Math.Round(capitalBalance * instances);
            }
            else
            {
                //geometric series
                //https://math.stackexchange.com/questions/2521313/mathmatical-formula-possible/2521351?noredirect=1#comment5206959_2521351
                decimal ir = (1.0m + (InterestRate / 100.0m));
                decimal x = capitalBalance * ir * ((1.0m / ir) - 1.0m);
                decimal y = (1.0m / Convert.ToDecimal(Math.Pow(Convert.ToDouble(ir), Convert.ToDouble(instances)))) - 1.0m;
                ideal_total_repayment = x / y;
            }

            if (ideal_total_repayment == 0)
                throw new Exception("Ideal Repyment Cant Be Zero");

            //track the last date
            DateTime dtLast = dateStart.Date;

            //make sure the ideal is a rounded amount
            ideal_total_repayment = Math.Round(ideal_total_repayment, 2, MidpointRounding.ToEven);

            //need to keep track of the last date for generating interest
            dtLast = dateStart.Date;

            decimal interestDue = interestBalance;

            //iterate over each schedule entry instance 
            foreach (var entry in entries)
            {
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

                //only days after the first entry will accrue interest
                if (dtLast != entry.Date)
                    interestAccrued += InterestCalcFunc(dtLast, interest_accrual_end, currentBalance, InterestRate, 364);

                //rounded interest
                interestDue = 0;
                entry.ValueInterest = Math.Round(interestAccrued, 2, MidpointRounding.ToEven);

                //get the capital depending on how close to the ideal repayment we are
                decimal capitalValue = 0;

                if (entry.ValueInterest > ideal_total_repayment)
                    capitalValue = 0;
                else if (!entries.Any(n => n.Date > entry.Date))
                    capitalValue = currentBalance <= ideal_total_repayment ? currentBalance : ideal_total_repayment;
                else
                    capitalValue = ideal_total_repayment - entry.ValueInterest;

                entry.ValueCapital = Math.Round(capitalValue, 2);

                dtLast = entry.Date;
            }

            //how much of the loan are we currently paying back?
            decimal totalRepaymentsValue = entries.Sum(n => Math.Abs(n.ValueCapital));

            //how much difference between the principle amount and the total we have got now?
            decimal difference = capitalBalance - totalRepaymentsValue;

            //add the difference to the last repayment
            if (difference != 0)
                 entries.Last().ValueCapital += difference;

            return entries;
        }


        private decimal CalcInterest(DateTime dtFrom, DateTime dtTo, decimal balance, decimal intRate, int YEARDAYCOUNT)
        {
            // return 0;
            //var iRate = intRate / 100;

            var numDaysInRange = Math.Abs((dtTo.Date - dtFrom.Date).Days);
            decimal dailyIR = (intRate / 100) / YEARDAYCOUNT;
            decimal dailyInterest = dailyIR * Math.Max(0, balance);
            decimal interest = dailyInterest * numDaysInRange;

            numDaysInRange += 1;

            return interest;

            //var i = (balance * iRate) / YEARDAYCOUNT;
            //var ir = i * days;
            //return ir;
        }



        //public static InterestQueryRowResult QuickInterestCalculation(int daysInYear, decimal balance, decimal interestRate, DateTime dtFrom, DateTime dtTo, decimal? dailyRounding = null)
        //{
        //    var result = new InterestQueryRowResult();

        //    //get days in range
        //    int numDaysInRange = (dtTo.Date - dtFrom.Date).Days;
        //    decimal dailyIR = (interestRate / 100) / daysInYear;

        //    //negative balances should accrue 0 interest
        //    decimal dailyInterest = dailyIR * Math.Max(0, balance);

        //    if (dailyRounding != null)
        //        dailyInterest = Convert.ToDecimal(Math.Round(Convert.ToDouble(dailyInterest), Convert.ToInt16(dailyRounding.Value)));

        //    //check we arent trying to get interest for same day
        //    if (numDaysInRange < 0)
        //        throw new Exception("Calculating Interest on a negative range is not allowed.");

        //    //DateStart and DateEnd for interest accruals are inclusive. 
        //    // Example (These are the dates stored in the database) :
        //    //   dtFrom = 2018-01-01
        //    //   dtTo = 2018-01-05
        //    // If we use (dtTo.Date - dtFrom.Date).Days logic, then we will get 4 days. This is wrong, we need to include the 5th as well. 
        //    // So lets add a day here.
        //    numDaysInRange += 1;

        //    //calculate the interest over these days
        //    decimal interest = dailyInterest * numDaysInRange;

        //    result.Balance = balance;
        //    result.DateStart = dtFrom;
        //    result.DateEnd = dtTo;
        //    result.AccruedInterest = interest;
        //    result.DailyInterest = dailyInterest;
        //    result.Days = numDaysInRange;
        //    result.InterestRate = interestRate;
        //    result.DailyInterestRate = dailyIR;

        //    return result;
        //}


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
