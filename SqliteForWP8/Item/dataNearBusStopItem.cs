using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMTA.Item
{
    public class dataNearBusStopItem
    {
        public string id { get; set; }
        public string stop_name { get; set; }
        public string stop_name_en { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string distance { get; set; }
        public string busline { get; set; }
    }
}
