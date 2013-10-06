using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SumcApi.Helpers
{
    public class SearchParameters
    {
        public string Bus { get; set; }
        public string BusId { get; set; }

        public string DirectionOne { get; set; }
        public string DirectionTwo { get; set; }

        public string DirectionOneId { get; set; }
        public string DirectionTwoId { get; set; }

        public Dictionary<string, string> DirectionOneStops { get; set; }
        public Dictionary<string, string> DirectionTwoStops { get; set; }

        public List<GPSCoordinates> Coordinates { get; set; }

        public SearchParameters()
        {
            this.DirectionOneStops = new Dictionary<string, string>();
            this.DirectionTwoStops = new Dictionary<string, string>();
            this.Coordinates = new List<GPSCoordinates>();
        }

        public override string ToString()
        {
            var returnString = string.Format("\n Bus-{0} \n BusId-{1} \n DirectionOne-{2} \n DirectionTwo-{3} \n DirectionOneId-{4} \n DirectionTwoId-{5}",
                this.Bus, this.BusId, this.DirectionOne,
                this.DirectionTwo, this.DirectionOneId, this.DirectionTwoId);
            return returnString;
        }
    }
}