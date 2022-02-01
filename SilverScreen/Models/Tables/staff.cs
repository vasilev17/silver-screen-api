using System;
using System.Collections.Generic;

#nullable disable

namespace SilverScreen.Models.Tables
{
    public partial class staff
    {
        public staff()
        {
            MovieStaffs = new HashSet<MovieStaff>();
        }

        public int Id { get; set; }
        public int MovieId { get; set; }
        public string Name { get; set; }
        public string Position { get; set; }

        public virtual Movie Movie { get; set; }
        public virtual ICollection<MovieStaff> MovieStaffs { get; set; }
    }
}
