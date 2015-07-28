using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace BMTA
{

    public sealed class buslineItem
    {
        public string lang = (Application.Current as App).Language;

        public int id { get; set; }

        public int status { get; set; }

        public string bus_name { get; set; }

        public string bus_name_en { get; set; }

        public string bus_line { get; set; }

        public string bus_owner { get; set; }

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

        public string published { get; set; }

        public string bus_start_final 
        {
            get
            {
                if (lang.Equals("th"))
                {
                    return bus_start;
                }
                else
                {
                    return bus_start_en;
                }
                
            }
            set
            {
                this.bus_start_final = value;
            }
        }

        public string bus_stop_final
        {
            get
            {
                if (lang.Equals("th"))
                {
                    return bus_stop;
                }
                else
                {
                    return bus_stop_en;
                }

            }
            set
            {
                this.bus_stop_final = value;
            }
        }

        public string bus_line_bg
        {
            get
            {
                switch (this.bus_color)
                {
                    case 1:
                        return "Assets/btn_normal1.png";
                       
                    case 2:
                        return "Assets/btn_normal2.png";
                        
                    case 3:
                        return "Assets/btn_normal3.png";
                      
                    case 4:
                        return "Assets/btn_normal4.png";
                        
                    case 5:
                        return "Assets/btn_normal5.png";
                     
                    case 10:
                        return "Assets/btn_normal10.png";
                      
                    case 14:
                        return "Assets/btn_normal14.png";
                       
                    case 99:
                        return "Assets/btn_normal99.png";
                        
                    default:
                        return "Assets/btn_normal99.png";
                }
            }
            set
            {
                this.bus_line_bg = value;
            }
        }
    }
}
