using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace BMTA
{

    public class insert_landmark_detailItem
    {
        public int id { get; set; }

        public string busname { get; set; }

        public string name { get; set; }

        public string name_en { get; set; }

        public string latitude { get; set; }

        public string longitude { get; set; }

        public int type { get; set; }

        public string modify_date { get; set; }

        public int published { get; set; }
    }
}
