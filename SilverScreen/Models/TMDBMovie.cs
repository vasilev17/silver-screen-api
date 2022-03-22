
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SilverScreen.Models
{
    class TMDBMovie
    {
        public int id { get; set; }
        public bool adult { get; set; }
        public string title { get; set; }
        public string poster_path { get; set; }
        public double vote_average { get; set; } 
        public string overview { get; set; } 
        public string release_date { get; set; }
        public string backdrop_path { get; set; }

        
    }
}
