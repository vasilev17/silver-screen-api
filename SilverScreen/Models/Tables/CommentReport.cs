using System;
using System.Collections.Generic;

#nullable disable

namespace SilverScreen.Models.Tables
{
    public partial class CommentReport
    {
        public CommentReport()
        {
            UserCommentReports = new HashSet<UserCommentReport>();
        }

        public int Id { get; set; }
        public int UserId { get; set; }
        public int CommentId { get; set; }
        public string Contents { get; set; }
        public int? UnderReview { get; set; }
        public bool ReportedForFalsePositive { get; set; }
        public bool ReportIsLegit { get; set; }

        public virtual User UnderReviewNavigation { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<UserCommentReport> UserCommentReports { get; set; }
    }
}
