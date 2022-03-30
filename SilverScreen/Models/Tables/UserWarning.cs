using System;
using System.Collections.Generic;

#nullable disable

namespace SilverScreen.Models.Tables
{
    public partial class UserWarning
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Reason { get; set; }
        public bool IsItBan { get; set; }

        public virtual User User { get; set; }
    }
}
