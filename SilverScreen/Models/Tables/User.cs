using System;
using System.Collections.Generic;

#nullable disable

namespace SilverScreen.Models.Tables
{
    public partial class User
    {
        public User()
        {
            Comments = new HashSet<Comment>();
            FriendListUserId1Navigations = new HashSet<FriendList>();
            FriendListUsers = new HashSet<FriendList>();
            MovieNotifications = new HashSet<MovieNotification>();
            MovieRatings = new HashSet<MovieRating>();
            NotificationAuthors = new HashSet<Notification>();
            NotificationUsers = new HashSet<Notification>();
            WatchedMovies = new HashSet<WatchedMovie>();
        }

        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Avatar { get; set; }
        public bool IsAdmin { get; set; }
        public DateTime? Banned { get; set; }
        public bool IsDeleted { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<FriendList> FriendListUserId1Navigations { get; set; }
        public virtual ICollection<FriendList> FriendListUsers { get; set; }
        public virtual ICollection<MovieNotification> MovieNotifications { get; set; }
        public virtual ICollection<MovieRating> MovieRatings { get; set; }
        public virtual ICollection<Notification> NotificationAuthors { get; set; }
        public virtual ICollection<Notification> NotificationUsers { get; set; }
        public virtual ICollection<WatchedMovie> WatchedMovies { get; set; }
    }
}
