using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMTA.Item
{
    public class FeedItem
    {
        public string title { get; set; }
        public string description { get; set; }
        public string link { get; set; }
        public string atomlink { get; set; }
        public List<FeedItemDescription> item { get; set; }
    }
}
