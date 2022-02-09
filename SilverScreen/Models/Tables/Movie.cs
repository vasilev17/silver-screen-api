using System;
using System.Collections.Generic;

#nullable disable

namespace SilverScreen.Models.Tables
{
    public partial class Movie
    {
        public Movie()
        {
            Comments = new HashSet<Comment>();
            MovieGenres = new HashSet<MovieGenre>();
            MovieNotifications = new HashSet<MovieNotification>();
            MovieRatings = new HashSet<MovieRating>();
            MovieStaffs = new HashSet<MovieStaff>();
            Notifications = new HashSet<Notification>();
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Thumbnail { get; set; }
        public string ImdbId { get; set; }
        public string Trailer { get; set; }
        public double Rating { get; set; }
        public string NetflixUrl { get; set; }
        public short ReleaseDate { get; set; }
        public int Duration { get; set; }
        public string MaturityRating { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<MovieGenre> MovieGenres { get; set; }
        public virtual ICollection<MovieNotification> MovieNotifications { get; set; }
        public virtual ICollection<MovieRating> MovieRatings { get; set; }
        public virtual ICollection<MovieStaff> MovieStaffs { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }
    }
}
