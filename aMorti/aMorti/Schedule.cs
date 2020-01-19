using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static aMorti.Common;

namespace aMorti
{
    public class Schedule : System.ComponentModel.IListSource
    {

        public class ScheduleOptions
        {
            public int Version { get; set; }
            public decimal InterestRate { get; set; }
            public DateTime StartDate { get; set; }
        }

        private class Entry
        {
            public DateTime Date { get; set; }
            public decimal ValueCapital { get; set; }
            public decimal ValueInterest { get; set; }

            public override string ToString()
            {
                return $"{Date.ToShortDateString()}:{ValueCapital}:{ValueInterest}->{ValueCapital + ValueInterest}";
            }
        }

        public int Version { get; }
        public DateTime StartDate { get; }
        public decimal InterestRate { get; }

        /// <summary>
        /// Allow a user to define their own interest calculation function.
        /// <param>DateFrom</param>
        /// <param>DateTo</param>
        /// <param>Balance</param>
        /// <param>IntRate</param>
        /// <return>decimal</return>decimal>
        /// </summary>
        public Func<DateTime, DateTime, decimal, decimal, int, decimal> InterestCalcFunc => Interest.Calculate;

        public List<ScheduleEntry> ScheduleEntries { get; } = new List<ScheduleEntry>();

        public bool ContainsListCollection => false;

        public static IEnumerable<ScheduleParameter.ParameterType> RequiredParams = new[] { 
            ScheduleParameter.ParameterType.VERSION, 
            ScheduleParameter.ParameterType.STARTDATE, 
            ScheduleParameter.ParameterType.INTERESTRATE
        };

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

        public Schedule(ScheduleOptions options)
            :this(options.StartDate, options.InterestRate, options.Version)
        {    /**/   }

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
        /// <returns>parsed value or default(T) if not found</returns>
        private static T ParseParameter<T>(IEnumerable<ScheduleParameter> prms, ScheduleParameter.ParameterType type)
        {
            var r = prms.SingleOrDefault(n => n.Type == type);

            if (r == null)
                return default(T);

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
            if (paymentParameters != null && paymentParameters.Any())
            {
                //get all payment entry dates
                // IEnumerable<Entry> pEntries = BuildDateTable().Select(n => new Entry { Date = n });
            }

            if (repaymentParameters != null && repaymentParameters.Any())
            { 
                //check we have a repayment option defined
                int repaymentOption = repaymentParameters.Count(n => n.Type == ScheduleParameter.ParameterType.REPAYMENT_OPTION_FREQUENCY_INSTANCES || n.Type == ScheduleParameter.ParameterType.REPAYMENT_OPTION_REPAY_VALUE);

                if (repaymentOption == 0)
                    throw new Exception("No repayment option specified");

                if (repaymentOption > 1)
                    throw new Exception("Too many repayment options specified");

                //how many months?
                var repaymentOptionDuration = ParseParameter<int>(repaymentParameters, ScheduleParameter.ParameterType.REPAYMENT_OPTION_FREQUENCY_INSTANCES);

                //how much to repay each repayment?
                var repaymentOptionValue = ParseParameter<decimal>(repaymentParameters, ScheduleParameter.ParameterType.REPAYMENT_OPTION_REPAY_VALUE);

                IEnumerable<Entry> rEntries = default(IEnumerable<Entry>);

                Frequency freq = (Common.Frequency)Enum.Parse(typeof(Common.Frequency), ParseParameter<string>(repaymentParameters, ScheduleParameter.ParameterType.REPAYMENT_FREQUENCY));
                
                //which day of the month are we paying?
                int dayOfMonth = ParseParameter<int>(repaymentParameters, ScheduleParameter.ParameterType.REPAYMENT_DAYOFMONTH);

                //Repay by duration?
                if (repaymentOptionDuration != 0)
                {
              
                    //get a set of dates based off the start date forward
                    rEntries = BuildDateTable(freq, StartDate, StartDate.AddFrequency(freq, repaymentOptionDuration), dayOfMonth, false).Select(n => new Entry { Date = n }).ToList();
                    
                    //generate repayments
                    rEntries = GenerateRepayments(StartDate, rEntries, freq,
                                                            ParseParameter<decimal>(repaymentParameters, ScheduleParameter.ParameterType.REPAYMENT_CAPITALOUTSTANDING),
                                                            ParseParameter<decimal>(repaymentParameters, ScheduleParameter.ParameterType.REPAYMENT_INTERESTOUTSTANDING));

                }
                else if (repaymentOptionValue != 0)
                {
                    rEntries =  GenerateRepayments(StartDate, repaymentOptionValue, freq, dayOfMonth, ParseParameter<decimal>(repaymentParameters, ScheduleParameter.ParameterType.REPAYMENT_CAPITALOUTSTANDING),
                        ParseParameter<decimal>(repaymentParameters, ScheduleParameter.ParameterType.REPAYMENT_INTERESTOUTSTANDING));
                }

                if (rEntries == null || rEntries.Count() == 0)
                    throw new Exception("Failed to generate any repayment entries");

                foreach (var e in rEntries)
                {
                    var entry = new ScheduleEntry(this, ScheduleEntry.ScheduleEntryTypeEnum.Repay, e.Date);
                    entry.AddScheduleEntryTransaction(ScheduleEntryTransaction.TransactionType.Capital, e.ValueCapital);
                    entry.AddScheduleEntryTransaction(ScheduleEntryTransaction.TransactionType.Interest, e.ValueInterest);
                    ScheduleEntries.Add(entry);
                }
            }
        }

 

