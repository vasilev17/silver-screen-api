using System;
using System.Collections.Generic;

#nullable disable

namespace SilverScreen.Models.Tables
{
    public partial class Comment
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int MovieId { get; set; }
        public string Content { get; set; }

        public virtual Movie Movie { get; set; }
        public virtual User User { get; set; }
    }
}
