using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilverScreen.Models
{
    class IMDBMovie
    {
        public string id { get; set; }
        public string description { get; set; }
        public string image { get; set; }
        public string runtimeStr { get; set; }
        public string genres { get; set; }
        public string imDbRating { get; set; }
        public string plot { get; set; }
        public string stars { get; set; }

    }
}
