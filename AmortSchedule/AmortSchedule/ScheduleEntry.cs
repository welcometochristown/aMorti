using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmortSchedule
{
    public class ScheduleEntry
    {
        public enum ScheduleEntryTypeEnum
        {
            Pay,
            Repay
        }

        [Browsable(false)]
        public Schedule Schedule { get; }

        [Browsable(true)]
        [DisplayName("Type")]
        public ScheduleEntryTypeEnum EntryType { get; }

        [Browsable(true)]
        [DisplayName("Date")]
        public DateTime EntryDate { get; }

        [Browsable(true)]
        [DisplayName("Capital")]
        public decimal Capital
        {
            get
            {
                return ScheduleEntryTransactions.Where(n => n.Type == ScheduleEntryTransaction.TransactionType.Capital).Sum(n => n.Value);
            }
        }

        [Browsable(true)]
        [DisplayName("Interest")]
        public decimal Interest
        {
            get
            {
                return ScheduleEntryTransactions.Where(n => n.Type == ScheduleEntryTransaction.TransactionType.Interest).Sum(n => n.Value);
            }
        }

        [Browsable(true)]
        [DisplayName("Total")]
        public decimal Total
        {
            get
            {
                return Capital + Interest;
            }
        }


        [Browsable(true)]
        [DisplayName("Outstanding Capital")]
        public decimal OutstandingCapital
        {
            get
            {
                return ScheduleEntryTransactions.Where(n => n.Type == ScheduleEntryTransaction.TransactionType.Capital).Sum(n => n.Outstanding);
            }
        }

        [Browsable(true)]
        [DisplayName("Outstanding Interest")]
        public decimal OutstandingInterest
        {
            get
            {
                return ScheduleEntryTransactions.Where(n => n.Type == ScheduleEntryTransaction.TransactionType.Interest).Sum(n => n.Outstanding);
            }
        }

        [Browsable(true)]
        [DisplayName("Total")]
        public decimal OutstandingTotal
        {
            get
            {
                return OutstandingCapital + OutstandingInterest;
            }
        }


        [Browsable(false)]
        public List<ScheduleEntryTransaction> ScheduleEntryTransactions { get; } = new List<ScheduleEntryTransaction>();

        public ScheduleEntry(Schedule schedule, ScheduleEntryTypeEnum type, DateTime entryDate)
        {
            this.Schedule = schedule;
            this.EntryDate = entryDate.Date;
            this.EntryType = type;
        }

        public ScheduleEntryTransaction AddScheduleEntryTransaction(ScheduleEntryTransaction.TransactionType type, decimal value)
        {
            var transaction = new ScheduleEntryTransaction(type, value);
            this.ScheduleEntryTransactions.Add(transaction);
            return transaction;
        }

        public IEnumerable<ScheduleEntryTransaction> AddScheduleEntryTransactions(IEnumerable<Tuple<ScheduleEntryTransaction.TransactionType, decimal>> entries )
        {
            foreach(var i in entries)
                yield return AddScheduleEntryTransaction(i.Item1, i.Item2);
        }

        public decimal Outstanding(ScheduleEntryTransaction.TransactionType type)
        {
            return ScheduleEntryTransactions.Where(n => n.Type == type).Sum(n => n.Outstanding);
        }

    }
}
