using System;
using System.Collections.Generic;

#nullable disable

namespace SilverScreen.Models.Tables
{
    public partial class Genre
    {
        public Genre()
        {
            MovieGenres = new HashSet<MovieGenre>();
        }

        public int Id { get; set; }
        public int MovieId { get; set; }
        public string Genre1 { get; set; }

        public virtual Movie Movie { get; set; }
        public virtual ICollection<MovieGenre> MovieGenres { get; set; }
    }
}
