﻿namespace MetarLookup
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MetarForm));
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
            this.txtAltInHg = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.lblCat = new System.Windows.Forms.Label();
            this.txtTime = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.txtSkyConditions = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.txtGusts = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.txtElevFeet = new System.Windows.Forms.TextBox();
            this.txtElevMeter = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.txtAltQNH = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.txtAtis = new System.Windows.Forms.TextBox();
            this.btnArrivalAtis = new System.Windows.Forms.Button();
            this.btnDepartureAtis = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtAirportCode
            // 
            this.txtAirportCode.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtAirportCode.Location = new System.Drawing.Point(12, 12);
            this.txtAirportCode.Name = "txtAirportCode";
            this.txtAirportCode.PlaceholderText = "ICAO Code";
            this.txtAirportCode.Size = new System.Drawing.Size(100, 16);
            this.txtAirportCode.TabIndex = 0;
            // 
            // btnGetMetar
            // 
            this.btnGetMetar.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.btnGetMetar.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnGetMetar.Location = new System.Drawing.Point(118, 8);
            this.btnGetMetar.Name = "btnGetMetar";
            this.btnGetMetar.Size = new System.Drawing.Size(86, 23);
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
            this.txtMetarReport.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtMetarReport.Location = new System.Drawing.Point(12, 112);
            this.txtMetarReport.Multiline = true;
            this.txtMetarReport.Name = "txtMetarReport";
            this.txtMetarReport.ReadOnly = true;
            this.txtMetarReport.Size = new System.Drawing.Size(421, 63);
            this.txtMetarReport.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 94);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(87, 15);
            this.label1.TabIndex = 4;
            this.label1.Text = "Metar Raw Text";
            // 
            // txtID
            // 
            this.txtID.BackColor = System.Drawing.SystemColors.ControlLight;
            this.txtID.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtID.Location = new System.Drawing.Point(56, 38);
            this.txtID.Name = "txtID";
            this.txtID.ReadOnly = true;
            this.txtID.Size = new System.Drawing.Size(40, 16);
            this.txtID.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 41);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 15);
            this.label2.TabIndex = 6;
            this.label2.Text = "ICAO";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 458);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(68, 15);
            this.label3.TabIndex = 8;
            this.label3.Text = "Date / Time";
            // 
            // txtDate
            // 
            this.txtDate.BackColor = System.Drawing.SystemColors.ControlLight;
            this.txtDate.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtDate.Location = new System.Drawing.Point(84, 455);
            this.txtDate.Name = "txtDate";
            this.txtDate.ReadOnly = true;
            this.txtDate.Size = new System.Drawing.Size(88, 16);
            this.txtDate.TabIndex = 7;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 487);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(98, 15);
            this.label4.TabIndex = 10;
            this.label4.Text = "Temp / Dewpoint";
            // 
            // txtTempC
            // 
            this.txtTempC.BackColor = System.Drawing.SystemColors.ControlLight;
            this.txtTempC.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtTempC.Location = new System.Drawing.Point(159, 484);
            this.txtTempC.Name = "txtTempC";
            this.txtTempC.ReadOnly = true;
            this.txtTempC.Size = new System.Drawing.Size(44, 16);
            this.txtTempC.TabIndex = 9;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(209, 487);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(12, 15);
            this.label5.TabIndex = 11;
            this.label5.Text = "/";
            // 
            // txtDew
            // 
            this.txtDew.BackColor = System.Drawing.SystemColors.ControlLight;
            this.txtDew.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtDew.Location = new System.Drawing.Point(225, 483);
            this.txtDew.Name = "txtDew";
            this.txtDew.ReadOnly = true;
            this.txtDew.Size = new System.Drawing.Size(44, 16);
            this.txtDew.TabIndex = 12;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(13, 515);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(136, 15);
            this.label6.TabIndex = 13;
            this.label6.Text = "Wind Dir / Speed / Gusts";
            // 
            // txtSpeed
            // 
            this.txtSpeed.BackColor = System.Drawing.SystemColors.ControlLight;
            this.txtSpeed.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtSpeed.Location = new System.Drawing.Point(215, 512);
            this.txtSpeed.Name = "txtSpeed";
            this.txtSpeed.ReadOnly = true;
            this.txtSpeed.Size = new System.Drawing.Size(33, 16);
            this.txtSpeed.TabIndex = 16;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(198, 515);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(12, 15);
            this.label7.TabIndex = 15;
            this.label7.Text = "/";
            // 
            // txtDir
            // 
            this.txtDir.BackColor = System.Drawing.SystemColors.ControlLight;
            this.txtDir.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtDir.Location = new System.Drawing.Point(159, 512);
            this.txtDir.Name = "txtDir";
            this.txtDir.ReadOnly = true;
            this.txtDir.Size = new System.Drawing.Size(33, 16);
            this.txtDir.TabIndex = 14;
            // 
            // txtVis
            // 
            this.txtVis.BackColor = System.Drawing.SystemColors.ControlLight;
            this.txtVis.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtVis.Location = new System.Drawing.Point(84, 542);
            this.txtVis.Name = "txtVis";
            this.txtVis.ReadOnly = true;
            this.txtVis.Size = new System.Drawing.Size(58, 16);
            this.txtVis.TabIndex = 18;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(13, 545);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(51, 15);
            this.label8.TabIndex = 17;
            this.label8.Text = "Visibility";
            // 
            // txtAltInHg
            // 
            this.txtAltInHg.BackColor = System.Drawing.SystemColors.ControlLight;
            this.txtAltInHg.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtAltInHg.Location = new System.Drawing.Point(84, 570);
            this.txtAltInHg.Name = "txtAltInHg";
            this.txtAltInHg.ReadOnly = true;
            this.txtAltInHg.Size = new System.Drawing.Size(65, 16);
            this.txtAltInHg.TabIndex = 20;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(13, 573);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(56, 15);
            this.label9.TabIndex = 19;
            this.label9.Text = "Altimeter";
            // 
            // lblCat
            // 
            this.lblCat.AutoSize = true;
            this.lblCat.Font = new System.Drawing.Font("Segoe UI", 26F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblCat.Location = new System.Drawing.Point(216, 15);
            this.lblCat.Name = "lblCat";
            this.lblCat.Size = new System.Drawing.Size(171, 47);
            this.lblCat.TabIndex = 21;
            this.lblCat.Text = "Category";
            this.lblCat.Visible = false;
            // 
            // txtTime
            // 
            this.txtTime.BackColor = System.Drawing.SystemColors.ControlLight;
            this.txtTime.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtTime.Location = new System.Drawing.Point(194, 455);
            this.txtTime.Name = "txtTime";
            this.txtTime.ReadOnly = true;
            this.txtTime.Size = new System.Drawing.Size(88, 16);
            this.txtTime.TabIndex = 23;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(178, 458);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(12, 15);
            this.label10.TabIndex = 24;
            this.label10.Text = "/";
            // 
            // txtSkyConditions
            // 
            this.txtSkyConditions.BackColor = System.Drawing.SystemColors.ControlLight;
            this.txtSkyConditions.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtSkyConditions.Location = new System.Drawing.Point(310, 486);
            this.txtSkyConditions.Multiline = true;
            this.txtSkyConditions.Name = "txtSkyConditions";
            this.txtSkyConditions.ReadOnly = true;
            this.txtSkyConditions.Size = new System.Drawing.Size(123, 130);
            this.txtSkyConditions.TabIndex = 25;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(309, 468);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(86, 15);
            this.label11.TabIndex = 26;
            this.label11.Text = "Sky Conditions";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(389, 621);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(40, 15);
            this.label12.TabIndex = 27;
            this.label12.Text = "v.1.1.4";
            // 
            // txtGusts
            // 
            this.txtGusts.BackColor = System.Drawing.SystemColors.ControlLight;
            this.txtGusts.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtGusts.Location = new System.Drawing.Point(272, 512);
            this.txtGusts.Name = "txtGusts";
            this.txtGusts.ReadOnly = true;
            this.txtGusts.Size = new System.Drawing.Size(33, 16);
            this.txtGusts.TabIndex = 28;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(255, 515);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(12, 15);
            this.label13.TabIndex = 29;
            this.label13.Text = "/";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(14, 602);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(55, 15);
            this.label14.TabIndex = 30;
            this.label14.Text = "Elevation";
            // 
            // txtElevFeet
            // 
            this.txtElevFeet.BackColor = System.Drawing.SystemColors.ControlLight;
            this.txtElevFeet.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtElevFeet.Location = new System.Drawing.Point(84, 599);
            this.txtElevFeet.Name = "txtElevFeet";
            this.txtElevFeet.ReadOnly = true;
            this.txtElevFeet.Size = new System.Drawing.Size(58, 16);
            this.txtElevFeet.TabIndex = 31;
            // 
            // txtElevMeter
            // 
            this.txtElevMeter.BackColor = System.Drawing.SystemColors.ControlLight;
            this.txtElevMeter.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtElevMeter.Location = new System.Drawing.Point(162, 599);
            this.txtElevMeter.Name = "txtElevMeter";
            this.txtElevMeter.ReadOnly = true;
            this.txtElevMeter.Size = new System.Drawing.Size(59, 16);
            this.txtElevMeter.TabIndex = 32;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(147, 602);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(12, 15);
            this.label15.TabIndex = 33;
            this.label15.Text = "/";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(154, 573);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(12, 15);
            this.label16.TabIndex = 34;
            this.label16.Text = "/";
            // 
            // txtAltQNH
            // 
            this.txtAltQNH.BackColor = System.Drawing.SystemColors.ControlLight;
            this.txtAltQNH.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtAltQNH.Location = new System.Drawing.Point(170, 570);
            this.txtAltQNH.Name = "txtAltQNH";
            this.txtAltQNH.ReadOnly = true;
            this.txtAltQNH.Size = new System.Drawing.Size(65, 16);
            this.txtAltQNH.TabIndex = 35;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(12, 70);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(39, 15);
            this.label17.TabIndex = 36;
            this.label17.Text = "Name";
            // 
            // txtName
            // 
            this.txtName.BackColor = System.Drawing.SystemColors.ControlLight;
            this.txtName.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtName.Location = new System.Drawing.Point(56, 67);
            this.txtName.Name = "txtName";
            this.txtName.ReadOnly = true;
            this.txtName.Size = new System.Drawing.Size(376, 16);
            this.txtName.TabIndex = 37;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(347, 9);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(89, 19);
            this.checkBox1.TabIndex = 41;
            this.checkBox1.Text = "Dark Theme";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // txtAtis
            // 
            this.txtAtis.BackColor = System.Drawing.SystemColors.ControlLight;
            this.txtAtis.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtAtis.Location = new System.Drawing.Point(12, 216);
            this.txtAtis.Multiline = true;
            this.txtAtis.Name = "txtAtis";
            this.txtAtis.ReadOnly = true;
            this.txtAtis.Size = new System.Drawing.Size(421, 233);
            this.txtAtis.TabIndex = 42;
            // 
            // btnArrivalAtis
            // 
            this.btnArrivalAtis.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.btnArrivalAtis.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnArrivalAtis.Location = new System.Drawing.Point(12, 187);
            this.btnArrivalAtis.Name = "btnArrivalAtis";
            this.btnArrivalAtis.Size = new System.Drawing.Size(86, 23);
            this.btnArrivalAtis.TabIndex = 43;
            this.btnArrivalAtis.Text = "ARR ATIS";
            this.btnArrivalAtis.UseVisualStyleBackColor = true;
            this.btnArrivalAtis.Click += new System.EventHandler(this.btnArrivalAtis_Click);
            // 
            // btnDepartureAtis
            // 
            this.btnDepartureAtis.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.btnDepartureAtis.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnDepartureAtis.Location = new System.Drawing.Point(104, 187);
            this.btnDepartureAtis.Name = "btnDepartureAtis";
            this.btnDepartureAtis.Size = new System.Drawing.Size(86, 23);
            this.btnDepartureAtis.TabIndex = 44;
            this.btnDepartureAtis.Text = "DEP ATIS";
            this.btnDepartureAtis.UseVisualStyleBackColor = true;
            this.btnDepartureAtis.Click += new System.EventHandler(this.btnDepartureAtis_Click);
            // 
            // MetarForm
            // 
            this.AcceptButton = this.btnGetMetar;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(444, 639);
            this.Controls.Add(this.btnDepartureAtis);
            this.Controls.Add(this.btnArrivalAtis);
            this.Controls.Add(this.txtAtis);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.label17);
            this.Controls.Add(this.txtAltQNH);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.txtElevMeter);
            this.Controls.Add(this.txtElevFeet);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.txtGusts);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.txtSkyConditions);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.txtTime);
            this.Controls.Add(this.lblCat);
            this.Controls.Add(this.txtAltInHg);
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
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MetarForm";
            this.Text = "METAR Lookup";
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
        private TextBox txtAltInHg;
        private Label label9;
        private Label lblCat;
        private TextBox txtTime;
        private Label label10;
        private TextBox txtSkyConditions;
        private Label label11;
        private Label label12;
        private TextBox txtGusts;
        private Label label13;
        private Label label14;
        private TextBox txtElevFeet;
        private TextBox txtElevMeter;
        private Label label15;
        private Label label16;
        private TextBox txtAltQNH;
        private Label label17;
        private TextBox txtName;
        private CheckBox checkBox1;
        private TextBox txtAtis;
        private Button btnArrivalAtis;
        private Button btnDepartureAtis;
    }
}