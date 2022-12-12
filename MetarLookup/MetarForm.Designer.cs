namespace MetarLookup
{
    partial class MetarForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.txtAirportCode = new System.Windows.Forms.TextBox();
            this.btnGetMetar = new System.Windows.Forms.Button();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.txtMetarReport = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtID = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtDate = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtTempC = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtDew = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtSpeed = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtDir = new System.Windows.Forms.TextBox();
            this.txtVis = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.txtAlt = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.lblCat = new System.Windows.Forms.Label();
            this.txtTime = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.txtSkyConditions = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // txtAirportCode
            // 
            this.txtAirportCode.Location = new System.Drawing.Point(12, 12);
            this.txtAirportCode.Name = "txtAirportCode";
            this.txtAirportCode.Size = new System.Drawing.Size(100, 23);
            this.txtAirportCode.TabIndex = 0;
            this.txtAirportCode.Text = "enter airport code";
            // 
            // btnGetMetar
            // 
            this.btnGetMetar.Location = new System.Drawing.Point(136, 12);
            this.btnGetMetar.Name = "btnGetMetar";
            this.btnGetMetar.Size = new System.Drawing.Size(108, 23);
            this.btnGetMetar.TabIndex = 1;
            this.btnGetMetar.Text = "Get METAR";
            this.btnGetMetar.UseVisualStyleBackColor = true;
            this.btnGetMetar.Click += new System.EventHandler(this.btnGetMetar_Click_1);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // txtMetarReport
            // 
            this.txtMetarReport.BackColor = System.Drawing.SystemColors.ControlLight;
            this.txtMetarReport.Location = new System.Drawing.Point(12, 82);
            this.txtMetarReport.Multiline = true;
            this.txtMetarReport.Name = "txtMetarReport";
            this.txtMetarReport.ReadOnly = true;
            this.txtMetarReport.Size = new System.Drawing.Size(421, 54);
            this.txtMetarReport.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 64);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 15);
            this.label1.TabIndex = 4;
            this.label1.Text = "Raw Text";
            // 
            // txtID
            // 
            this.txtID.BackColor = System.Drawing.SystemColors.ControlLight;
            this.txtID.Location = new System.Drawing.Point(67, 142);
            this.txtID.Name = "txtID";
            this.txtID.ReadOnly = true;
            this.txtID.Size = new System.Drawing.Size(66, 23);
            this.txtID.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 150);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(18, 15);
            this.label2.TabIndex = 6;
            this.label2.Text = "ID";
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(16, 174);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(68, 15);
            this.label3.TabIndex = 8;
            this.label3.Text = "Date / Time";
            this.label3.Click += new System.EventHandler(this.label3_Click);
            // 
            // txtDate
            // 
            this.txtDate.BackColor = System.Drawing.SystemColors.ControlLight;
            this.txtDate.Location = new System.Drawing.Point(87, 171);
            this.txtDate.Name = "txtDate";
            this.txtDate.ReadOnly = true;
            this.txtDate.Size = new System.Drawing.Size(88, 23);
            this.txtDate.TabIndex = 7;
            this.txtDate.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(16, 203);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(98, 15);
            this.label4.TabIndex = 10;
            this.label4.Text = "Temp / Dewpoint";
            // 
            // txtTempC
            // 
            this.txtTempC.BackColor = System.Drawing.SystemColors.ControlLight;
            this.txtTempC.Location = new System.Drawing.Point(149, 200);
            this.txtTempC.Name = "txtTempC";
            this.txtTempC.ReadOnly = true;
            this.txtTempC.Size = new System.Drawing.Size(44, 23);
            this.txtTempC.TabIndex = 9;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(192, 203);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(12, 15);
            this.label5.TabIndex = 11;
            this.label5.Text = "/";
            // 
            // txtDew
            // 
            this.txtDew.BackColor = System.Drawing.SystemColors.ControlLight;
            this.txtDew.Location = new System.Drawing.Point(201, 199);
            this.txtDew.Name = "txtDew";
            this.txtDew.ReadOnly = true;
            this.txtDew.Size = new System.Drawing.Size(44, 23);
            this.txtDew.TabIndex = 12;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(16, 231);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(129, 15);
            this.label6.TabIndex = 13;
            this.label6.Text = "Wind Direction / Speed";
            // 
            // txtSpeed
            // 
            this.txtSpeed.BackColor = System.Drawing.SystemColors.ControlLight;
            this.txtSpeed.Location = new System.Drawing.Point(191, 228);
            this.txtSpeed.Name = "txtSpeed";
            this.txtSpeed.ReadOnly = true;
            this.txtSpeed.Size = new System.Drawing.Size(33, 23);
            this.txtSpeed.TabIndex = 16;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(181, 231);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(12, 15);
            this.label7.TabIndex = 15;
            this.label7.Text = "/";
            // 
            // txtDir
            // 
            this.txtDir.BackColor = System.Drawing.SystemColors.ControlLight;
            this.txtDir.Location = new System.Drawing.Point(149, 228);
            this.txtDir.Name = "txtDir";
            this.txtDir.ReadOnly = true;
            this.txtDir.Size = new System.Drawing.Size(33, 23);
            this.txtDir.TabIndex = 14;
            // 
            // txtVis
            // 
            this.txtVis.BackColor = System.Drawing.SystemColors.ControlLight;
            this.txtVis.Location = new System.Drawing.Point(75, 251);
            this.txtVis.Name = "txtVis";
            this.txtVis.ReadOnly = true;
            this.txtVis.Size = new System.Drawing.Size(58, 23);
            this.txtVis.TabIndex = 18;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(16, 254);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(51, 15);
            this.label8.TabIndex = 17;
            this.label8.Text = "Visibility";
            // 
            // txtAlt
            // 
            this.txtAlt.BackColor = System.Drawing.SystemColors.ControlLight;
            this.txtAlt.Location = new System.Drawing.Point(75, 279);
            this.txtAlt.Name = "txtAlt";
            this.txtAlt.ReadOnly = true;
            this.txtAlt.Size = new System.Drawing.Size(58, 23);
            this.txtAlt.TabIndex = 20;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(18, 282);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(56, 15);
            this.label9.TabIndex = 19;
            this.label9.Text = "Altimeter";
            // 
            // lblCat
            // 
            this.lblCat.AutoSize = true;
            this.lblCat.Font = new System.Drawing.Font("Segoe UI", 26F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblCat.Location = new System.Drawing.Point(262, 9);
            this.lblCat.Name = "lblCat";
            this.lblCat.Size = new System.Drawing.Size(171, 47);
            this.lblCat.TabIndex = 21;
            this.lblCat.Text = "Category";
            this.lblCat.Visible = false;
            // 
            // txtTime
            // 
            this.txtTime.BackColor = System.Drawing.SystemColors.ControlLight;
            this.txtTime.Location = new System.Drawing.Point(182, 171);
            this.txtTime.Name = "txtTime";
            this.txtTime.ReadOnly = true;
            this.txtTime.Size = new System.Drawing.Size(88, 23);
            this.txtTime.TabIndex = 23;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(173, 174);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(12, 15);
            this.label10.TabIndex = 24;
            this.label10.Text = "/";
            // 
            // txtSkyConditions
            // 
            this.txtSkyConditions.BackColor = System.Drawing.SystemColors.ControlLight;
            this.txtSkyConditions.Location = new System.Drawing.Point(306, 171);
            this.txtSkyConditions.Multiline = true;
            this.txtSkyConditions.Name = "txtSkyConditions";
            this.txtSkyConditions.ReadOnly = true;
            this.txtSkyConditions.Size = new System.Drawing.Size(126, 130);
            this.txtSkyConditions.TabIndex = 25;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(306, 150);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(86, 15);
            this.label11.TabIndex = 26;
            this.label11.Text = "Sky Conditions";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(392, 304);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(40, 15);
            this.label12.TabIndex = 27;
            this.label12.Text = "v.1.0.0";
            // 
            // MetarForm
            // 
            this.AcceptButton = this.btnGetMetar;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(444, 323);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.txtSkyConditions);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.txtTime);
            this.Controls.Add(this.lblCat);
            this.Controls.Add(this.txtAlt);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.txtVis);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.txtSpeed);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.txtDir);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.txtDew);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtTempC);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtDate);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtID);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtMetarReport);
            this.Controls.Add(this.btnGetMetar);
            this.Controls.Add(this.txtAirportCode);
            this.Name = "MetarForm";
            this.Text = "Metar Lookup";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private TextBox txtAirportCode;
        private Button btnGetMetar;
        private ContextMenuStrip contextMenuStrip1;
        private TextBox txtMetarReport;
        private Label label1;
        private TextBox txtID;
        private Label label2;
        private Label label3;
        private TextBox txtDate;
        private Label label4;
        private TextBox txtTempC;
        private Label label5;
        private TextBox txtDew;
        private Label label6;
        private TextBox txtSpeed;
        private Label label7;
        private TextBox txtDir;
        private TextBox txtVis;
        private Label label8;
        private TextBox txtAlt;
        private Label label9;
        private Label lblCat;
        private TextBox txtTime;
        private Label label10;
        private TextBox txtSkyConditions;
        private Label label11;
        private Label label12;
    }
}