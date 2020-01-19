using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aMorti
{
    public class ScheduleEntryTransaction
    {
        public enum TransactionType
        {
            Capital = 0,
            Interest = 1
        }

        public decimal Value { get; }

        public decimal Outstanding { get; set; }

        public TransactionType Type { get; }

        public ScheduleEntryTransaction(TransactionType type, decimal value)
        {
            this.Type = type;
            this.Value = value;
            this.Outstanding = value;
        }

        /// <summary>
        /// Repay an amount from this transaction
        /// </summary>
        /// <param name="amount"></param>
        /// <returns>the new outstanding amount</returns>
        public decimal Repay(decimal amount)
        {
            Outstanding -= Math.Min(Outstanding, amount);
            return Outstanding;
        }
    }
}
