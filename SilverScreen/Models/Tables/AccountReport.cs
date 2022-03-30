using System;
using System.Collections.Generic;

#nullable disable

namespace SilverScreen.Models.Tables
{
    public partial class AccountReport
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int FakeReports { get; set; }
        public int Reports { get; set; }

        public virtual User User { get; set; }
    }
}
