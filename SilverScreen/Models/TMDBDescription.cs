using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SilverScreen.Models
{
    public class TMDBDescription
    {
        public List<TMDBGenres> genres { get; set; }
        public int runtime { get; set; }
        public List<int> episode_run_time { get; set; }
        public bool adult { get; set; }

    }
}
