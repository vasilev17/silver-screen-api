using System;
using System.Collections.Generic;

#nullable disable

namespace SilverScreen.Models.Tables
{
    public partial class MovieStaff
    {
        public int StaffId { get; set; }
        public int MoveId { get; set; }

        public virtual Movie Move { get; set; }
        public virtual staff Staff { get; set; }
    }
}
