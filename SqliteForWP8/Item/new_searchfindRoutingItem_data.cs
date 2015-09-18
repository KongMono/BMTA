using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMTA.Item
{

    public class new_searchfindRoutingItem_data
    {
        public string segment { get; set; }
        public string distance { get; set; }
        public string price { get; set; }
        public string[][] polyline { get; set; }
        public List<new_searchfindRoutingItem_data_list> list { get; set; }
        public List<new_searchfindRoutingItem_routing> route { get; set; }
    }


}
