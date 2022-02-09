using SilverScreen.Models.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilverScreen.Models
{
    class IMDBQuery
    {
        public string queryString { get; set; }
        public List<IMDBMovie> results { get; set; }
        public string errorMessage { get; set; }
    }
}
