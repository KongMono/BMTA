using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BMTA.Item
{
    class searchStartStopDetailItem
    {
        public string lang = (Application.Current as App).Language;

        public string id { get; set; }

        public string name { get; set; }

        public string name_en { get; set; }

        public string ref_name { get; set; }

        public string stop_name { get; set; }

        public string stop_name_en { get; set; }

        public string lattitude { get; set; }

        public string longtitude { get; set; }

        public string name_final
        {
            get
            {
                if (lang.Equals("th"))
                {
                    return name;
                }
                else
                {
                    return name_en;
                }

            }
            set
            {
                this.name_final = value;
            }
        }

    }
}
