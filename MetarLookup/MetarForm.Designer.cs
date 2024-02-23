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
            lblRaw = new Label();
            txtID = new TextBox();
            lblICAO = new Label();
            lblDateTime = new Label();
            txtDate = new TextBox();
            lblTempDew = new Label();
            txtTempC = new TextBox();
            label5 = new Label();
            txtDew = new TextBox();
            lblWind = new Label();
            txtSpeed = new TextBox();
            label7 = new Label();
            txtDir = new TextBox();
            txtVis = new TextBox();
            lblVis = new Label();
            txtAltInHg = new TextBox();
            lblAlt = new Label();
            lblCat = new Label();
            txtTime = new TextBox();
            label10 = new Label();
            txtSkyConditions = new TextBox();
            lblSky = new Label();
            lblVersion = new Label();
            txtGusts = new TextBox();
            label13 = new Label();
            lblElev = new Label();
            txtElevFeet = new TextBox();
            txtElevMeter = new TextBox();
            label15 = new Label();
            label16 = new Label();
            txtAltQNH = new TextBox();
            lblName = new Label();
            txtName = new TextBox();
            checkBox1 = new CheckBox();
            txtAtis = new TextBox();
            btnArrivalAtis = new Button();
            btnDepartureAtis = new Button();
            txtLocation = new TextBox();
            lblLocation = new Label();
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
            btnGetMetar.BackColor = Color.LightGray;
            btnGetMetar.FlatAppearance.BorderColor = Color.Black;
            btnGetMetar.FlatStyle = FlatStyle.Popup;
            btnGetMetar.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnGetMetar.Location = new Point(118, 8);
            btnGetMetar.Margin = new Padding(1);
            btnGetMetar.Name = "btnGetMetar";
            btnGetMetar.Size = new Size(85, 46);
            btnGetMetar.TabIndex = 1;
            btnGetMetar.Text = "Get Metar";
            btnGetMetar.UseVisualStyleBackColor = false;
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
            txtMetarReport.Location = new Point(12, 143);
            txtMetarReport.Multiline = true;
            txtMetarReport.Name = "txtMetarReport";
            txtMetarReport.ReadOnly = true;
            txtMetarReport.Size = new Size(421, 63);
            txtMetarReport.TabIndex = 3;
            // 
            // lblRaw
            // 
            lblRaw.AutoSize = true;
            lblRaw.Location = new Point(12, 125);
            lblRaw.Name = "lblRaw";
            lblRaw.Size = new Size(87, 15);
            lblRaw.TabIndex = 4;
            lblRaw.Text = "Metar Raw Text";
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
            // lblICAO
            // 
            lblICAO.AutoSize = true;
            lblICAO.Location = new Point(13, 41);
            lblICAO.Name = "lblICAO";
            lblICAO.Size = new Size(35, 15);
            lblICAO.TabIndex = 6;
            lblICAO.Text = "ICAO";
            // 
            // lblDateTime
            // 
            lblDateTime.AutoSize = true;
            lblDateTime.Location = new Point(12, 489);
            lblDateTime.Name = "lblDateTime";
            lblDateTime.Size = new Size(68, 15);
            lblDateTime.TabIndex = 8;
            lblDateTime.Text = "Date / Time";
            // 
            // txtDate
            // 
            txtDate.BackColor = SystemColors.ControlLight;
            txtDate.BorderStyle = BorderStyle.None;
            txtDate.Location = new Point(84, 486);
            txtDate.Name = "txtDate";
            txtDate.ReadOnly = true;
            txtDate.Size = new Size(88, 16);
            txtDate.TabIndex = 7;
            // 
            // lblTempDew
            // 
            lblTempDew.AutoSize = true;
            lblTempDew.Location = new Point(12, 518);
            lblTempDew.Name = "lblTempDew";
            lblTempDew.Size = new Size(98, 15);
            lblTempDew.TabIndex = 10;
            lblTempDew.Text = "Temp / Dewpoint";
            // 
            // txtTempC
            // 
            txtTempC.BackColor = SystemColors.ControlLight;
            txtTempC.BorderStyle = BorderStyle.None;
            txtTempC.Location = new Point(159, 515);
            txtTempC.Name = "txtTempC";
            txtTempC.ReadOnly = true;
            txtTempC.Size = new Size(44, 16);
            txtTempC.TabIndex = 9;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(209, 518);
            label5.Name = "label5";
            label5.Size = new Size(12, 15);
            label5.TabIndex = 11;
            label5.Text = "/";
            // 
            // txtDew
            // 
            txtDew.BackColor = SystemColors.ControlLight;
            txtDew.BorderStyle = BorderStyle.None;
            txtDew.Location = new Point(225, 514);
            txtDew.Name = "txtDew";
            txtDew.ReadOnly = true;
            txtDew.Size = new Size(44, 16);
            txtDew.TabIndex = 12;
            // 
            // lblWind
            // 
            lblWind.AutoSize = true;
            lblWind.Location = new Point(13, 546);
            lblWind.Name = "lblWind";
            lblWind.Size = new Size(136, 15);
            lblWind.TabIndex = 13;
            lblWind.Text = "Wind Dir / Speed / Gusts";
            // 
            // txtSpeed
            // 
            txtSpeed.BackColor = SystemColors.ControlLight;
            txtSpeed.BorderStyle = BorderStyle.None;
            txtSpeed.Location = new Point(215, 543);
            txtSpeed.Name = "txtSpeed";
            txtSpeed.ReadOnly = true;
            txtSpeed.Size = new Size(33, 16);
            txtSpeed.TabIndex = 16;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(198, 546);
            label7.Name = "label7";
            label7.Size = new Size(12, 15);
            label7.TabIndex = 15;
            label7.Text = "/";
            // 
            // txtDir
            // 
            txtDir.BackColor = SystemColors.ControlLight;
            txtDir.BorderStyle = BorderStyle.None;
            txtDir.Location = new Point(159, 543);
            txtDir.Name = "txtDir";
            txtDir.ReadOnly = true;
            txtDir.Size = new Size(33, 16);
            txtDir.TabIndex = 14;
            // 
            // txtVis
            // 
            txtVis.BackColor = SystemColors.ControlLight;
            txtVis.BorderStyle = BorderStyle.None;
            txtVis.Location = new Point(84, 573);
            txtVis.Name = "txtVis";
            txtVis.ReadOnly = true;
            txtVis.Size = new Size(58, 16);
            txtVis.TabIndex = 18;
            // 
            // lblVis
            // 
            lblVis.AutoSize = true;
            lblVis.Location = new Point(13, 576);
            lblVis.Name = "lblVis";
            lblVis.Size = new Size(51, 15);
            lblVis.TabIndex = 17;
            lblVis.Text = "Visibility";
            // 
            // txtAltInHg
            // 
            txtAltInHg.BackColor = SystemColors.ControlLight;
            txtAltInHg.BorderStyle = BorderStyle.None;
            txtAltInHg.Location = new Point(84, 601);
            txtAltInHg.Name = "txtAltInHg";
            txtAltInHg.ReadOnly = true;
            txtAltInHg.Size = new Size(65, 16);
            txtAltInHg.TabIndex = 20;
            // 
            // lblAlt
            // 
            lblAlt.AutoSize = true;
            lblAlt.Location = new Point(13, 604);
            lblAlt.Name = "lblAlt";
            lblAlt.Size = new Size(56, 15);
            lblAlt.TabIndex = 19;
            lblAlt.Text = "Altimeter";
            // 
            // lblCat
            // 
            lblCat.AutoSize = true;
            lblCat.Font = new Font("Segoe UI", 26F, FontStyle.Bold);
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
            txtTime.Location = new Point(194, 486);
            txtTime.Name = "txtTime";
            txtTime.ReadOnly = true;
            txtTime.Size = new Size(88, 16);
            txtTime.TabIndex = 23;
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Location = new Point(178, 489);
            label10.Name = "label10";
            label10.Size = new Size(12, 15);
            label10.TabIndex = 24;
            label10.Text = "/";
            // 
            // txtSkyConditions
            // 
            txtSkyConditions.BackColor = SystemColors.ControlLight;
            txtSkyConditions.BorderStyle = BorderStyle.None;
            txtSkyConditions.Location = new Point(310, 517);
            txtSkyConditions.Multiline = true;
            txtSkyConditions.Name = "txtSkyConditions";
            txtSkyConditions.ReadOnly = true;
            txtSkyConditions.Size = new Size(123, 130);
            txtSkyConditions.TabIndex = 25;
            // 
            // lblSky
            // 
            lblSky.AutoSize = true;
            lblSky.Location = new Point(309, 499);
            lblSky.Name = "lblSky";
            lblSky.Size = new Size(86, 15);
            lblSky.TabIndex = 26;
            lblSky.Text = "Sky Conditions";
            // 
            // lblVersion
            // 
            lblVersion.AutoSize = true;
            lblVersion.Location = new Point(389, 652);
            lblVersion.Name = "lblVersion";
            lblVersion.Size = new Size(40, 15);
            lblVersion.TabIndex = 27;
            lblVersion.Text = "v.1.3.0";
            // 
            // txtGusts
            // 
            txtGusts.BackColor = SystemColors.ControlLight;
            txtGusts.BorderStyle = BorderStyle.None;
            txtGusts.Location = new Point(272, 543);
            txtGusts.Name = "txtGusts";
            txtGusts.ReadOnly = true;
            txtGusts.Size = new Size(33, 16);
            txtGusts.TabIndex = 28;
            // 
            // label13
            // 
            label13.AutoSize = true;
            label13.Location = new Point(255, 546);
            label13.Name = "label13";
            label13.Size = new Size(12, 15);
            label13.TabIndex = 29;
            label13.Text = "/";
            // 
            // lblElev
            // 
            lblElev.AutoSize = true;
            lblElev.Location = new Point(14, 633);
            lblElev.Name = "lblElev";
            lblElev.Size = new Size(55, 15);
            lblElev.TabIndex = 30;
            lblElev.Text = "Elevation";
            // 
            // txtElevFeet
            // 
            txtElevFeet.BackColor = SystemColors.ControlLight;
            txtElevFeet.BorderStyle = BorderStyle.None;
            txtElevFeet.Location = new Point(84, 630);
            txtElevFeet.Name = "txtElevFeet";
            txtElevFeet.ReadOnly = true;
            txtElevFeet.Size = new Size(58, 16);
            txtElevFeet.TabIndex = 31;
            // 
            // txtElevMeter
            // 
            txtElevMeter.BackColor = SystemColors.ControlLight;
            txtElevMeter.BorderStyle = BorderStyle.None;
            txtElevMeter.Location = new Point(162, 630);
            txtElevMeter.Name = "txtElevMeter";
            txtElevMeter.ReadOnly = true;
            txtElevMeter.Size = new Size(59, 16);
            txtElevMeter.TabIndex = 32;
            // 
            // label15
            // 
            label15.AutoSize = true;
            label15.Location = new Point(147, 633);
            label15.Name = "label15";
            label15.Size = new Size(12, 15);
            label15.TabIndex = 33;
            label15.Text = "/";
            // 
            // label16
            // 
            label16.AutoSize = true;
            label16.Location = new Point(154, 604);
            label16.Name = "label16";
            label16.Size = new Size(12, 15);
            label16.TabIndex = 34;
            label16.Text = "/";
            // 
            // txtAltQNH
            // 
            txtAltQNH.BackColor = SystemColors.ControlLight;
            txtAltQNH.BorderStyle = BorderStyle.None;
            txtAltQNH.Location = new Point(170, 601);
            txtAltQNH.Name = "txtAltQNH";
            txtAltQNH.ReadOnly = true;
            txtAltQNH.Size = new Size(65, 16);
            txtAltQNH.TabIndex = 35;
            // 
            // lblName
            // 
            lblName.AutoSize = true;
            lblName.Location = new Point(14, 68);
            lblName.Name = "lblName";
            lblName.Size = new Size(39, 15);
            lblName.TabIndex = 36;
            lblName.Text = "Name";
            // 
            // txtName
            // 
            txtName.BackColor = SystemColors.ControlLight;
            txtName.BorderStyle = BorderStyle.None;
            txtName.Location = new Point(72, 67);
            txtName.Name = "txtName";
            txtName.ReadOnly = true;
            txtName.Size = new Size(360, 16);
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
            txtAtis.Location = new Point(12, 247);
            txtAtis.Multiline = true;
            txtAtis.Name = "txtAtis";
            txtAtis.ReadOnly = true;
            txtAtis.Size = new Size(421, 233);
            txtAtis.TabIndex = 42;
            // 
            // btnArrivalAtis
            // 
            btnArrivalAtis.BackColor = Color.LightGray;
            btnArrivalAtis.FlatAppearance.BorderColor = Color.Black;
            btnArrivalAtis.FlatStyle = FlatStyle.Popup;
            btnArrivalAtis.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnArrivalAtis.Location = new Point(12, 218);
            btnArrivalAtis.Name = "btnArrivalAtis";
            btnArrivalAtis.Size = new Size(86, 23);
            btnArrivalAtis.TabIndex = 43;
            btnArrivalAtis.Text = "ARR ATIS";
            btnArrivalAtis.UseVisualStyleBackColor = false;
            btnArrivalAtis.Click += btnArrivalAtis_Click;
            // 
            // btnDepartureAtis
            // 
            btnDepartureAtis.BackColor = Color.LightGray;
            btnDepartureAtis.FlatAppearance.BorderColor = Color.Black;
            btnDepartureAtis.FlatStyle = FlatStyle.Popup;
            btnDepartureAtis.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnDepartureAtis.Location = new Point(104, 218);
            btnDepartureAtis.Name = "btnDepartureAtis";
            btnDepartureAtis.Size = new Size(86, 23);
            btnDepartureAtis.TabIndex = 44;
            btnDepartureAtis.Text = "DEP ATIS";
            btnDepartureAtis.UseVisualStyleBackColor = false;
            btnDepartureAtis.Click += btnDepartureAtis_Click;
            // 
            // txtLocation
            // 
            txtLocation.BackColor = SystemColors.ControlLight;
            txtLocation.BorderStyle = BorderStyle.None;
            txtLocation.Location = new Point(72, 95);
            txtLocation.Name = "txtLocation";
            txtLocation.ReadOnly = true;
            txtLocation.Size = new Size(361, 16);
            txtLocation.TabIndex = 46;
            txtLocation.TextChanged += textBox1_TextChanged;
            // 
            // lblLocation
            // 
            lblLocation.AutoSize = true;
            lblLocation.Location = new Point(14, 95);
            lblLocation.Name = "lblLocation";
            lblLocation.Size = new Size(53, 15);
            lblLocation.TabIndex = 45;
            lblLocation.Text = "Location";
            lblLocation.Click += label18_Click;
            // 
            // MetarForm
            // 
            AcceptButton = btnGetMetar;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(444, 682);
            Controls.Add(txtLocation);
            Controls.Add(lblLocation);
            Controls.Add(btnDepartureAtis);
            Controls.Add(btnArrivalAtis);
            Controls.Add(txtAtis);
            Controls.Add(checkBox1);
            Controls.Add(txtName);
            Controls.Add(lblName);
            Controls.Add(txtAltQNH);
            Controls.Add(label16);
            Controls.Add(label15);
            Controls.Add(txtElevMeter);
            Controls.Add(txtElevFeet);
            Controls.Add(lblElev);
            Controls.Add(label13);
            Controls.Add(txtGusts);
            Controls.Add(lblVersion);
            Controls.Add(lblSky);
            Controls.Add(txtSkyConditions);
            Controls.Add(label10);
            Controls.Add(txtTime);
            Controls.Add(lblCat);
            Controls.Add(txtAltInHg);
            Controls.Add(lblAlt);
            Controls.Add(txtVis);
            Controls.Add(lblVis);
            Controls.Add(txtSpeed);
            Controls.Add(label7);
            Controls.Add(txtDir);
            Controls.Add(lblWind);
            Controls.Add(txtDew);
            Controls.Add(label5);
            Controls.Add(lblTempDew);
            Controls.Add(txtTempC);
            Controls.Add(lblDateTime);
            Controls.Add(txtDate);
            Controls.Add(lblICAO);
            Controls.Add(txtID);
            Controls.Add(lblRaw);
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
        private Label lblRaw;
        private TextBox txtID;
        private Label lblICAO;
        private Label lblDateTime;
        private TextBox txtDate;
        private Label lblTempDew;
        private TextBox txtTempC;
        private Label label5;
        private TextBox txtDew;
        private Label lblWind;
        private TextBox txtSpeed;
        private Label label7;
        private TextBox txtDir;
        private TextBox txtVis;
        private Label lblVis;
        private TextBox txtAltInHg;
        private Label lblAlt;
        private Label lblCat;
        private TextBox txtTime;
        private Label label10;
        private TextBox txtSkyConditions;
        private Label lblSky;
        private Label lblVersion;
        private TextBox txtGusts;
        private Label label13;
        private Label lblElev;
        private TextBox txtElevFeet;
        private TextBox txtElevMeter;
        private Label label15;
        private Label label16;
        private TextBox txtAltQNH;
        private Label lblName;
        private TextBox txtName;
        private CheckBox checkBox1;
        private TextBox txtAtis;
        private Button btnArrivalAtis;
        private Button btnDepartureAtis;
        private TextBox txtLocation;
        private Label lblLocation;
    }
}