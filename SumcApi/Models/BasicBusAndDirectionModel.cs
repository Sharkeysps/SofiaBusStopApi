using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace SumcApi.Models
{
    public class BasicBusAndDirectionModel
    {
        public string DirectionOneName { get; set; }
        public string DirectionTwoName { get; set; }

        public BasicBusAndDirectionModel(string busName)
        {
            GetBusDirectionNames(busName);
        }

        private void GetBusDirectionNames(string busName)
        {
            try
            {
                var baseUrl = "http://m.sofiatraffic.bg/schedules?tt=1&ln=" + busName + "&s=%D0%A2%D1%8A%D1%80%D1%81%D0%B5%D0%BD%D0%B5";
                WebRequest req = HttpWebRequest.Create(baseUrl);
                req.Method = "GET";

                string source;
                using (StreamReader reader = new StreamReader(req.GetResponse().GetResponseStream()))
                {
                    source = reader.ReadToEnd();
                }


                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(source);

                var directions = doc.DocumentNode.SelectNodes("//div[@class='info']");
                var counter = 0;

                foreach (var direction in directions)
                {

                    if (counter == 0)
                    {
                        this.DirectionOneName = direction.InnerText.Trim();
                    }
                    if (counter == 1)
                    {
                        this.DirectionTwoName = direction.InnerText.Trim();
                    }
                    counter++;

                }
            }
            catch (Exception ex)
            {
            }
        }
    }
}