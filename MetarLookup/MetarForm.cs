using Newtonsoft.Json;
using System.Collections.Specialized;
using System.Configuration;
using System.Globalization;
using System.Windows.Forms;
using System.Xml;


namespace MetarLookup
{

    public partial class MetarForm : Form
    {
        string arrivalAtis;
        string departureAtis;
        public MetarForm()
        {
            InitializeComponent();
            CultureInfo culture = new CultureInfo("en-US");
            CultureInfo.CurrentCulture = culture;
            if (ConfigurationManager.AppSettings["DarkThemeYN"] == "Y")
            {
                checkBox1.Checked = true;
            }
            else
            {
                checkBox1.Checked = false;
            }
        }
        


        static async Task<Metar> GetMetarAsync(string airportCode)
        {
            // Use the HttpClient class to send an HTTP request to the NOAA Aviation Weather Center's web service
            using (HttpClient client = new HttpClient())
            {
                Metar metar = new Metar();
                metar.skyCondition = new List<SkyCondition>();
                // Set the user agent and referrer headers
                client.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/83.0.4103.61 Safari/537.36");
                client.DefaultRequestHeaders.Add("referer", "https://www.aviationweather.gov/");

                // Send a GET request to the web service's API endpoint
                string url = "https://aviationweather.gov/cgi-bin/data/dataserver.php?requestType=retrieve&dataSource=metars&stationString=" + airportCode + " &hoursBeforeNow=3&format=xml&mostRecent=true";
                HttpResponseMessage response = await client.GetAsync(url).ConfigureAwait(false);

                // Read the response as a string
                string responseString = await response.Content.ReadAsStringAsync();

                // Parse the XML response to extract the METAR weather report
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(responseString);
                if (doc.DocumentElement.SelectNodes("/response/data/METAR") != null)
                {
                    XmlNodeList nodes = doc.DocumentElement.SelectNodes("/response/data/METAR");
                    XmlNodeList skyConditions = doc.DocumentElement.SelectNodes("/response/data/METAR/sky_condition");
                    foreach (XmlNode node in nodes)
                    {

                        metar.rawText = node.SelectSingleNode("raw_text").InnerText.ToString();
                        metar.observationTime = node.SelectSingleNode("observation_time").InnerText.ToString();
                        metar.stationID = node.SelectSingleNode("station_id").InnerText.ToString();
                        metar.tempC = node.SelectSingleNode("temp_c").InnerText.ToString();
                        metar.dewpointC = node.SelectSingleNode("dewpoint_c").InnerText.ToString();
                        if (node.SelectSingleNode("wind_dir_degrees") != null)
                        {
                            metar.windDir = node.SelectSingleNode("wind_dir_degrees").InnerText.ToString() + "�";
                        }
                        if (node.SelectSingleNode("wind_speed_kt") != null)
                        {
                            metar.windSpeedKt = node.SelectSingleNode("wind_speed_kt").InnerText.ToString();
                        }
                        if (node.SelectSingleNode("wind_gust_kt") != null)
                        {
                            metar.windGustsKt = node.SelectSingleNode("wind_gust_kt").InnerText.ToString();
                        }
                        else
                        {
                            metar.windGustsKt = "0";
                        }
                        metar.visibility = node.SelectSingleNode("visibility_statute_mi").InnerText.ToString();
                        metar.flightCat = node.SelectSingleNode("flight_category").InnerText.ToString();
                        metar.elevationMeter = node.SelectSingleNode("elevation_m").InnerText.ToString();

                        metar.altimeter = node.SelectSingleNode("altim_in_hg").InnerText.ToString();






                        foreach (XmlElement node2 in skyConditions)
                        {
                            SkyCondition condition = new SkyCondition();

                            condition.skyCover = node2.GetAttribute("sky_cover").ToString();
                            condition.cloudBase = node2.GetAttribute("cloud_base_ft_agl").ToString();
                            metar.skyCondition.Add(condition);
                        }


                    }
                }
                return metar;
            }
        }




