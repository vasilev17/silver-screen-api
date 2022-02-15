using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SilverScreen.Models
{
    public class IMDBScreenplayJobs
    {
        public string job { get; set; }
        public List<IMDBActors> items { get; set; }
    }
}
