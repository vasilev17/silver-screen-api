using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SilverScreen.Models
{
    public class TMDBCast
    {
        public List<TMDBFullCast> cast { get; set; }
        public List<TMDBFullCast> crew { get; set; }
    }
}