        private async void btnGetMetar_Click_1(object sender, EventArgs e)
        {
            clearBoxes();
            Metar metar = new Metar();
            // Get the airport code entered by the user
            string airportCode = txtAirportCode.Text;

            // Get the METAR weather report for the specified airport code
            try
            {
                metar = GetMetarAsync(airportCode).Result;
            }
            catch (Exception ex)
            {
            }
            try
            {
                await getAirportData(airportCode);
            }
            catch (Exception ex)
            {
            }
            try
            {
                await getAirportATIS(airportCode);
            }
            catch
            {

            }

            // Display the METAR weather report
            if (metar.rawText != null)
            {
                txtMetarReport.Text = metar.rawText;
                if (metar.observationTime != null)
                {
                    txtDate.Text = " " + DateTime.ParseExact(metar.observationTime, "yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                    txtTime.Text = " " + DateTime.ParseExact(metar.observationTime, "yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture).ToUniversalTime().ToString("HH:mm:ss") + " Z";
                }
                txtTempC.Text = " " + metar.tempC + " C";
                txtDew.Text = " " + metar.dewpointC + " C"; ;
                txtDir.Text = " " + metar.windDir;
                txtSpeed.Text = " " + metar.windSpeedKt + " kt";
                txtGusts.Text = " " + metar.windGustsKt + " kt";
                txtVis.Text = " " + metar.visibility + " sm";
                txtAltInHg.Text = " " + Math.Round(Convert.ToDecimal(metar.altimeter), 2).ToString() + " inHg";
                txtAltQNH.Text = " " + Math.Round((Convert.ToDecimal(metar.altimeter) * Convert.ToDecimal(33.8639)), 0).ToString() + " QNH";
                txtElevMeter.Text = " " + metar.elevationMeter + " m";
                txtElevFeet.Text = " " + Math.Round((Convert.ToDecimal(metar.elevationMeter) * Convert.ToDecimal(3.28084)), 0).ToString() + " ft";
                if (metar.skyCondition != null)
                {
                    foreach (SkyCondition condition in metar.skyCondition)
                    {
                        if (condition.skyCover != "CLR" && condition.skyCover != "CAVOK")
                        {
                            txtSkyConditions.AppendText(condition.skyCover + " at " + condition.cloudBase + System.Environment.NewLine);
                        }
                        else
                        {
                            txtSkyConditions.AppendText(condition.skyCover + System.Environment.NewLine);
                        }
                    }
                }

                lblCat.Text = metar.flightCat;
                lblCat.Visible = true;
                if (metar.flightCat == "VFR")
                {
                    lblCat.ForeColor = System.Drawing.Color.Green;
                }
                else if (metar.flightCat == "MVFR")
                {
                    lblCat.ForeColor = System.Drawing.Color.Orange;
                }
                else
                {
                    lblCat.ForeColor = System.Drawing.Color.Red;
                }
            }
            else
            {
                txtMetarReport.Text = "METAR is not available";
            }
        }


