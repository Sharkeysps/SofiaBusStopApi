using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SumcApi.Models;
using Newtonsoft.Json;
namespace SumcApi.Controllers
{
    public class BusController : ApiController
    {
        // GET api/bus
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        //[HttpGet]
        //[ActionName("directions")]
        public BasicBusAndDirectionModel GetBus(int id)
        {
            var busStopNames = new BasicBusAndDirectionModel(id.ToString());
            //var returnString = JsonConvert.SerializeObject(busNames);
            return busStopNames;
        }

        // POST api/bus
        public DetailedBusInfoModel Post([FromBody]BasicRequestBusStopModel value)
        {
            var busInfo = new DetailedBusInfoModel(value.Bus, value.Latitude, value.Longitude, value.Direction);
           // var serialisedInfo = JsonConvert.SerializeObject(busInfo);
            return busInfo;
        }

        // PUT api/bus/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/bus/5
        public void Delete(int id)
        {
        }
    }
}
