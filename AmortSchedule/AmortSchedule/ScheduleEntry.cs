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
        [Browsable(false)]
        public  Schedule Schedule { get; }

        [Browsable(true)]
        [DisplayName("Date")]
        public DateTime EntryDate { get; }

        [Browsable(true)]
        [DisplayName("Capital Due")]
        public decimal Capital
        {
            get
            {
                return ScheduleEntryTransactions.Where(n => n.Type == ScheduleEntryTransaction.TransactionType.Capital).Sum(n => n.Value);
            }
        }

        [Browsable(true)]
        [DisplayName("Interest Due")]
        public decimal Interest
        {
            get
            {
                return ScheduleEntryTransactions.Where(n => n.Type == ScheduleEntryTransaction.TransactionType.Interest).Sum(n => n.Value);
            }
        }

        [Browsable(true)]
        [DisplayName("Total Due")]
        public decimal Total
        {
            get
            {
                return Capital + Interest;
            }
        }

        [Browsable(true)]
        [DisplayName("Capital Paid")]
        public decimal CapitalPaid
        {
            get
            {
                return Capital - CapitalOutstanding;
            }
        }

        [Browsable(true)]
        [DisplayName("Interest Paid")]
        public decimal InterestPaid
        {
            get
            {
                return Interest - InterestOutstanding;
            }
        }


        [Browsable(true)]
        [DisplayName("Total Paid")]
        public decimal TotalPaid
        {
            get
            {
                return Total - TotalOutstanding;
            }
        }

        [Browsable(true)]
        [DisplayName("Capital Outstanding")]
        public decimal CapitalOutstanding
        {
            get
            {
                return ScheduleEntryTransactions.Where(n => n.Type == ScheduleEntryTransaction.TransactionType.Capital).Sum(n => n.Outstanding);
            }
        }

        [Browsable(true)]
        [DisplayName("Interest Outstanding")]
        public decimal InterestOutstanding
        {
            get
            {
                return ScheduleEntryTransactions.Where(n => n.Type == ScheduleEntryTransaction.TransactionType.Interest).Sum(n => n.Outstanding);
            }
        }

        [Browsable(true)]
        [DisplayName("Total Outstanding")]
        public decimal TotalOutstanding
        {
            get
            {
                return ScheduleEntryTransactions.Sum(n => n.Outstanding);
            }
        }

        [Browsable(false)]
        public List<ScheduleEntryTransaction> ScheduleEntryTransactions { get; } = new List<ScheduleEntryTransaction>();

        public ScheduleEntry(Schedule schedule, DateTime entryDate)
        {
            this.Schedule = schedule;
            this.EntryDate = entryDate.Date;
        }

        public ScheduleEntryTransaction AddScheduleEntryTransaction(ScheduleEntryTransaction.TransactionType type, decimal value)
        {
            var transaction = new ScheduleEntryTransaction(type, value);
            this.ScheduleEntryTransactions.Add(transaction);
            return transaction;
        }

        public void AddScheduleEntryTransaction(IEnumerable<ScheduleEntryTransaction> transactions)
        {
            this.ScheduleEntryTransactions.AddRange(transactions);
        }
    }
}
