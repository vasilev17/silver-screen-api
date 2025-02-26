﻿using System;
using System.Collections.Generic;

#nullable disable

namespace SilverScreen.Models.Tables
{
    public partial class Movie
    {
        public Movie()
        {
            CommentReports = new HashSet<CommentReport>();
            Comments = new HashSet<Comment>();
            MovieGenres = new HashSet<MovieGenre>();
            MovieNotifications = new HashSet<MovieNotification>();
            MovieRatings = new HashSet<MovieRating>();
            MovieStaffs = new HashSet<MovieStaff>();
            MyLists = new HashSet<MyList>();
            Notifications = new HashSet<Notification>();
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Thumbnail { get; set; }
        public string Trailer { get; set; }
        public double? Rating { get; set; }
        public string ReleaseDate { get; set; }
        public int? Duration { get; set; }
        public int TmdbId { get; set; }
        public string ContentType { get; set; }
        public string Bgimage { get; set; }

        public virtual ICollection<CommentReport> CommentReports { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<MovieGenre> MovieGenres { get; set; }
        public virtual ICollection<MovieNotification> MovieNotifications { get; set; }
        public virtual ICollection<MovieRating> MovieRatings { get; set; }
        public virtual ICollection<MovieStaff> MovieStaffs { get; set; }
        public virtual ICollection<MyList> MyLists { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }
    }
}
