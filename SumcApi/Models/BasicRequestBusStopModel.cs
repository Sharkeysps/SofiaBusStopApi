using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SumcApi.Models
{
    public class BasicRequestBusStopModel
    {
        public string Bus { get; set; }
        public int Direction { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}