using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetarLookup
{
    internal class Metar
    {


        public string rawText { get; set; }
        public string observationTime { get; set; }
        public string stationID { get; set; }
        public string tempC { get; set; }
        public string dewpointC { get; set; }
        public string windDir { get; set; }
        public string windSpeedKt { get; set; }
        public string windGustsKt { get; set; }
        public string visibility { get; set; }
        public string altimeter { get; set; }
        public string flightCat { get; set; }
        public string elevationMeter { get; set; }
        public List<SkyCondition> skyCondition { get; set; }
    }
    internal class SkyCondition
    {
        public string skyCover { get; set; }
        public string cloudBase { get; set; }
    }
}