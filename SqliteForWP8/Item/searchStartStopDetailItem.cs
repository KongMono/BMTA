using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMTA.Item
{
    class searchStartStopDetailItem
    {
        public string id { get;set; }

        public string stop_name { get; set; }

        public string stop_name_en { get; set; }

        public double longitude { get; set; }

        public double latitude { get; set; }

        public double distance { get; set; }

        public double busline { get; set; }
    }
}
