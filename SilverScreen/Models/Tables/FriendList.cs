using System;
using System.Collections.Generic;

#nullable disable

namespace SilverScreen.Models.Tables
{
    public partial class FriendList
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int UserId1 { get; set; }

        public virtual User User { get; set; }
        public virtual User UserId1Navigation { get; set; }
    }
}