        private IEnumerable<DateTime> BuildDateTable(Frequency frequency, DateTime start, DateTime? end, int day, bool useStartAsFirst = true)
        {
            if (day < 1)
                throw new Exception("Day can't be less than 1");

            if (day > 31)
                throw new Exception("Day can't be more than 31");

            List<DateTime> list = new List<DateTime>();
            DateTime dtLast = start;

            while (true)
            {
                DateTime dt;

                //do we use the start date as the first entry, are we on the first entry?
                if (useStartAsFirst && !list.Any())
                    dt = start;
                else
                    dt = dtLast.NextDate(frequency, day);

                //if we have gone past the end date then edn
                if (end.HasValue && dt.Date > end.Value)
                    break;

                //add day to list
                list.Add(dt.Date);

                //this is now the current date
                dtLast = dt;
            }

            return list;
        }

        private IEnumerable<Entry> GenerateRepayments(DateTime dateStart, IEnumerable<Entry> entries, Frequency frequency , decimal capitalBalance, decimal interestBalance)
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

                //ratio based on a 12 month year
                decimal yearly_month_ratio = 1200 / Common.GetMonthlyMultiplier(frequency);
                decimal ir = (1.0m + (InterestRate / yearly_month_ratio));
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
                if (dateStart != entry.Date)
                    interestAccrued += InterestCalcFunc(dtLast, interest_accrual_end, currentBalance, InterestRate, 365);

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

        private IEnumerable<Entry> GenerateRepayments(DateTime dateStart, decimal repayment_capital_value, Frequency frequency, int dayofmonth, decimal capitalBalance, decimal interestBalance)
        {
            List<Entry> entries = new List<Entry>();

            //this is the ideal repayment across all schedule repayment entries
            decimal ideal_total_repayment = repayment_capital_value;

            if (ideal_total_repayment == 0)
                throw new Exception("Ideal Repyment Cant Be Zero");

            DateTime dtLast = dateStart.Date;
            DateTime dt = dateStart.Date;

            decimal interestDue = interestBalance;
            decimal currentBalance = capitalBalance;

            while (capitalBalance > entries.Sum(n => Math.Abs(n.ValueCapital)))
            {
                Entry entry = new Entry { Date = dt };

                decimal previousCapitalRepayments = entries.Sum(n => Math.Abs(n.ValueCapital));
                currentBalance = capitalBalance - previousCapitalRepayments;

                //get interest from the core microservice.
                decimal interestAccrued = interestDue;

                //when should the interest accrual end?
                var interest_accrual_end = entry.Date.AddDays(-1);
                 
                //only days after the first entry will accrue interest
                if (dateStart != entry.Date)
                    interestAccrued += InterestCalcFunc(dtLast, interest_accrual_end, currentBalance, InterestRate, 365);

                interestDue = 0;
                entry.ValueInterest = Math.Round(interestAccrued, 2, MidpointRounding.ToEven);

                decimal capitalValue = 0;

                if (ideal_total_repayment < currentBalance)
                    capitalValue = ideal_total_repayment - entry.ValueInterest;
                else if (currentBalance + entry.ValueInterest > ideal_total_repayment)
                    capitalValue = currentBalance - entry.ValueInterest;
                else
                    capitalValue = currentBalance + entry.ValueInterest;

                entry.ValueCapital = Math.Round(capitalValue, 2);

                dtLast = dt;
                dt = dtLast.NextDate(frequency, dayofmonth);

                entries.Add(entry);
            }

            return entries;
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
