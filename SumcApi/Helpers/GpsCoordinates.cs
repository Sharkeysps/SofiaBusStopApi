using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SumcApi.Helpers
{
    public class GPSCoordinates
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public string BusStopName { get; set; }
        public string BusStopCode { get; set; }

        public double Distance { get; set; }
    }
}