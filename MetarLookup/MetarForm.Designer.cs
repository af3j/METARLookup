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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MetarForm));
            txtAirportCode = new TextBox();
            btnGetMetar = new Button();
            contextMenuStrip1 = new ContextMenuStrip(components);
            txtMetarReport = new TextBox();
            label1 = new Label();
            txtID = new TextBox();
            label2 = new Label();
            label3 = new Label();
            txtDate = new TextBox();
            label4 = new Label();
            txtTempC = new TextBox();
            label5 = new Label();
            txtDew = new TextBox();
            label6 = new Label();
            txtSpeed = new TextBox();
            label7 = new Label();
            txtDir = new TextBox();
            txtVis = new TextBox();
            label8 = new Label();
            txtAltInHg = new TextBox();
            label9 = new Label();
            lblCat = new Label();
            txtTime = new TextBox();
            label10 = new Label();
            txtSkyConditions = new TextBox();
            label11 = new Label();
            label12 = new Label();
            txtGusts = new TextBox();
            label13 = new Label();
            label14 = new Label();
            txtElevFeet = new TextBox();
            txtElevMeter = new TextBox();
            label15 = new Label();
            label16 = new Label();
            txtAltQNH = new TextBox();
            label17 = new Label();
            txtName = new TextBox();
            checkBox1 = new CheckBox();
            txtAtis = new TextBox();
            btnArrivalAtis = new Button();
            btnDepartureAtis = new Button();
            SuspendLayout();
            // 
            // txtAirportCode
            // 
            txtAirportCode.BorderStyle = BorderStyle.None;
            txtAirportCode.Location = new Point(12, 12);
            txtAirportCode.Name = "txtAirportCode";
            txtAirportCode.PlaceholderText = "ICAO Code";
            txtAirportCode.Size = new Size(100, 16);
            txtAirportCode.TabIndex = 0;
            // 
            // btnGetMetar
            // 
            btnGetMetar.FlatAppearance.BorderColor = Color.Black;
            btnGetMetar.FlatStyle = FlatStyle.Popup;
            btnGetMetar.Location = new Point(118, 8);
            btnGetMetar.Name = "btnGetMetar";
            btnGetMetar.Size = new Size(86, 23);
            btnGetMetar.TabIndex = 1;
            btnGetMetar.Text = "Get METAR";
            btnGetMetar.UseVisualStyleBackColor = true;
            btnGetMetar.Click += btnGetMetar_Click_1;
            // 
            // contextMenuStrip1
            // 
            contextMenuStrip1.Name = "contextMenuStrip1";
            contextMenuStrip1.Size = new Size(61, 4);
            // 
            // txtMetarReport
            // 
            txtMetarReport.BackColor = SystemColors.ControlLight;
            txtMetarReport.BorderStyle = BorderStyle.None;
            txtMetarReport.Location = new Point(12, 112);
            txtMetarReport.Multiline = true;
            txtMetarReport.Name = "txtMetarReport";
            txtMetarReport.ReadOnly = true;
            txtMetarReport.Size = new Size(421, 63);
            txtMetarReport.TabIndex = 3;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 94);
            label1.Name = "label1";
            label1.Size = new Size(87, 15);
            label1.TabIndex = 4;
            label1.Text = "Metar Raw Text";
            // 
            // txtID
            // 
            txtID.BackColor = SystemColors.ControlLight;
            txtID.BorderStyle = BorderStyle.None;
            txtID.Location = new Point(56, 38);
            txtID.Name = "txtID";
            txtID.ReadOnly = true;
            txtID.Size = new Size(40, 16);
            txtID.TabIndex = 5;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 41);
            label2.Name = "label2";
            label2.Size = new Size(35, 15);
            label2.TabIndex = 6;
            label2.Text = "ICAO";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(12, 458);
            label3.Name = "label3";
            label3.Size = new Size(68, 15);
            label3.TabIndex = 8;
            label3.Text = "Date / Time";
            // 
            // txtDate
            // 
            txtDate.BackColor = SystemColors.ControlLight;
            txtDate.BorderStyle = BorderStyle.None;
            txtDate.Location = new Point(84, 455);
            txtDate.Name = "txtDate";
            txtDate.ReadOnly = true;
            txtDate.Size = new Size(88, 16);
            txtDate.TabIndex = 7;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(12, 487);
            label4.Name = "label4";
            label4.Size = new Size(98, 15);
            label4.TabIndex = 10;
            label4.Text = "Temp / Dewpoint";
            // 
            // txtTempC
            // 
            txtTempC.BackColor = SystemColors.ControlLight;
            txtTempC.BorderStyle = BorderStyle.None;
            txtTempC.Location = new Point(159, 484);
            txtTempC.Name = "txtTempC";
            txtTempC.ReadOnly = true;
            txtTempC.Size = new Size(44, 16);
            txtTempC.TabIndex = 9;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(209, 487);
            label5.Name = "label5";
            label5.Size = new Size(12, 15);
            label5.TabIndex = 11;
            label5.Text = "/";
            // 
            // txtDew
            // 
            txtDew.BackColor = SystemColors.ControlLight;
            txtDew.BorderStyle = BorderStyle.None;
            txtDew.Location = new Point(225, 483);
            txtDew.Name = "txtDew";
            txtDew.ReadOnly = true;
            txtDew.Size = new Size(44, 16);
            txtDew.TabIndex = 12;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(13, 515);
            label6.Name = "label6";
            label6.Size = new Size(136, 15);
            label6.TabIndex = 13;
            label6.Text = "Wind Dir / Speed / Gusts";
            // 
            // txtSpeed
            // 
            txtSpeed.BackColor = SystemColors.ControlLight;
            txtSpeed.BorderStyle = BorderStyle.None;
            txtSpeed.Location = new Point(215, 512);
            txtSpeed.Name = "txtSpeed";
            txtSpeed.ReadOnly = true;
            txtSpeed.Size = new Size(33, 16);
            txtSpeed.TabIndex = 16;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(198, 515);
            label7.Name = "label7";
            label7.Size = new Size(12, 15);
            label7.TabIndex = 15;
            label7.Text = "/";
            // 
            // txtDir
            // 
            txtDir.BackColor = SystemColors.ControlLight;
            txtDir.BorderStyle = BorderStyle.None;
            txtDir.Location = new Point(159, 512);
            txtDir.Name = "txtDir";
            txtDir.ReadOnly = true;
            txtDir.Size = new Size(33, 16);
            txtDir.TabIndex = 14;
            // 
            // txtVis
            // 
            txtVis.BackColor = SystemColors.ControlLight;
            txtVis.BorderStyle = BorderStyle.None;
            txtVis.Location = new Point(84, 542);
            txtVis.Name = "txtVis";
            txtVis.ReadOnly = true;
            txtVis.Size = new Size(58, 16);
            txtVis.TabIndex = 18;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(13, 545);
            label8.Name = "label8";
            label8.Size = new Size(51, 15);
            label8.TabIndex = 17;
            label8.Text = "Visibility";
            // 
            // txtAltInHg
            // 
            txtAltInHg.BackColor = SystemColors.ControlLight;
            txtAltInHg.BorderStyle = BorderStyle.None;
            txtAltInHg.Location = new Point(84, 570);
            txtAltInHg.Name = "txtAltInHg";
            txtAltInHg.ReadOnly = true;
            txtAltInHg.Size = new Size(65, 16);
            txtAltInHg.TabIndex = 20;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new Point(13, 573);
            label9.Name = "label9";
            label9.Size = new Size(56, 15);
            label9.TabIndex = 19;
            label9.Text = "Altimeter";
            // 
            // lblCat
            // 
            lblCat.AutoSize = true;
            lblCat.Font = new Font("Segoe UI", 26F, FontStyle.Bold, GraphicsUnit.Point);
            lblCat.Location = new Point(216, 15);
            lblCat.Name = "lblCat";
            lblCat.Size = new Size(171, 47);
            lblCat.TabIndex = 21;
            lblCat.Text = "Category";
            lblCat.Visible = false;
            // 
            // txtTime
            // 
            txtTime.BackColor = SystemColors.ControlLight;
            txtTime.BorderStyle = BorderStyle.None;
            txtTime.Location = new Point(194, 455);
            txtTime.Name = "txtTime";
            txtTime.ReadOnly = true;
            txtTime.Size = new Size(88, 16);
            txtTime.TabIndex = 23;
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Location = new Point(178, 458);
            label10.Name = "label10";
            label10.Size = new Size(12, 15);
            label10.TabIndex = 24;
            label10.Text = "/";
            // 
            // txtSkyConditions
            // 
            txtSkyConditions.BackColor = SystemColors.ControlLight;
            txtSkyConditions.BorderStyle = BorderStyle.None;
            txtSkyConditions.Location = new Point(310, 486);
            txtSkyConditions.Multiline = true;
            txtSkyConditions.Name = "txtSkyConditions";
            txtSkyConditions.ReadOnly = true;
            txtSkyConditions.Size = new Size(123, 130);
            txtSkyConditions.TabIndex = 25;
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Location = new Point(309, 468);
            label11.Name = "label11";
            label11.Size = new Size(86, 15);
            label11.TabIndex = 26;
            label11.Text = "Sky Conditions";
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.Location = new Point(389, 621);
            label12.Name = "label12";
            label12.Size = new Size(40, 15);
            label12.TabIndex = 27;
            label12.Text = "v.1.2.0";
            // 
            // txtGusts
            // 
            txtGusts.BackColor = SystemColors.ControlLight;
            txtGusts.BorderStyle = BorderStyle.None;
            txtGusts.Location = new Point(272, 512);
            txtGusts.Name = "txtGusts";
            txtGusts.ReadOnly = true;
            txtGusts.Size = new Size(33, 16);
            txtGusts.TabIndex = 28;
            // 
            // label13
            // 
            label13.AutoSize = true;
            label13.Location = new Point(255, 515);
            label13.Name = "label13";
            label13.Size = new Size(12, 15);
            label13.TabIndex = 29;
            label13.Text = "/";
            // 
            // label14
            // 
            label14.AutoSize = true;
            label14.Location = new Point(14, 602);
            label14.Name = "label14";
            label14.Size = new Size(55, 15);
            label14.TabIndex = 30;
            label14.Text = "Elevation";
            // 
            // txtElevFeet
            // 
            txtElevFeet.BackColor = SystemColors.ControlLight;
            txtElevFeet.BorderStyle = BorderStyle.None;
            txtElevFeet.Location = new Point(84, 599);
            txtElevFeet.Name = "txtElevFeet";
            txtElevFeet.ReadOnly = true;
            txtElevFeet.Size = new Size(58, 16);
            txtElevFeet.TabIndex = 31;
            // 
            // txtElevMeter
            // 
            txtElevMeter.BackColor = SystemColors.ControlLight;
            txtElevMeter.BorderStyle = BorderStyle.None;
            txtElevMeter.Location = new Point(162, 599);
            txtElevMeter.Name = "txtElevMeter";
            txtElevMeter.ReadOnly = true;
            txtElevMeter.Size = new Size(59, 16);
            txtElevMeter.TabIndex = 32;
            // 
            // label15
            // 
            label15.AutoSize = true;
            label15.Location = new Point(147, 602);
            label15.Name = "label15";
            label15.Size = new Size(12, 15);
            label15.TabIndex = 33;
            label15.Text = "/";
            // 
            // label16
            // 
            label16.AutoSize = true;
            label16.Location = new Point(154, 573);
            label16.Name = "label16";
            label16.Size = new Size(12, 15);
            label16.TabIndex = 34;
            label16.Text = "/";
            // 
            // txtAltQNH
            // 
            txtAltQNH.BackColor = SystemColors.ControlLight;
            txtAltQNH.BorderStyle = BorderStyle.None;
            txtAltQNH.Location = new Point(170, 570);
            txtAltQNH.Name = "txtAltQNH";
            txtAltQNH.ReadOnly = true;
            txtAltQNH.Size = new Size(65, 16);
            txtAltQNH.TabIndex = 35;
            // 
            // label17
            // 
            label17.AutoSize = true;
            label17.Location = new Point(12, 70);
            label17.Name = "label17";
            label17.Size = new Size(39, 15);
            label17.TabIndex = 36;
            label17.Text = "Name";
            // 
            // txtName
            // 
            txtName.BackColor = SystemColors.ControlLight;
            txtName.BorderStyle = BorderStyle.None;
            txtName.Location = new Point(56, 67);
            txtName.Name = "txtName";
            txtName.ReadOnly = true;
            txtName.Size = new Size(376, 16);
            txtName.TabIndex = 37;
            // 
            // checkBox1
            // 
            checkBox1.AutoSize = true;
            checkBox1.Location = new Point(347, 9);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new Size(89, 19);
            checkBox1.TabIndex = 41;
            checkBox1.Text = "Dark Theme";
            checkBox1.UseVisualStyleBackColor = true;
            checkBox1.CheckedChanged += checkBox1_CheckedChanged;
            // 
            // txtAtis
            // 
            txtAtis.BackColor = SystemColors.ControlLight;
            txtAtis.BorderStyle = BorderStyle.None;
            txtAtis.Location = new Point(12, 216);
            txtAtis.Multiline = true;
            txtAtis.Name = "txtAtis";
            txtAtis.ReadOnly = true;
            txtAtis.Size = new Size(421, 233);
            txtAtis.TabIndex = 42;
            // 
            // btnArrivalAtis
            // 
            btnArrivalAtis.FlatAppearance.BorderColor = Color.Black;
            btnArrivalAtis.FlatStyle = FlatStyle.Popup;
            btnArrivalAtis.Location = new Point(12, 187);
            btnArrivalAtis.Name = "btnArrivalAtis";
            btnArrivalAtis.Size = new Size(86, 23);
            btnArrivalAtis.TabIndex = 43;
            btnArrivalAtis.Text = "ARR ATIS";
            btnArrivalAtis.UseVisualStyleBackColor = true;
            btnArrivalAtis.Click += btnArrivalAtis_Click;
            // 
            // btnDepartureAtis
            // 
            btnDepartureAtis.FlatAppearance.BorderColor = Color.Black;
            btnDepartureAtis.FlatStyle = FlatStyle.Popup;
            btnDepartureAtis.Location = new Point(104, 187);
            btnDepartureAtis.Name = "btnDepartureAtis";
            btnDepartureAtis.Size = new Size(86, 23);
            btnDepartureAtis.TabIndex = 44;
            btnDepartureAtis.Text = "DEP ATIS";
            btnDepartureAtis.UseVisualStyleBackColor = true;
            btnDepartureAtis.Click += btnDepartureAtis_Click;
            // 
            // MetarForm
            // 
            AcceptButton = btnGetMetar;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(444, 639);
            Controls.Add(btnDepartureAtis);
            Controls.Add(btnArrivalAtis);
            Controls.Add(txtAtis);
            Controls.Add(checkBox1);
            Controls.Add(txtName);
            Controls.Add(label17);
            Controls.Add(txtAltQNH);
            Controls.Add(label16);
            Controls.Add(label15);
            Controls.Add(txtElevMeter);
            Controls.Add(txtElevFeet);
            Controls.Add(label14);
            Controls.Add(label13);
            Controls.Add(txtGusts);
            Controls.Add(label12);
            Controls.Add(label11);
            Controls.Add(txtSkyConditions);
            Controls.Add(label10);
            Controls.Add(txtTime);
            Controls.Add(lblCat);
            Controls.Add(txtAltInHg);
            Controls.Add(label9);
            Controls.Add(txtVis);
            Controls.Add(label8);
            Controls.Add(txtSpeed);
            Controls.Add(label7);
            Controls.Add(txtDir);
            Controls.Add(label6);
            Controls.Add(txtDew);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(txtTempC);
            Controls.Add(label3);
            Controls.Add(txtDate);
            Controls.Add(label2);
            Controls.Add(txtID);
            Controls.Add(label1);
            Controls.Add(txtMetarReport);
            Controls.Add(btnGetMetar);
            Controls.Add(txtAirportCode);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "MetarForm";
            Text = "METAR Lookup";
            ResumeLayout(false);
            PerformLayout();
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