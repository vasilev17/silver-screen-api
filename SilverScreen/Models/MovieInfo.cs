using SilverScreen.Models.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SilverScreen.Models
{
    public class MovieInfo
    {
        public Movie Movie { get; set; }
        public List<string> Genres { get; set; }
        public List<staff> Staff { get; set; }




    }
}
