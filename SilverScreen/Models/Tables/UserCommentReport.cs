using System;
using System.Collections.Generic;

#nullable disable

namespace SilverScreen.Models.Tables
{
    public partial class UserCommentReport
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ReportId { get; set; }

        public virtual CommentReport Report { get; set; }
        public virtual User User { get; set; }
    }
}
