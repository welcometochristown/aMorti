using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aMorti
{
    public class ScheduleRepaymentParameter : ScheduleParameter
    {
        public ScheduleRepaymentParameter(ParameterType type, string value)
            :base(type, value)
        {
            if (!type.ToString().StartsWith("REPAYMENT_"))
                throw new InvalidOperationException($"Invalid repayment paramenter type {type.ToString()}");
        }
    }
}
