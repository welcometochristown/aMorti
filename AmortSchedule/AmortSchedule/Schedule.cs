using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmortSchedule
{
    public class Schedule  : System.ComponentModel.IListSource
    {
        public int Version { get; }

        public DateTime StartDate { get; }

        public decimal InterestRate { get; }

        public decimal PrincipleAmount { get; }

        public List<ScheduleEntry> ScheduleEntries { get; } = new List<ScheduleEntry>();

        public bool ContainsListCollection => false;

        public Schedule(DateTime startdate, decimal interestrate, decimal principleamount, int version=1)
        {
            this.Version = version;
            this.InterestRate = interestrate;
            this.PrincipleAmount = principleamount;
            this.StartDate = startdate;
        }

        public ScheduleEntry AddEntry(DateTime entrydate)
        {
            var entry = new ScheduleEntry(this, entrydate);
            ScheduleEntries.Add(entry);
            return entry;
        }

        public static Schedule Generate(object details, int version)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Total amount outstanding on this schedule.
        /// </summary>
        /// <returns></returns>
        public decimal Outstanding()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Total amount outstanding on this schedule for this transaction type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public decimal Outstanding(ScheduleEntryTransaction.TransactionType type)
        {
            throw new NotImplementedException();
        }

        public IList GetList()
        {
            return ScheduleEntries;
        }
    }
}
