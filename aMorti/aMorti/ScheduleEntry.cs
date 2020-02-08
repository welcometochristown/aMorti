using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aMorti
{
    public class ScheduleEntry
    {
        public enum ScheduleEntryTypeEnum
        {
            Pay = 0,
            Repay =1
        }

        private bool _closedEntry;

        [Browsable(true)]
        [DisplayName("Entry Closed")]
        public bool ClosedEntry
        {
            get
            {
                if (OutstandingTotal == 0)
                    return true;

                return this._closedEntry;
            }
            set
            {
                this._closedEntry = value;
            }
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
        [DisplayName("Capital Balance")]
        public decimal CapitalBalance
        {
            get
            {
                return Schedule.ScheduleEntries.Where(n => n.EntryDate >= EntryDate).Sum(n => -n.CapitalSigned);
            }
        }

        [Browsable(false)]
        [DisplayName("Capital")]
        public decimal Capital
        {
            get
            {
                return ScheduleEntryTransactions.Where(n => n.Type == ScheduleEntryTransaction.TransactionType.Capital).Sum(n => n.Value);
            }
        }

        [Browsable(true)]
        [DisplayName("Capital")]
        public decimal CapitalSigned
        {
            get
            {
                return Capital * EntryPolarity;
            }
        }


        [Browsable(false)]
        [DisplayName("Interest")]
        public decimal Interest
        {
            get
            {
                return ScheduleEntryTransactions.Where(n => n.Type == ScheduleEntryTransaction.TransactionType.Interest).Sum(n => n.Value);
            }
        }

        [Browsable(true)]
        [DisplayName("Interest")]
        public decimal InterestSigned
        {
            get
            {
                return Interest * EntryPolarity;
            }
        }

        [Browsable(false)]
        [DisplayName("Total")]
        public decimal Total
        {
            get
            {
                return Capital + Interest;
            }
        }

        [Browsable(true)]
        [DisplayName("Total")]
        public decimal TotalSigned
        {
            get
            {
                return CapitalSigned + InterestSigned;
            }
        }

        [Browsable(false)]
        [DisplayName("Outstanding Capital")]
        public decimal OutstandingCapital
        {
            get
            {
                if (this._closedEntry)
                    return 0;

                return ScheduleEntryTransactions.Where(n => n.Type == ScheduleEntryTransaction.TransactionType.Capital).Sum(n => n.Outstanding) ;
            }
        }

        [Browsable(true)]
        [DisplayName("Outstanding Capital")]
        public decimal OutstandingCapitalSigned
        {
            get
            {
                if (this._closedEntry)
                    return 0;

                return OutstandingCapital * EntryPolarity;
            }
        }

        [Browsable(true)]
        [DisplayName("Outstanding Interest")]
        public decimal OutstandingInterest
        {
            get
            {
                if (this._closedEntry)
                    return 0;

                return ScheduleEntryTransactions.Where(n => n.Type == ScheduleEntryTransaction.TransactionType.Interest).Sum(n => n.Outstanding);
            }
        }

        [Browsable(true)]
        [DisplayName("Outstanding Interest")]
        public decimal OutstandingInterestSigned
        {
            get
            {
                if (this._closedEntry)
                    return 0;

                return OutstandingInterest * EntryPolarity;
            }
        }

        [Browsable(true)]
        [DisplayName("Outstanding Total")]
        public decimal OutstandingTotal
        {
            get
            {
                if (this._closedEntry)
                    return 0;

                return OutstandingCapital + OutstandingInterest;
            }
        }

        [Browsable(true)]
        [DisplayName("Outstanding Total")]
        public decimal OutstandingTotalSigned
        {
            get
            {
                if (this._closedEntry)
                    return 0;

                return OutstandingTotal * EntryPolarity;
            }
        }

        [Browsable(false)]
        [DisplayName("Polarity")]
        private int EntryPolarity
        {
            get
            {
                return (EntryType == ScheduleEntryTypeEnum.Repay ? -1 : 1);
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

        public override string ToString()
        {
            return $"{EntryDate.ToShortDateString()} : {EntryType.ToString()} {Capital} Capital and {Interest} Interest";
        }

    }
}
