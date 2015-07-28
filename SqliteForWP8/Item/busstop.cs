using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BMTA
{
    public class busstop
    {
        [SQLite.PrimaryKey, SQLite.AutoIncrement]
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
