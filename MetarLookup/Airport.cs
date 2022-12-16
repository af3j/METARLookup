

namespace MetarLookup
{
    internal class Airport
    {
        public int id { get; set; }
        public string iata { get; set; }
        public string icao { get; set; }
        public string name { get; set; }
        public string location { get; set; }
        public string street_number { get; set; }
        public string street { get; set; }
        public string city { get; set; }
        public string county { get; set; }
        public string state { get; set; }
        public string country_iso { get; set; }
        public string country { get; set; }
        public string postal_code { get; set; }
        public string phone { get; set; }
        public float latitude { get; set; }
        public float longitude { get; set; }
        public int uct { get; set; }
        public string website { get; set; }
    }
}
