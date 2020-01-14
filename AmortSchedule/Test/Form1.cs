using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AmortSchedule;

namespace Test
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            DataTable dt = new DataTable();

           
            //Manual Creation 
            Schedule sManual = new Schedule(DateTime.Now.Date, 1, 100);

            var pEntry = sManual.AddEntry(ScheduleEntry.ScheduleEntryTypeEnum.Pay, DateTime.Now);
            pEntry.AddScheduleEntryTransaction(ScheduleEntryTransaction.TransactionType.Capital, 100);

            var rEntry = sManual.AddEntry(ScheduleEntry.ScheduleEntryTypeEnum.Repay, DateTime.Now.AddYears(1));
            rEntry.AddScheduleEntryTransaction(ScheduleEntryTransaction.TransactionType.Capital, 100);
            rEntry.AddScheduleEntryTransaction(ScheduleEntryTransaction.TransactionType.Interest, 8.25m);

            //Auto Creation 
            Schedule sAuto = Schedule.Create(new[] { 
                new ScheduleParameter(ScheduleParameter.ParameterType.STARTDATE, DateTime.Now.Date.ToShortDateString()),
                new ScheduleParameter(ScheduleParameter.ParameterType.INTERESTRATE, "3.45"),
                new ScheduleParameter(ScheduleParameter.ParameterType.VERSION, "1")
            });

            sAuto.Fill(null, new[] {
                new ScheduleRepaymentParameter(ScheduleParameter.ParameterType.REPAYMENT_DATEFIRST, DateTime.Now.Date.ToShortDateString()),
                new ScheduleRepaymentParameter(ScheduleParameter.ParameterType.REPAYMENT_CAPITALOUTSTANDING, "100.00"),
                new ScheduleRepaymentParameter(ScheduleParameter.ParameterType.REPAYMENT_INTERESTOUTSTANDING, "0.00"),
                new ScheduleRepaymentParameter(ScheduleParameter.ParameterType.REPAYMENT_DAYOFMONTH, "1")
            });

            //Mixed Creation 
            //  Schedule sMixed = new Schedule(DateTime.Now.Date, 1, 100);



            dataGridView1.DataSource = sAuto;
        }
    }
}
