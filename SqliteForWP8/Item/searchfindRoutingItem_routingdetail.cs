using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMTA.Item
{
    class searchfindRoutingItem_routingdetail 
    {       public string busline_id { get; set; }
            public string busline_name { get; set; }
            public string bus_line { get; set; }
            public string bus_color { get; set; }
            public string bustype { get; set; }
            public string bus_owner { get; set; }
            public string bus_running { get; set; }
            public string segment { get; set; }
            public string bustation_id_start { get; set; }
            public string bustation_id_end { get; set; }
            public string busline_coordinates { get; set; }
            public string[][] bus_polyline { get; set; }
            public string type { get; set; }
            public string route_type { get; set; }
            public string exchange_point { get; set; }
            public List<searchfindRoutingItem_routingdetail_busstop> busstop { get; set; }
            public string distance { get; set; }
            public string price { get; set; }
           
    }
}
