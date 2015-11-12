using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BMTA
{
    public class news
    {
        [SQLite.PrimaryKey]
        public int id { get; set; }

        public string title { get; set; }

        public string description { get; set; }

        public string image { get; set; }

        public string createdate { get; set; }

        public string type { get; set; }

        public string read { get; set; }
    }
}
