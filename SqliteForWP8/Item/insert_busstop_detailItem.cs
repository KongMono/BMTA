using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace BMTA
{
    
    public class insert_busstop_detailItem
    {
        public int id { get; set; }

        public string stop_name { get; set; }

        public string stop_name_en { get; set; }

        public string stop_description { get; set; }

        public string latitude { get; set; }

        public string longitude { get; set; }

        public string modify_date { get; set; }

        public int status { get; set; }
    }
}
