﻿namespace SilverScreen.Models
{
    public class ReviewComment
    {
        public int ReviewId { get; set; }
        public string Contents { get; set; }
        public int TimesReported { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; }
    }
}
