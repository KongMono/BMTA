using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace BMTA
{

    public class busline
    {
        [SQLite.PrimaryKey]
        public int id { get; set; }

        public string bus_name { get; set; }

        public string bus_name_en { get; set; }

        public string bus_line { get; set; }

        public int bus_owner { get; set; }

        public string bustype { get; set; }

        public int bus_running { get; set; }

        public int bus_color { get; set; }

        public string bus_description { get; set; }

        public string bus_startstop_time { get; set; }

        public string busstop_list { get; set; }

        public string bus_polyline { get; set; }
        
        public string bus_start { get; set; }

        public string bus_start_en { get; set; }

        public string bus_stop { get; set; }

        public string bus_stop_en { get; set; }

        public string bus_direction { get; set; }

        public string bus_direction_en { get; set; }

        public string modify_date { get; set; }

        public int published { get; set; }

    }
}
