using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BMTA
{
    public sealed class buslineItem
    {
        public int id { get; set; }

        public string bus_name { get; set; }

        public string bus_line { get; set; }

        public string bustype { get; set; }

        public string bus_description { get; set; }

        public string bus_start { get; set; }

        public string bus_end { get; set; }

        public string bus_startstop_time { get; set; }

        public string bus_polyline { get; set; }

        public string bus_stop { get; set; }

        public string zone { get; set; }

        public string important_location { get; set; }

        public string bus_color { get; set; }

        public string bus_running { get; set; }

        public string bus_id { get; set; }

        public string bus_direction { get; set; }

        public string bus_direction_en { get; set; }

        public string bus_name_en { get; set; }

        public string bus_start_en { get; set; }

        public string bus_stop_en { get; set; }

        public string bus_owner { get; set; }

        public string bus_line_bg
        {
            get
            {
                return "Assets/btn_normal"+ this.bus_color +".png";
            }
            set
            {
                this.bus_line_bg = value;
            }
        }
    }
}
