using Newtonsoft.Json;
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
            public DateTime StartDate { get; set; }
        }

        private class Entry
        {
            public enum EntryTypeEnum
            {
                Pay = 0, Repay = 1, Outstanding = 2
            }

            public EntryTypeEnum Type { get; set; }
            public DateTime Date { get; set; }
            public decimal ValueCapital { get; set; }
            public decimal ValueCapitalSigned 
            {
                get
                {
                    return Type == EntryTypeEnum.Repay ? -ValueCapital : ValueCapital;
                }
            }

            public decimal ValueInterest { get; set; }
            public decimal ValueInterestSigned
            {
                get
                {
                    return Type == EntryTypeEnum.Repay ? -ValueInterest : ValueInterest;
                }
            }

            public override string ToString()
            {
                return $"{Date.ToShortDateString()}:{ValueCapitalSigned}:{ValueInterestSigned}->{ValueCapitalSigned + ValueInterestSigned}";
            }
        }

        public int Version { get; }
        public DateTime StartDate { get; }

        /// <summary>
        /// Allow a user to define their own interest calculation function.
        /// <param>DateFrom</param>
        /// <param>DateTo</param>
        /// <param>Balance</param>
        /// <param>IntRate</param>
        /// <return>decimal</return>
        /// </summary>
        public Func<DateTime, DateTime, decimal, decimal, int, decimal> InterestCalcFunc => Interest.Calculate;

        //Schedule Entries On This Schedule
        public List<ScheduleEntry> ScheduleEntries { get; } = new List<ScheduleEntry>();

        //IListSource
        public bool ContainsListCollection => false;

        //Required Parameters For A Schedule
        public static IEnumerable<ScheduleParameter.ParameterType> RequiredParams = new[] { 
            ScheduleParameter.ParameterType.VERSION, 
            ScheduleParameter.ParameterType.STARTDATE
        };

        /// <summary>
        /// Schedule Constructor. Must take start date and interest rate.
        /// </summary>
        /// <param name="startdate"></param>
        /// <param name="interestrate"></param>
        /// <param name="version"></param>
        public Schedule(DateTime startdate, int version = 1)
        {
            this.Version = version;
            this.StartDate = startdate;
        }

        /// <summary>
        /// Schedule Constructor. Constructed from [options]
        /// </summary>
        /// <param name="options"></param>
        public Schedule(ScheduleOptions options)
            :this(options.StartDate, options.Version)
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
                                        ParseParameter<int>(parameters, ScheduleParameter.ParameterType.VERSION));

            //fil schedule
            schedule.Fill(paymentParameters, repaymentParameters);

            //return the finished schedule
            return schedule;
        }

        /// <summary>
        /// parse parameters from single string values
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="prms"></param>
        /// <param name="type"></param>
        /// <returns>parsed value or default(T) if not found</returns>
        private static T ParseParameter<T>(IEnumerable<ScheduleParameter> prms, ScheduleParameter.ParameterType type)
        {
           return ParseParameter<T>(prms.SingleOrDefault(n => n.Type == type));
        }

        /// <summary>
        /// Parse a paramter
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="prm"></param>
        /// <returns></returns>
        private static T ParseParameter<T>(ScheduleParameter prm)
        {
            if (prm == null)
                return default;

            var converter = TypeDescriptor.GetConverter(typeof(T));

            if (converter != null)
                return (T)converter.ConvertFromString(prm.Value);

            throw new Exception($"invalid {nameof(T)} value");
        }

        private static T ParseParameterJSON<T>(IEnumerable<ScheduleParameter> prms, ScheduleParameter.ParameterType type)
        {
            return ParseParameterJSON<T>(prms.SingleOrDefault(n => n.Type == type));
        }

        private static T ParseParameterJSON<T>(ScheduleParameter prm)
        {
            return JsonConvert.DeserializeObject<T>(prm.Value);

        }

        /// <summary>
        /// Parse a JSON string parameter into a T,N tuple from a collection of [params]
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="N"></typeparam>
        /// <param name="prm"></param>
        /// <returns></returns>
        private static Tuple<T, N> ParseParameterJSON<T, N>(IEnumerable<ScheduleParameter> prms, ScheduleParameter.ParameterType type)
        {
            return ParseParameterJSON<T, N>(prms.SingleOrDefault(n => n.Type == type));
        }

        /// <summary>
        /// Parse a JSON string parameter into a T,N tuple
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="N"></typeparam>
        /// <param name="prm"></param>
        /// <returns></returns>
        private static Tuple<T, N> ParseParameterJSON<T, N>(ScheduleParameter prm)
        {
            return JsonConvert.DeserializeObject<Tuple<T, N>>(prm.Value);
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
            List<Entry> entries = new List<Entry>();
            List<Entry> balanceMovements = new List<Entry>();

            //Payments
            if (paymentParameters != null && paymentParameters.Any())
            {
                foreach(var p in paymentParameters.Where(n => n.Type == ScheduleParameter.ParameterType.PAYMENT_CAPITAL))
                {
                    var prm = ParseParameterJSON<DateTime, decimal>(p);
                    var e = new Entry
                    {
                        Date = prm.Item1,
                        ValueCapital = prm.Item2,
                        Type = Entry.EntryTypeEnum.Pay
                    };

                    balanceMovements.Add(e);
                }
            }

            //Repayments
            if (repaymentParameters != null && repaymentParameters.Any())
            { 
                //check we have a repayment option defined
                int repaymentOptionCount = repaymentParameters.Count(n => 
                     n.Type == ScheduleParameter.ParameterType.REPAYMENT_OPTION_FREQUENCY_INSTANCES || 
                     n.Type == ScheduleParameter.ParameterType.REPAYMENT_OPTION_REPAY_VALUE ||
                     n.Type == ScheduleParameter.ParameterType.REPAYMENT_OPTION_MATURITY_DATE
                );

                if (repaymentOptionCount == 0)
                    throw new Exception("No repayment option specified");

                if (repaymentOptionCount > 1)
                    throw new Exception("Too many repayment options specified");

                //any outstanding capital?
                decimal capitalOutstanding = ParseParameter<decimal>(repaymentParameters, ScheduleParameter.ParameterType.REPAYMENT_CAPITALOUTSTANDING);

                if (capitalOutstanding > 0)
                {
                    //repay any outstanding balances
                    balanceMovements.Add(new Entry
                    {
                        ValueCapital = capitalOutstanding,
                        Date = StartDate,
                        Type = Entry.EntryTypeEnum.Outstanding
                    });
                }

                //repayment frequency
                Frequency freq = (Common.Frequency)Enum.Parse(typeof(Common.Frequency), ParseParameter<string>(repaymentParameters, ScheduleParameter.ParameterType.REPAYMENT_FREQUENCY));

                //day of month to repay
                int dayOfMonth = ParseParameter<int>(repaymentParameters, ScheduleParameter.ParameterType.REPAYMENT_DAYOFMONTH);

                //interest rate
                int interestRate = ParseParameter<int>(repaymentParameters, ScheduleParameter.ParameterType.REPAYMENT_INTEREST_RATE);

                //how many months?
                var repaymentOptionDuration = ParseParameter<int>(repaymentParameters, ScheduleParameter.ParameterType.REPAYMENT_OPTION_FREQUENCY_INSTANCES);

                //maturity date
                var repaymentOptionMaturityDate = ParseParameter<DateTime>(repaymentParameters, ScheduleParameter.ParameterType.REPAYMENT_OPTION_MATURITY_DATE);

                //how much to repay each repayment?
                var repaymentOptionValue = ParseParameter<decimal>(repaymentParameters, ScheduleParameter.ParameterType.REPAYMENT_OPTION_REPAY_VALUE);

                var daysInYear = ParseParameter<int>(repaymentParameters, ScheduleParameter.ParameterType.REPAYMENT_DAYS_IN_YEAR);


                //repayment option
                bool instanced_repayments = repaymentOptionDuration > 0 || repaymentOptionMaturityDate > DateTime.MinValue;
                bool valued_repayments = repaymentOptionValue > 0 ;

                DateTime? capRepaymentHolidayEnd = ParseParameter<DateTime?>(repaymentParameters, ScheduleParameter.ParameterType.REPAYMENT_CAPITAL_HOLIDAY_END);
                DateTime? firstRepaymentDate = ParseParameter<DateTime?>(repaymentParameters, ScheduleParameter.ParameterType.REPAYMENT_DATEFIRST);
                DateTime? secondRepaymentDate = ParseParameter<DateTime?>(repaymentParameters, ScheduleParameter.ParameterType.REPAYMENT_DATEFIRST);

                //calculate the max date between two
                DateTime maxDate(DateTime? dt1, DateTime dt2)
                {
                    if (dt1 == null)
                        return dt2;

                    return dt1 < dt2 ? dt2 : dt1.Value;
                }

                foreach (var movement in balanceMovements.OrderBy(n => n.Date))
                {
                    //add this pay/outsanding entry
                    entries.RemoveAll(n => n.Date > movement.Date);
                    entries.Add(movement);

                    decimal capitalBalance = entries.Where(n => n.Date <= movement.Date).Sum(n => n.ValueCapitalSigned);

                    //Repay by duration?
                    if (instanced_repayments)
                    {
                        //get the maturity date if we are doing instanced repayments or the maturity date has been defined
                        if (repaymentOptionDuration > 0)
                            repaymentOptionMaturityDate = maxDate(firstRepaymentDate, movement.Date).AddFrequency(repaymentOptionDuration, freq);

                        //get a set of dates based off the start date forward
                        var dates = BuildDateTable(freq, maxDate(firstRepaymentDate, movement.Date), repaymentOptionMaturityDate, dayOfMonth, false).Select(n => new Entry { Date = n }).ToList();

                        if(!dates.Any())
                            throw new Exception("Not enough repayment instances to repay full term. Maturity date is not long enough");

                        //generate repayments
                        var repayments = GenerateRepayments(movement.Date, dates, freq, capitalBalance, interestRate, daysInYear, capRepaymentHolidayEnd);

                        //add entries
                        entries.AddRange(repayments);

                    }
                    else if (valued_repayments)
                    {
                        //generate repayments
                        var repayments = GenerateRepayments(movement.Date, repaymentOptionValue, freq, dayOfMonth, capitalBalance, interestRate, daysInYear, capRepaymentHolidayEnd);

                        //add entries
                        entries.AddRange(repayments);
                    }                   
                }
            }
            else
            {
                entries.AddRange(balanceMovements.Where(n => n.Type == Entry.EntryTypeEnum.Pay));
            }

            ScheduleEntries.Clear();

            foreach (var e in entries.Where(n => n.Type != Entry.EntryTypeEnum.Outstanding))
            {
                var entry = new ScheduleEntry(this, (ScheduleEntry.ScheduleEntryTypeEnum)e.Type, e.Date);
                entry.AddScheduleEntryTransaction(ScheduleEntryTransaction.TransactionType.Capital, e.ValueCapital);
                entry.AddScheduleEntryTransaction(ScheduleEntryTransaction.TransactionType.Interest, e.ValueInterest);
                ScheduleEntries.Add(entry);
            }
        }

        /// <summary>
        /// Build a collection of dates between [start] and [end] using [frequency] as interval.
        /// </summary>
        /// <param name="frequency"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="day"></param>
        /// <param name="useStartAsFirst"></param>
        /// <returns></returns>
        private IEnumerable<DateTime> BuildDateTable(Frequency frequency, DateTime start, DateTime end, int day, bool useStartAsFirst = true)
        {
            if (day < 1)
                throw new Exception("Day can't be less than 1");

            if (day > 31)
                throw new Exception("Day can't be more than 31");

            if (end < start)
                throw new Exception("End cant be earlier than start");

            List<DateTime> list = new List<DateTime>();
            DateTime dtLast = start;

            while (true)
            {
                DateTime dt;

                //do we use the start date as the first entry, are we on the first entry?
                if (useStartAsFirst && !list.Any())
                    dt = start;
                else
                    dt = dtLast.AddFrequency(frequency, day);

                //if we have gone past the end date then edn
                if (dt.Date > end)
                    break;

                //add day to list
                list.Add(dt.Date);

                //this is now the current date
                dtLast = dt;
            }

            return list;
        }

        /// <summary>
        /// Generate Repayments for a set of entries.
        /// </summary>
        /// <param name="dateStart"></param>
        /// <param name="entries"></param>
        /// <param name="frequency"></param>
        /// <param name="capitalBalance"></param>
        /// <param name="capRepaymentHolidayEnd"></param>
        /// <returns></returns>
        private IEnumerable<Entry> GenerateRepayments(DateTime dateStart, IEnumerable<Entry> entries, Frequency frequency, decimal capitalBalance, decimal interestRate, int daysInYear, DateTime? capRepaymentHolidayEnd)
        {
            //how many do we have?
            int instances = entries.Count();

            //if there are none just return them back
            if (instances == 0)
                return entries;

            //this is the ideal repayment across all schedule repayment entries
            decimal ideal_total_repayment = 0;

            if (capRepaymentHolidayEnd != null)
            {
                //change ideal_total_repayment to entries after the end of the capital repayment holday
                instances = entries.Count(n => n.Date > capRepaymentHolidayEnd);
            }

            if (interestRate == 0)
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
                decimal ir = (1.0m + (interestRate / yearly_month_ratio));
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
                decimal interestAccrued = 0;
                
                //when should the interest accrual end?
                var interest_accrual_end = entry.Date.AddDays(-1);

                //only days after the first entry will accrue interest
                if (dateStart != entry.Date)
                    interestAccrued += InterestCalcFunc(dtLast, interest_accrual_end, currentBalance, interestRate, daysInYear);

                //rounded interest
                entry.ValueInterest = Math.Round(interestAccrued, 2, MidpointRounding.ToEven);

                //get the capital depending on how close to the ideal repayment we are
                decimal capitalValue = 0;

                if (capRepaymentHolidayEnd != null && entry.Date <= capRepaymentHolidayEnd)
                    capitalValue = 0;
                else if (entry.ValueInterest > ideal_total_repayment)
                    capitalValue = 0;
                else if (!entries.Any(n => n.Date > entry.Date))
                    capitalValue = currentBalance <= ideal_total_repayment ? currentBalance : ideal_total_repayment;
                else
                    capitalValue = ideal_total_repayment - entry.ValueInterest;

                entry.ValueCapital = Math.Round(capitalValue, 2);
                entry.Type = Entry.EntryTypeEnum.Repay;

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

        /// <summary>
        /// Generate Repayments for a set value.
        /// </summary>
        /// <param name="dateStart"></param>
        /// <param name="repayment_capital_value"></param>
        /// <param name="frequency"></param>
        /// <param name="dayofmonth"></param>
        /// <param name="capitalBalance"></param>
        /// <param name="capRepaymentHolidayEnd"></param>
        /// <returns></returns>
        private IEnumerable<Entry> GenerateRepayments(DateTime dateStart, decimal repayment_capital_value, Frequency frequency, int dayofmonth, decimal capitalBalance, decimal interestRate, int daysInYear, DateTime? capRepaymentHolidayEnd)
        {
            List<Entry> entries = new List<Entry>();

            //this is the ideal repayment across all schedule repayment entries
            decimal ideal_total_repayment = repayment_capital_value;

            if (ideal_total_repayment == 0)
                throw new Exception("Ideal Repyment Cant Be Zero");

            DateTime dtLast = dateStart.Date;
            DateTime dt = dateStart.Date;

            decimal currentBalance = capitalBalance;


            while (capitalBalance > entries.Sum(n => Math.Abs(n.ValueCapital)))
            {
                Entry entry = new Entry { Date = dt };

                decimal previousCapitalRepayments = entries.Sum(n => Math.Abs(n.ValueCapital));
                currentBalance = capitalBalance - previousCapitalRepayments;

                //get interest from the core microservice.
                decimal interestAccrued = 0;

                //when should the interest accrual end?
                var interest_accrual_end = entry.Date.AddDays(-1);
                 
                //only days after the first entry will accrue interest
                if (dateStart != entry.Date)
                    interestAccrued += InterestCalcFunc(dtLast, interest_accrual_end, currentBalance, interestRate, daysInYear);

                entry.ValueInterest = Math.Round(interestAccrued, 2, MidpointRounding.ToEven);

                decimal capitalValue = 0;

                if(capRepaymentHolidayEnd != null && entry.Date <= capRepaymentHolidayEnd)
                    capitalValue = 0;
                else if (ideal_total_repayment < currentBalance)
                    capitalValue = ideal_total_repayment - entry.ValueInterest;
                else if (currentBalance + entry.ValueInterest > ideal_total_repayment)
                    capitalValue = currentBalance - entry.ValueInterest;
                else
                    capitalValue = currentBalance;

                entry.ValueCapital = Math.Round(capitalValue, 2);
                entry.Type = Entry.EntryTypeEnum.Repay;

                dtLast = dt;
                dt = dtLast.AddFrequency(frequency, dayofmonth);

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
