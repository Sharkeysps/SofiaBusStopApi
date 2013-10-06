using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SumcApi.Helpers;
using HtmlAgilityPack;
using System.Net;
using System.IO;
using System.Xml;
namespace SumcApi.Models
{
    public class DetailedBusInfoModel
    {

        public string Bus { get; set; }
        public string BusStopName { get; set; }
        public double DistanceToStop { get; set; }
        public string Schedule { get; set; }

        public double BusStopLatitude { get; set; }
        public double BusStopLongitude { get; set; }

        public DetailedBusInfoModel(string bus,double lat,double lon,int direction)
        {
            
            FillSearchParameter(bus,lat,lon,direction);
        }


        private void XMLChecker(SearchParameters parameter, int direction)
        {
            XmlDocument xd = new XmlDocument();
            //xd.Load("..\\..\\Xml\\StationCoordinates.xml");
            xd.Load("https://sofia-public-transport-navigator.googlecode.com/hg/res/raw/coordinates.xml");
            foreach (XmlElement emp in xd.SelectNodes("/stations/station"))
            {
                if (direction == 1)
                {
                    if (parameter.DirectionOneStops.ContainsValue(emp.GetAttribute("code").ToString()))
                    {
                        var coordinate = new GPSCoordinates();
                        coordinate.BusStopCode = emp.GetAttribute("code").Trim();
                        coordinate.BusStopName = emp.GetAttribute("label").Trim();
                        coordinate.Latitude = double.Parse(emp.GetAttribute("lat"));
                        coordinate.Longitude = double.Parse(emp.GetAttribute("lon"));

                        parameter.Coordinates.Add(coordinate);

                        var t = 5;
                    }
                }

                if (direction == 2)
                {
                    if (parameter.DirectionTwoStops.ContainsValue(emp.GetAttribute("code").ToString()))
                    {
                        var coordinate = new GPSCoordinates();
                        coordinate.BusStopCode = emp.GetAttribute("code").Trim();
                        coordinate.BusStopName = emp.GetAttribute("label").Trim();
                        coordinate.Latitude = double.Parse(emp.GetAttribute("lat"));
                        coordinate.Longitude = double.Parse(emp.GetAttribute("lon"));

                        parameter.Coordinates.Add(coordinate);

                        var t = 5;
                    }

                }
            }
        }

        private  void FillSearchParameter(string searchedBus, double lat, double lon, int direction)
        {
            if (direction != 1 && direction != 2)
            {
                direction = 1;
            }

            var baseUrl = "http://m.sofiatraffic.bg/schedules?tt=1&ln=" + searchedBus + "&s=%D0%A2%D1%8A%D1%80%D1%81%D0%B5%D0%BD%D0%B5";
            WebRequest req = HttpWebRequest.Create(baseUrl);
            req.Method = "GET";

            string source;
            using (StreamReader reader = new StreamReader(req.GetResponse().GetResponseStream()))
            {
                source = reader.ReadToEnd();
            }

            var parameter = new SearchParameters();
            parameter.Bus = searchedBus;
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(source);

            GetDirectionNames(parameter, doc);
            GetDirectionsId(parameter, doc);
            GetAllStops(parameter, doc);

            XMLChecker(parameter, direction);



            var returnCoordinates = NearestBusStop(parameter, lat, lon);

            var busId = parameter.BusId;
            var directionId = parameter.DirectionOneId;
            var stopId = "";
            if (direction == 1)
            {
                stopId = parameter.DirectionOneStops.Where(x => x.Value == returnCoordinates.BusStopCode).FirstOrDefault().Key;
            }
            if (direction == 2)
            {
                stopId = parameter.DirectionTwoStops.Where(x => x.Value == returnCoordinates.BusStopCode).FirstOrDefault().Key;
            }

            var searchParameter = "/schedules/vehicle-vt?s=" + stopId + "&lid=" + busId + "&vt=1&rid=" + directionId;

            this.Bus = parameter.Bus;
            this.BusStopName = returnCoordinates.BusStopName;
            this.DistanceToStop = returnCoordinates.Distance;
            this.BusStopLatitude = returnCoordinates.Latitude;
            this.BusStopLongitude = returnCoordinates.Longitude;
         
            PrintSchedule(searchParameter);

        }

