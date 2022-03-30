using System;
using System.Collections.Generic;

#nullable disable

namespace SilverScreen.Models.Tables
{
    public partial class BanConfig
    {
        public int Id { get; set; }
        public int? FakeReportsLimit { get; set; }
        public int? WarningsLimit { get; set; }
    }
}
