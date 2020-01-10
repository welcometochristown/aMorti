using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmortSchedule
{
    public class Schedule : System.ComponentModel.IListSource
    {
        public int Version { get; }

        public DateTime StartDate { get; }

        public decimal InterestRate { get; }

        public List<ScheduleEntry> ScheduleEntries { get; } = new List<ScheduleEntry>();

        public bool ContainsListCollection => false;

        public static IEnumerable<ScheduleParameter.ParameterType> RequiredParams = new [] { ScheduleParameter.ParameterType.VERSION, ScheduleParameter.ParameterType.STARTDATE, ScheduleParameter.ParameterType.INTERESTRATE, ScheduleParameter.ParameterType.PRINCIPLEAMOUNT};

        public Schedule(DateTime startdate, decimal interestrate, int version = 1)
        {
            this.Version = version;
            this.InterestRate = interestrate;
            this.StartDate = startdate;
        }

        public ScheduleEntry AddEntry(DateTime entrydate)
        {
            var entry = new ScheduleEntry(this, entrydate);
            ScheduleEntries.Add(entry);
            return entry;
        }


        /// <summary>
        /// generate a schedule with given parameters
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static Schedule Generate(IEnumerable<ScheduleParameter> parameters)
        {
            return Generate(parameters, null);
        }

        /// <summary>
        /// generate a schedule with given parameters and repayment paramaters
        /// </summary>
        /// <param name="details"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        public static Schedule Generate(IEnumerable<ScheduleParameter> parameters,
                                        IEnumerable<ScheduleRepaymentParameter> repaymentParameters)
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

            //find parameters for schedule creation
            var version = parameters.SingleOrDefault(n => n.Type == ScheduleParameter.ParameterType.VERSION);
            var interestrate = parameters.SingleOrDefault(n => n.Type == ScheduleParameter.ParameterType.INTERESTRATE);
            var startdate = parameters.SingleOrDefault(n => n.Type == ScheduleParameter.ParameterType.STARTDATE);

            //validate / parse
            if (version == null || !int.TryParse(version.Value, out int iVersion))
                throw new Exception($"Missing {ScheduleParameter.ParameterType.VERSION.ToString()} parameter or invalid numerical integer value");

            if (interestrate == null || !decimal.TryParse(interestrate.Value, out decimal dInterestRate))
                throw new Exception($"Missing {ScheduleParameter.ParameterType.INTERESTRATE.ToString()} parameter or invalid numerical decimal value");

            if (startdate == null || !DateTime.TryParse(startdate.Value, out DateTime dtStartDate))
                throw new Exception($"Missing {ScheduleParameter.ParameterType.STARTDATE.ToString()} parameter or invalid datetime value");

            //create schedule
            var schedule = new Schedule(dtStartDate, dInterestRate, iVersion);

            //TODO: generate entries (dates)
            schedule.GenerateEntries(repaymentParameters);

            //TODO: fill entries (values)
            schedule.FillEntries();

            //return the finished schedule
            return schedule;
        }

        protected void GenerateEntries(IEnumerable<ScheduleRepaymentParameter> parameters)
        {
            //var repaymentdatefirst = parameters.SingleOrDefault(n => n.Type == ScheduleParameter.ParameterType.REPAYMENTDATEFIRST);
            //var repaymentdayofmonth = parameters.SingleOrDefault(n => n.Type == ScheduleParameter.ParameterType.REPAYMENTDAYOFMONTH);
            //var repaymentdayendofmonth = parameters.SingleOrDefault(n => n.Type == ScheduleParameter.ParameterType.REPAYMENTDAYENDOFMONTH);
            //var repaymentduration = parameters.SingleOrDefault(n => n.Type == ScheduleParameter.ParameterType.REPAYMENTDURATION);

            ////validate / parse
            //if (repaymentdatefirst != null && !DateTime.TryParse(repaymentdatefirst.Value, out DateTime dtRepaymentDateFirst))
            //    throw new Exception($" {ScheduleParameter.ParameterType.REPAYMENTDATEFIRST.ToString()} parameter invalid datetime value");

            //if (repaymentdayofmonth != null && !DateTime.TryParse(repaymentdatefirst.Value, out DateTime dtRepaymentDateFirst))
            //    throw new Exception($" {ScheduleParameter.ParameterType.REPAYMENTDATEFIRST.ToString()} parameter invalid datetime value");

            //if (repaymentdayendofmonth != null && !DateTime.TryParse(repaymentdatefirst.Value, out DateTime dtRepaymentDateFirst))
            //    throw new Exception($" {ScheduleParameter.ParameterType.REPAYMENTDATEFIRST.ToString()} parameter invalid datetime value");

            //if (repaymentduration != null && !DateTime.TryParse(repaymentdatefirst.Value, out DateTime dtRepaymentDateFirst))
            //    throw new Exception($" {ScheduleParameter.ParameterType.REPAYMENTDATEFIRST.ToString()} parameter invalid datetime value");
        }

        protected void FillEntries()
        {

        }

        /// <summary>
        /// Total amount outstanding on this schedule.
        /// </summary>
        /// <returns></returns>
        public decimal Outstanding()
        {
            return ScheduleEntries.Sum(n => n.TotalOutstanding);
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
