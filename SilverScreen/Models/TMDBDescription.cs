using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SilverScreen.Models
{
    public class TMDBDescription
    {
        public List<TMDBGenres> genres { get; set; }
        public string runtime { get; set; }
       
    }
}
