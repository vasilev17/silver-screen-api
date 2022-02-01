using System;
using System.Collections.Generic;

#nullable disable

namespace SilverScreen.Models.Tables
{
    public partial class MovieGenre
    {
        public int GenreId { get; set; }
        public int MovieId { get; set; }

        public virtual Genre Genre { get; set; }
        public virtual Movie Movie { get; set; }
    }
}
