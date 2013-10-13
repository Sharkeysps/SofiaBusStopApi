using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SumcApi.Models
{
    public class AllBussesModel
    {
        public List<BusNamesModel> Busses { get; set; }

        public AllBussesModel()
        {
            this.Busses = new List<BusNamesModel>();
            FillBusses();
        }

        private void FillBusses()
        {
            var intArray = new[]
            {
                1,3, 4, 5, 6, 8,
                9, 10, 11, 12, 14, 18, 20,
                22, 23, 24, 25, 26, 27, 28, 29, 30,
                31, 42, 44, 45, 45, 48, 45, 48, 49,
                54, 56, 59, 60, 63, 64, 65, 67, 69,
                70, 72, 73, 74, 75, 76, 77, 78, 79, 81,
                82, 83, 84, 85, 86, 87, 88,
                90, 93, 94, 98, 100, 101, 102, 107, 108, 111,
                113, 114, 117, 118, 119, 120, 122, 123, 150, 204,
                213, 260, 280, 285, 294, 305, 306, 309, 310, 314, 384, 404, 413
            };
            var list = new List<int>(intArray);
            foreach (var bus in list)
            {
                var busName = new BusNamesModel()
                {
                    Id = bus,
                    Name = "Автобус Номер: "+bus.ToString()
                };
                this.Busses.Add(busName);
            }
        }
    }
}