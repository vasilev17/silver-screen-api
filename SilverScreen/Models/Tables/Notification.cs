using System;
using System.Collections.Generic;

#nullable disable

namespace SilverScreen.Models.Tables
{
    public partial class Notification
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int AuthorId { get; set; }
        public string Type { get; set; }
        public string Content { get; set; }
        public int? MovieId { get; set; }
        public bool? Active { get; set; }

        public virtual User Author { get; set; }
        public virtual Movie Movie { get; set; }
        public virtual User User { get; set; }
    }
}