        public async Task getAirportData(string code)
        {
            var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(3);
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("https://www.airport-data.com/api/ap_info.json?icao=" + code),

            };
            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                Airport airport = JsonConvert.DeserializeObject<Airport>(body);
                string airportName = airport.name;
                string airportLocation = airport.location;
                if (airportName == null)
                {
                    airportName = "Not Available";
                }
                if (airportLocation == null)
                {
                    airportLocation = "Not Available";
                }
                txtName.Text = " " + airportName;
                txtLocation.Text = " " + airportLocation;
                txtID.Text = " " + airport.icao;
            }

        }


        //public async Task getAirportData(string code)
        //{
        //    var client = new HttpClient();
        //    client.Timeout = TimeSpan.FromSeconds(5);
        //    var request = new HttpRequestMessage
        //    {
        //        Method = HttpMethod.Get,
        //        RequestUri = new Uri("https://airport-info.p.rapidapi.com/airport?icao=" + code),
        //        Headers =
        //        {
        //            { "X-RapidAPI-Key", "" },
        //            { "X-RapidAPI-Host", "airport-info.p.rapidapi.com" },
        //        },
        //    };
        //    using (var response = await client.SendAsync(request))
        //    {
        //        response.EnsureSuccessStatusCode();
        //        var body = await response.Content.ReadAsStringAsync();
        //        Airport airport = JsonConvert.DeserializeObject<Airport>(body);
        //        string airportName = airport.name;
        //        string airportLocation = airport.location;
        //        if (airportName == null)
        //        {
        //            airportName = "Not Available";
        //        }
        //        if (airportLocation == null)
        //        {
        //            airportLocation = "Not Available";
        //        }
        //        txtName.Text = " " + airportName;
        //        txtLocation.Text = " " + airportLocation;
        //        txtID.Text = " " + airport.icao;
        //    }

        //}


        public async Task getAirportATIS(string code)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("https://datis.clowd.io/api/" + code),
            };
            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                if (body.Contains("error"))
                {
                    arrivalAtis = "ATIS is not available";
                    departureAtis = "ATIS is not available";
                }
                else
                {
                    ATIS[] atis = JsonConvert.DeserializeObject<ATIS[]>(body);

                    if (atis[0].type == "arr")
                    {
                        arrivalAtis = atis[0].datis;
                    }
                    else if (atis[0].type == "dep")
                    {
                        departureAtis = atis[0].datis;
                    }
                    else if (atis[0].type == "combined")
                    {
                        arrivalAtis = "***COMBINED ATIS***" + Environment.NewLine + atis[0].datis;
                        departureAtis = "***COMBINED ATIS***" + Environment.NewLine + atis[0].datis;
                        txtAtis.Text = arrivalAtis;
                    }
                    if (atis[1].type == "arr")
                    {
                        arrivalAtis = atis[1].datis;
                    }
                    else if (atis[1].type == "dep")
                    {
                        departureAtis = atis[1].datis;
                    }
                }

            }
            txtAtis.Text = arrivalAtis;
            btnArrivalAtis.Select();
        }





        public void clearBoxes()
        {
            foreach (Control textBox in Controls)
            {
                if ((textBox.GetType().ToString() == "System.Windows.Forms.TextBox") && textBox.Name != "txtAirportCode")
                {
                    if (!textBox.Name.Contains("lbl"))
                    {
                        textBox.Text = "";
                    }
                }
             }



        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                UpdateSetting("DarkThemeYN", "Y");
                ConfigurationManager.AppSettings["DarkThemeYN"] = "Y";
                this.BackColor = Color.FromArgb(64, 64, 64);
                this.ForeColor = Color.White;

                foreach (Control control in this.Controls)
                {
                    if (control is Button)
                    {
                        Button button = (Button)control;
                        button.BackColor = Color.Black;
                        button.ForeColor = Color.White;
                        
                    }
                    if (control is TextBox)
                    {
                        TextBox textBox = (TextBox)control;
                        textBox.BackColor = Color.DarkGray;
                        textBox.ForeColor = Color.Black;
                    }

                }

            }
            else
            {
                UpdateSetting("DarkThemeYN", "N");
                this.BackColor = SystemColors.Control;
                this.ForeColor = Color.Black;
                foreach (Control control in this.Controls)
                {
                    if (control is Button)
                    {
                        Button button = (Button)control;
                        button.BackColor = Color.LightGray;
                        button.ForeColor = Color.Black;
                        
                    }
                    if (control is TextBox)
                    {
                        TextBox textBox = (TextBox)control;
                        textBox.BackColor = Color.FromArgb(227, 227, 227);
                        textBox.ForeColor = Color.Black;
                    }
                    if (control.Name == "txtAirportCode")
                    {
                        TextBox textBox = (TextBox)control;
                        textBox.BackColor = Color.White;
                        textBox.ForeColor = Color.Black;
                    }
                }

            }
        }
        private static void UpdateSetting(string key, string value)
        {
            Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            configuration.AppSettings.Settings[key].Value = value;
            configuration.Save();

            ConfigurationManager.RefreshSection("appSettings");
        }
        private void btnArrivalAtis_Click(object sender, EventArgs e)
        {
            txtAtis.Clear();
            txtAtis.Text = arrivalAtis;
        }

        private void btnDepartureAtis_Click(object sender, EventArgs e)
        {
            txtAtis.Clear();
            txtAtis.Text = departureAtis;
        }

        private void label18_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
        }
    }



}
