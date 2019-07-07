using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pet_WebService_Autotest
{
    class Pet
    {
        public long id { get; set; }
        public string name { get; set; }
        public string status { get; set; }
        public Category category { get; set; }
        public List<string> photoUrls { get; set; }
        public List<Tag> tags { get; set; }
    }
    class Category
    {
        public long id { get; set; }
        public string name { get; set; }
    }
    class Tag
    {
        public long id { get; set; }
        public string name { get; set; }
    }
}
