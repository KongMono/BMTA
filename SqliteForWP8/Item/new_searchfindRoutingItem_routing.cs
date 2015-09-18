using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMTA.Item
{
    public class new_searchfindRoutingItem_routing
    {
        public string busid { get; set; }
        public string busline { get; set; }
        public string bustype { get; set; }
        public new_searchfindRoutingItem_routing_busstop busstop { get; set; }
        public List<new_searchfindRoutingItem_routing_change> change { get; set; }
        public string distance { get; set; }
        public string price { get; set; }
        public string step { get; set; }
    }
}
