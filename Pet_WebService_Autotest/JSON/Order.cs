using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pet_WebService_Autotest
{
    class Order
    {
        public long id { get; set; }
        public long petId { get; set; }
        public long quantity { get; set; }
        public string shipDate { get; set; }
        public string status { get; set; }
        public string complete { get; set; }
    }
}
