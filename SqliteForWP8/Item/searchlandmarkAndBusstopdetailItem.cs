using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BMTA
{
    public class searchlandmarkAndBusstopdetailItem
    {
        public string status { get; set; }
        
        public string id { get; set; }

        public string stop_name { get; set; }

        public string stop_name_en { get; set; }

        public string name { get; set; }

        public string name_en { get; set; }

        public string lattitude { get; set; }

        public string longtitude { get; set; }

    }
}