        private static GPSCoordinates NearestBusStop(SearchParameters parameter, double latitude, double longitude)
        {
            var currentMinDistance = 9999999.0d;
            var returnGPSCoordinates = new GPSCoordinates();

            foreach (var stops in parameter.Coordinates)
            {
                var check = DistanceAlgorithm.DistanceBetweenPlaces(stops.Longitude,
                    stops.Latitude, longitude, latitude);



                if (check < currentMinDistance)
                {
                    returnGPSCoordinates.BusStopCode = stops.BusStopCode;
                    returnGPSCoordinates.BusStopName = stops.BusStopName;
                    returnGPSCoordinates.Distance = check * 1000;
                    returnGPSCoordinates.Latitude = stops.Latitude;
                    returnGPSCoordinates.Longitude = stops.Longitude;
                    currentMinDistance = check;
                }
            }
            return returnGPSCoordinates;

        }

        private  void GetAllStops(SearchParameters parameter, HtmlDocument doc)
        {
            var stops = doc.DocumentNode.SelectNodes("//table//select");

            for (int i = 0; i < stops.Count; i++)
            {
                var search = new HtmlDocument();
                search.LoadHtml(stops[i].OuterHtml);

                var nodes = search.DocumentNode.SelectNodes("//select//option");

                foreach (var node in nodes)
                {
                    var searchedString = node.NextSibling.InnerText.Trim().IndexOf('(');
                    var searchedStringTwo = node.NextSibling.InnerText.Trim().IndexOf(')');
                    var code = node.NextSibling.InnerText.Trim().Substring(searchedString + 1, searchedStringTwo - searchedString - 1);
                    if (i == 0)
                    {
                        parameter.DirectionOneStops.Add(node.Id.Trim(), code);

                    }

                    if (i == 1)
                    {
                        parameter.DirectionTwoStops.Add(node.Id.Trim(), code);
                    }
                }

            }

        }

        private  void GetDirectionsId(SearchParameters parameter, HtmlDocument doc)
        {

            var inputs = doc.DocumentNode.SelectNodes("//input");
            foreach (var input in inputs)
            {
                if (input.Attributes["name"].Value == "lid")
                {
                    if (parameter.BusId == null)
                    {
                        parameter.BusId = input.Attributes["value"].Value;
                    }
                }

                if (input.Attributes["name"].Value == "rid")
                {
                    if (parameter.DirectionOneId == null)
                    {
                        parameter.DirectionOneId = input.Attributes["value"].Value;
                    }
                    else
                    {
                        parameter.DirectionTwoId = input.Attributes["value"].Value;
                    }

                }

            }
        }

        private void GetDirectionNames(SearchParameters parameter, HtmlDocument doc)
        {
            var directions = doc.DocumentNode.SelectNodes("//div[@class='info']");
            var counter = 0;

            foreach (var direction in directions)
            {

                if (counter == 0)
                {
                    parameter.DirectionOne = direction.InnerText.Trim();
                }
                if (counter == 1)
                {
                    parameter.DirectionTwo = direction.InnerText.Trim();
                }
                counter++;

            }
        }

        public  void PrintSchedule(string attribute)
        {
            var baseURl = "http://m.sofiatraffic.bg";
            HtmlDocument doc = new HtmlWeb().Load(baseURl + attribute);


            var schedules = doc.DocumentNode.SelectNodes("//div[@class='cnt']//b");

            var counter = 0;
            foreach (var schedule in schedules)
            {
                if (counter == 1)
                {
                    this.Schedule = schedule.InnerHtml.Trim();
                    if(string.IsNullOrEmpty(schedule.InnerHtml.Trim()))
                    {
                        this.Schedule = "Няма автобуси по това време";
                    }
                   
                   // Console.WriteLine(schedule.InnerHtml.Trim());
                    counter++;
                }
                if (schedule.InnerHtml.Trim() == "Направлениe:")
                {
                    counter++;
                }

            }
        }
    }
}