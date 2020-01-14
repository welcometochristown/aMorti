using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmortSchedule
{
    public class SchedulePaymentParameter : ScheduleParameter
    {
        public SchedulePaymentParameter(ScheduleParameter.ParameterType type, string value)
            :base(type, value)
        {
            if (!type.ToString().StartsWith("PAYMENT_"))
                throw new InvalidOperationException($"Invalid payment paramenter type {type.ToString()}");
        }
    }
}
