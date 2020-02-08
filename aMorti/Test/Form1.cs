using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using aMorti;
using Newtonsoft.Json;

namespace Test
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            Refresh();
        }

        private void Refresh(Schedule schedule = null)
        {
            Common.Frequency frequency = Common.Frequency.MONTHLY;
            
            if(!string.IsNullOrWhiteSpace(cmbfreq.Text))
                frequency = (Common.Frequency)Enum.Parse(typeof(Common.Frequency), cmbfreq.Text);

            if (schedule == null)
            {
                //Auto Creation 
                schedule = Schedule.Create(new[] {
                    new ScheduleParameter(ScheduleParameter.ParameterType.STARTDATE, dateLoanStart.Value.ToShortDateString()),
                    new ScheduleParameter(ScheduleParameter.ParameterType.VERSION, "1")
                });

                List<SchedulePaymentParameter> paymentParams = new List<SchedulePaymentParameter>();

                foreach (DataGridViewRow r in dataGridView2.Rows)
                {
                    if (r.Cells[0].Value == null || r.Cells[1] == null)
                        continue;

                    if (!DateTime.TryParse(r.Cells[0].Value.ToString(), out DateTime dt) || !decimal.TryParse(r.Cells[1].Value.ToString(), out decimal val))
                        continue;

                    paymentParams.Add(new SchedulePaymentParameter(ScheduleParameter.ParameterType.PAYMENT_CAPITAL, JsonConvert.SerializeObject(Tuple.Create(dt.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture), val))));
                    //paymentParams.Add(new SchedulePaymentParameter(ScheduleParameter.ParameterType.PAYMENT_CAPITAL_HOLIDAY_END, DateTime.Now.Date.AddMonths(3).ToShortDateString()));

                }

                List<ScheduleRepaymentParameter> repaymentParams = new[] {
                    new ScheduleRepaymentParameter(ScheduleParameter.ParameterType.REPAYMENT_INTEREST_RATE, numIR.Value.ToString()), 
                    new ScheduleRepaymentParameter(ScheduleParameter.ParameterType.REPAYMENT_CAPITALOUTSTANDING, NumrepayValue.Value.ToString()),
                    new ScheduleRepaymentParameter(ScheduleParameter.ParameterType.REPAYMENT_DAYOFMONTH, numRepayDay.Value.ToString()),
                    new ScheduleRepaymentParameter(ScheduleParameter.ParameterType.REPAYMENT_FREQUENCY, frequency.ToString()),
                    new ScheduleRepaymentParameter(ScheduleParameter.ParameterType.REPAYMENT_DAYS_IN_YEAR, cmbDaysInYear.Text),

                    //new ScheduleRepaymentParameter(ScheduleParameter.ParameterType.REPAYMENT_DATEFIRST, DateTime.Now.Date.AddMonths(4).ToShortDateString()),
                   // new ScheduleRepaymentParameter(ScheduleParameter.ParameterType.REPAYMENT_CAPITAL_HOLIDAY_END, DateTime.Now.Date.AddMonths(3).ToShortDateString()),

                }.ToList();

                if (radioMnths.Checked)
                    repaymentParams.Add(new ScheduleRepaymentParameter(ScheduleParameter.ParameterType.REPAYMENT_OPTION_FREQUENCY_INSTANCES, numericUpDown1.Value.ToString()));
                else if (radiorepayvalue.Checked)
                    repaymentParams.Add(new ScheduleRepaymentParameter(ScheduleParameter.ParameterType.REPAYMENT_OPTION_REPAY_VALUE, numericUpDown2.Value.ToString()));
                else if (radioMaturity.Checked)
                    repaymentParams.Add(new ScheduleRepaymentParameter(ScheduleParameter.ParameterType.REPAYMENT_OPTION_MATURITY_DATE, dateTimePicker1.Value.ToShortDateString()));

                schedule.Fill(paymentParams, repaymentParams);

            }

            dataGridView1.DataSource = schedule;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //Manual Creation 
            Schedule sManual = new Schedule(DateTime.Now.Date, 1);

            var pEntry = sManual.AddEntry(ScheduleEntry.ScheduleEntryTypeEnum.Pay, DateTime.Now.Date.AddMonths(3));
            pEntry.AddScheduleEntryTransaction(ScheduleEntryTransaction.TransactionType.Capital, 100);

            var rEntry = sManual.AddEntry(ScheduleEntry.ScheduleEntryTypeEnum.Repay, DateTime.Now.AddYears(1));
            rEntry.AddScheduleEntryTransaction(ScheduleEntryTransaction.TransactionType.Capital, 100);
            rEntry.AddScheduleEntryTransaction(ScheduleEntryTransaction.TransactionType.Interest, 8.25m);

            Refresh(sManual);
          
            cmbfreq.Items.AddRange(Enum.GetValues(typeof(Common.Frequency)).Cast<Common.Frequency>().Select(n => n.ToString()).ToArray());
            cmbfreq.Text = Common.Frequency.MONTHLY.ToString();
            cmbDaysInYear.SelectedIndex = 0;
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            var cells = dataGridView1.SelectedCells.Cast<object>().Where(n => n is DataGridViewTextBoxCell).Cast<DataGridViewTextBoxCell>().ToList();

            if (cells.Any(n => !(n.Value is decimal)))
                label1.Text = $"({cells.Count().ToString()})";
            else
                label1.Text = $"{cells.Sum(n => (decimal)n.Value).ToString("#,##0.00")} ({cells.Count().ToString()})";

        }

        private void dataGridView2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                var rows = dataGridView2.SelectedRows;
                foreach (DataGridViewRow r in rows)
                    dataGridView2.Rows.Remove(r);
             }
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            var c = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex] as DataGridViewCheckBoxCell;
            var o = dataGridView1.Rows[e.RowIndex].DataBoundItem as ScheduleEntry;
            if (c != null && o != null)
            {
                //update closed status
                o.ClosedEntry = (bool)c.Value;

                //update gridview
                dataGridView1.Update();
                dataGridView1.Refresh();
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            dataGridView1.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }
    }
}
