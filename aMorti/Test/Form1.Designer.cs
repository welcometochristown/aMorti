namespace Test
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.button1 = new System.Windows.Forms.Button();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.radioMnths = new System.Windows.Forms.RadioButton();
            this.radiorepayvalue = new System.Windows.Forms.RadioButton();
            this.numericUpDown2 = new System.Windows.Forms.NumericUpDown();
            this.cmbfreq = new System.Windows.Forms.ComboBox();
            this.NumrepayValue = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.radioMaturity = new System.Windows.Forms.RadioButton();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.numIR = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.numRepayDay = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.dataGridView2 = new System.Windows.Forms.DataGridView();
            this.label6 = new System.Windows.Forms.Label();
            this.Date = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Value = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dateLoanStart = new System.Windows.Forms.DateTimePicker();
            this.label7 = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumrepayValue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numIR)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRepayDay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Top;
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(1275, 441);
            this.dataGridView1.TabIndex = 0;
            this.dataGridView1.SelectionChanged += new System.EventHandler(this.dataGridView1_SelectionChanged);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(1194, 485);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(58, 92);
            this.button1.TabIndex = 1;
            this.button1.Text = "Refresh";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.Button1_Click);
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Location = new System.Drawing.Point(495, 484);
            this.numericUpDown1.Maximum = new decimal(new int[] {
            999999,
            0,
            0,
            0});
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(200, 20);
            this.numericUpDown1.TabIndex = 2;
            this.numericUpDown1.Value = new decimal(new int[] {
            15,
            0,
            0,
            0});
            // 
            // radioMnths
            // 
            this.radioMnths.AutoSize = true;
            this.radioMnths.Checked = true;
            this.radioMnths.Location = new System.Drawing.Point(380, 484);
            this.radioMnths.Name = "radioMnths";
            this.radioMnths.Size = new System.Drawing.Size(71, 17);
            this.radioMnths.TabIndex = 3;
            this.radioMnths.TabStop = true;
            this.radioMnths.Text = "Instances";
            this.radioMnths.UseVisualStyleBackColor = true;
            // 
            // radiorepayvalue
            // 
            this.radiorepayvalue.AutoSize = true;
            this.radiorepayvalue.Location = new System.Drawing.Point(380, 510);
            this.radiorepayvalue.Name = "radiorepayvalue";
            this.radiorepayvalue.Size = new System.Drawing.Size(109, 17);
            this.radiorepayvalue.TabIndex = 3;
            this.radiorepayvalue.Text = "Repayment Value";
            this.radiorepayvalue.UseVisualStyleBackColor = true;
            // 
            // numericUpDown2
            // 
            this.numericUpDown2.Location = new System.Drawing.Point(495, 510);
            this.numericUpDown2.Maximum = new decimal(new int[] {
            999999,
            0,
            0,
            0});
            this.numericUpDown2.Name = "numericUpDown2";
            this.numericUpDown2.Size = new System.Drawing.Size(200, 20);
            this.numericUpDown2.TabIndex = 2;
            this.numericUpDown2.ThousandsSeparator = true;
            this.numericUpDown2.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // cmbfreq
            // 
            this.cmbfreq.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbfreq.FormattingEnabled = true;
            this.cmbfreq.Location = new System.Drawing.Point(143, 533);
            this.cmbfreq.Name = "cmbfreq";
            this.cmbfreq.Size = new System.Drawing.Size(202, 21);
            this.cmbfreq.TabIndex = 4;
            // 
            // NumrepayValue
            // 
            this.NumrepayValue.Location = new System.Drawing.Point(143, 507);
            this.NumrepayValue.Maximum = new decimal(new int[] {
            999999,
            0,
            0,
            0});
            this.NumrepayValue.Name = "NumrepayValue";
            this.NumrepayValue.Size = new System.Drawing.Size(202, 20);
            this.NumrepayValue.TabIndex = 2;
            this.NumrepayValue.ThousandsSeparator = true;
            this.NumrepayValue.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Location = new System.Drawing.Point(0, 441);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(13, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "_";
            // 
            // radioMaturity
            // 
            this.radioMaturity.AutoSize = true;
            this.radioMaturity.Location = new System.Drawing.Point(380, 537);
            this.radioMaturity.Name = "radioMaturity";
            this.radioMaturity.Size = new System.Drawing.Size(88, 17);
            this.radioMaturity.TabIndex = 3;
            this.radioMaturity.Text = "Maturity Date";
            this.radioMaturity.UseVisualStyleBackColor = true;
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.Location = new System.Drawing.Point(495, 534);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(200, 20);
            this.dateTimePicker1.TabIndex = 6;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(38, 509);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(99, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Outstanding Capital";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(23, 536);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(114, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Repayment Frequency";
            // 
            // numIR
            // 
            this.numIR.Location = new System.Drawing.Point(143, 560);
            this.numIR.Maximum = new decimal(new int[] {
            999999,
            0,
            0,
            0});
            this.numIR.Name = "numIR";
            this.numIR.Size = new System.Drawing.Size(200, 20);
            this.numIR.TabIndex = 2;
            this.numIR.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(69, 562);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(68, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Interest Rate";
            // 
            // numRepayDay
            // 
            this.numRepayDay.Location = new System.Drawing.Point(143, 586);
            this.numRepayDay.Maximum = new decimal(new int[] {
            999999,
            0,
            0,
            0});
            this.numRepayDay.Name = "numRepayDay";
            this.numRepayDay.Size = new System.Drawing.Size(200, 20);
            this.numRepayDay.TabIndex = 2;
            this.numRepayDay.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(54, 586);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(83, 13);
            this.label5.TabIndex = 7;
            this.label5.Text = "Repayment Day";
            // 
            // dataGridView2
            // 
            this.dataGridView2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView2.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Date,
            this.Value});
            this.dataGridView2.Location = new System.Drawing.Point(801, 480);
            this.dataGridView2.Name = "dataGridView2";
            this.dataGridView2.Size = new System.Drawing.Size(365, 97);
            this.dataGridView2.TabIndex = 8;
            this.dataGridView2.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dataGridView2_KeyDown);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(742, 480);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(53, 13);
            this.label6.TabIndex = 7;
            this.label6.Text = "Payments";
            // 
            // Date
            // 
            this.Date.HeaderText = "Date";
            this.Date.Name = "Date";
            // 
            // Value
            // 
            this.Value.HeaderText = "Value";
            this.Value.Name = "Value";
            // 
            // dateLoanStart
            // 
            this.dateLoanStart.Location = new System.Drawing.Point(143, 480);
            this.dateLoanStart.Name = "dateLoanStart";
            this.dateLoanStart.Size = new System.Drawing.Size(200, 20);
            this.dateLoanStart.TabIndex = 6;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(81, 483);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(56, 13);
            this.label7.TabIndex = 7;
            this.label7.Text = "Loan Start";
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "360",
            "364",
            "365"});
            this.comboBox1.Location = new System.Drawing.Point(143, 611);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(200, 21);
            this.comboBox1.TabIndex = 9;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(70, 614);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(67, 13);
            this.label8.TabIndex = 7;
            this.label8.Text = "Days in Year";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1275, 642);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.dataGridView2);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.dateLoanStart);
            this.Controls.Add(this.dateTimePicker1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmbfreq);
            this.Controls.Add(this.radioMaturity);
            this.Controls.Add(this.radiorepayvalue);
            this.Controls.Add(this.radioMnths);
            this.Controls.Add(this.NumrepayValue);
            this.Controls.Add(this.numericUpDown2);
            this.Controls.Add(this.numRepayDay);
            this.Controls.Add(this.numIR);
            this.Controls.Add(this.numericUpDown1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.dataGridView1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumrepayValue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numIR)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRepayDay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.RadioButton radioMnths;
        private System.Windows.Forms.RadioButton radiorepayvalue;
        private System.Windows.Forms.NumericUpDown numericUpDown2;
        private System.Windows.Forms.ComboBox cmbfreq;
        private System.Windows.Forms.NumericUpDown NumrepayValue;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton radioMaturity;
        private System.Windows.Forms.DateTimePicker dateTimePicker1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown numIR;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown numRepayDay;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.DataGridView dataGridView2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Date;
        private System.Windows.Forms.DataGridViewTextBoxColumn Value;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.DateTimePicker dateLoanStart;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label label8;
    }
}

