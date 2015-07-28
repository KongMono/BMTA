using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace BMTA
{
    public class insert_landmarkItem
    {
        public int status { get; set; }
        public List<insert_landmark_detailItem> data { get; set; }
    }
}
