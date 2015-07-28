using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BMTA
{
    public class landmark
    {
        [SQLite.PrimaryKey, SQLite.AutoIncrement]
        public int id { get; set; }

        public string name { get; set; }

        public string name_en { get; set; }

        public string lattitude { get; set; }

        public string longtitude { get; set; }

        public int type { get; set; }

        public string modify_date { get; set; }

        public int published { get; set; }
    }
}
