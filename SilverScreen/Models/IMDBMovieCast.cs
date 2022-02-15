using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SilverScreen.Models
{
    public class IMDBMovieCast
    {
        public IMDBScreenplayJobs directors { get; set; }
        public IMDBScreenplayJobs writers { get; set; }

        public List<IMDBActors> actors { get; set; }
    }
}
