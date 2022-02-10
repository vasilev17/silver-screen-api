using System;
using System.Collections.Generic;

#nullable disable

namespace SilverScreen.Models.Tables
{
    public partial class MovieStaff
    {
        public int StaffId { get; set; }
        public int MovieId { get; set; }

        public virtual Movie Movie { get; set; }
        public virtual staff Staff { get; set; }
    }
}
