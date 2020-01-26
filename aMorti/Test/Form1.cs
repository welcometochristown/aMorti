using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using aMorti;

namespace Test
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Refresh();
        }

        private void Refresh()
        {
            Common.Frequency frequency = Common.Frequency.MONTHLY;
            
            if(!string.IsNullOrWhiteSpace(cmbfreq.Text))
                frequency = (Common.Frequency)Enum.Parse(typeof(Common.Frequency), cmbfreq.Text);

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
                new ScheduleParameter(ScheduleParameter.ParameterType.INTERESTRATE, "5.0"),
                new ScheduleParameter(ScheduleParameter.ParameterType.VERSION, "1")
            });

            List<ScheduleRepaymentParameter> repaymentParams = new[] {
                new ScheduleRepaymentParameter(ScheduleParameter.ParameterType.REPAYMENT_DATEFIRST, DateTime.Now.Date.AddMonths(1).ToShortDateString()),
                new ScheduleRepaymentParameter(ScheduleParameter.ParameterType.REPAYMENT_CAPITALOUTSTANDING, NumrepayValue.Value.ToString()),
                new ScheduleRepaymentParameter(ScheduleParameter.ParameterType.REPAYMENT_INTERESTOUTSTANDING, "0.00"),
                new ScheduleRepaymentParameter(ScheduleParameter.ParameterType.REPAYMENT_DAYOFMONTH, "1"),
                new ScheduleRepaymentParameter(ScheduleParameter.ParameterType.REPAYMENT_FREQUENCY, frequency.ToString())
            }.ToList();

            if(radioMnths.Checked)
                repaymentParams.Add(new ScheduleRepaymentParameter(ScheduleParameter.ParameterType.REPAYMENT_OPTION_FREQUENCY_INSTANCES, numericUpDown1.Value.ToString()));
            else if(radiorepayvalue.Checked)
               repaymentParams.Add(new ScheduleRepaymentParameter(ScheduleParameter.ParameterType.REPAYMENT_OPTION_REPAY_VALUE, numericUpDown2.Value.ToString()));


            sAuto.Fill(null, repaymentParams);

            //Mixed Creation 
            //  Schedule sMixed = new Schedule(DateTime.Now.Date, 1, 100);



            dataGridView1.DataSource = sAuto;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Refresh();
          
            cmbfreq.Items.AddRange(Enum.GetValues(typeof(Common.Frequency)).Cast<Common.Frequency>().Select(n => n.ToString()).ToArray());
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            var cells = dataGridView1.SelectedCells.Cast<DataGridViewTextBoxCell>();

            if (cells.Any(n => !(n.Value is decimal)))
                label1.Text = "NaN";
            else
                label1.Text = cells.Sum(n => (decimal)n.Value).ToString("#,##0.00");

        }
    }
}
