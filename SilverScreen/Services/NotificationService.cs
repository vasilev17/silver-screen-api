using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SilverScreen.Models.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SilverScreen.Services
{
    public class NotificationService
    {

        /// <summary>
        /// Gets all notification for a corresponding user by id.
        /// </summary>
        /// <param name="userId">User identifier.</param>
        /// <returns>A list that is full or empty, based on how social is the corresponding user.</returns>
        public Notification[] GetAllNotificationsForUser(int userId)
        {
            SilverScreenContext context = new SilverScreenContext();
            var notificationsRaw = context.Notifications
                .Where(x => x.UserId == userId)
                .Include(x => x.User)
                .Include(x => x.Author)
                .Include(x => x.Movie)
                .ToArray();

            List<Notification> notifications = new List<Notification>();

            foreach(var notification in notificationsRaw)
            {
                if(notification.MovieId != null)
                {
                    notifications.Add(new Notification
                    {
                        Id = notification.Id,
                        Active = notification.Active,
                        Author = new User
                        {
                            Id = notification.Author.Id,
                            Avatar = notification.Author.Avatar,
                            Username = notification.Author.Username
                        },
                        Type = notification.Type,
                        Content = notification.Content,
                        Movie = new Movie
                        {
                            Id = notification.Movie.Id,
                            Title = notification.Movie.Title
                        },
                    });
                }
                else
                {
                    notifications.Add(new Notification
                    {
                        Id = notification.Id,
                        Active = notification.Active,
                        Author = new User
                        {
                            Id = notification.Author.Id,
                            Avatar = notification.Author.Avatar,
                            Username = notification.Author.Username
                        },
                        Type = notification.Type,
                        Content = notification.Content
                    });
                }
                
            }

            context.Dispose();
            return notifications.ToArray();
        }

        /// <summary>
        /// Gets all upcoming movie notifications for a corresponding user by id.
        /// </summary>
        /// <param name="userId">User identifier.</param>
        /// <returns>A list that is full or empty, based on what's the interest for the corresponding user on upcoming films.</returns>
        public MovieNotification[] GetAllMovieNotificationsForUser(int userId)
        {
            SilverScreenContext context = new SilverScreenContext();
            var notificationsRaw = context.MovieNotifications
                .Where(x => x.UserId == userId)
                .Include(x => x.Movie)
                .ToArray();

            List<MovieNotification> notifications = new List<MovieNotification>();

            foreach (var notification in notificationsRaw)
            {
                if (notification.Date < DateTime.UtcNow)
                {
                    notifications.Add(new MovieNotification
                    {
                        Id = notification.Id,
                        Movie = new Movie
                        {
                            Id = notification.Movie.Id,
                            Title = notification.Movie.Title
                        },
                        Date = notification.Date,
                        UserId = notification.UserId,
                        MovieId = notification.MovieId,
                    });
                }
            }

            context.Dispose();
            return notifications.ToArray();
        }

        /// <summary>
        /// Send friend request via the notification system.
        /// </summary>
        /// <param name="userId">User identifier.</param>
        /// <param name="friendUsername">Username of the targeted friend. (was friend id before)</param>
        /// <param name="message">User stating why he wants the targeted friend in the friend list.</param>
        /// <returns>Return code, based on outcome. 0 for everything went smooth, -1 for finding a duplicate.</returns>
        public int SendFriendNotification(int userId, string friendUsername, string message)
        {
            SilverScreenContext context = new SilverScreenContext();

            var friendId = context.Users.Where(user => user.Username.Equals(friendUsername)).FirstOrDefault().Id;

            if (context.FriendLists.Where(user => user.UserId == userId && user.UserId1 == friendId).Any())
                return -1;

            //Check if similar notification already exists. Refuse the request if something like this happens
            if((context.Notifications.Where(x => x.UserId == userId && 
                                                 x.AuthorId == friendId).Any())
                ||
                (context.Notifications.Where(x => x.UserId == friendId &&
                                                  x.AuthorId == userId).Any()))
            {
                context.Dispose();
                return -1;
            }
            else
            {
                Notification friendRequest = new Notification()
                {
                    Active = true,
                    AuthorId = userId,
                    UserId = friendId,
                    Content = message,
                    Type = "FriendRequest"
                };
                context.Add(friendRequest);
                context.SaveChanges();
                context.Dispose();
                return 0;
            }
        }

        /// <summary>
        /// Recommend a movie to a user. There are currently no checks if the user and the another user are friends.
        /// </summary>
        /// <param name="userId">User identifier.</param>
        /// <param name="friendIds">User identifier of the targeted friend.</param>
        /// <param name="movieId">Movie identifier.</param>
        /// <param name="message">User stating why he wants the another user to watch the corresponding film.</param>
        /// <returns>Return code, based on outcome. 0 for everything went smooth, -1 for throwing an exception.</returns>
        public int RecommendMovieToAFriend(int userId, List<int> friendIds, int movieId, string message)
        {
            SilverScreenContext context = new SilverScreenContext();

            //Check if similar notification already exists. If it exists, replace the notification
            try
            {
                foreach(int friendId in friendIds)
                {
                    if (context.Notifications.Where(x => x.AuthorId == userId && x.UserId == friendId && x.MovieId == movieId).Any())
                    {
                        context.Notifications.Where(x => x.UserId == friendId && x.MovieId == movieId && x.AuthorId == userId).FirstOrDefault().Content = message;
                        context.SaveChanges();
                    }
                    else
                    {
                        Notification notification = new Notification()
                        {
                            Active = true,
                            AuthorId = userId,
                            UserId = friendId,
                            Content = message,
                            MovieId = movieId,
                            Type = "TextOnly"
                        };
                        context.Add(notification);
                        context.SaveChanges();
                    }
                } 
                context.Dispose();
                return 0;
            }
            catch (Exception)
            {
                return -1;
            }
        }

        /// <summary>
        /// Sets notification for upcoming film for a user.
        /// </summary>
        /// <param name="userId">User identifier.</param>
        /// <param name="movieID">Movie identifier.</param>
        /// <param name="status">If true, delete the notification, but if false, add a notification.</param>
        /// <returns>Return code, based on outcome. 0 for everything went smooth, 404 for not found, -1 if the record exists.</returns>
        public int SetFilmReleaseNotification(int userId, int movieID, bool status)
        {
            SilverScreenContext context = new SilverScreenContext();
            if (status)
            {
                if (context.MovieNotifications.Where(x => x.UserId == userId && x.MovieId == movieID).Any())
                {
                    context.Remove(context.MovieNotifications.Where(x => x.UserId == userId && x.MovieId == movieID).FirstOrDefault());
                    context.SaveChanges();
                    context.Dispose();
                    return 0;
                }
                context.Dispose();
                return 404;
            }
            else
            {
                if (!context.MovieNotifications.Where(x => x.UserId == userId && x.MovieId == movieID).Any())
                {
                    MovieNotification movieNotification = new MovieNotification()
                    {
                        Date = DateTime.Parse(context.Movies.Find(movieID).ReleaseDate),
                        MovieId = movieID,
                        UserId = userId
                    };
                    context.Add(movieNotification);
                    context.SaveChanges();
                    context.Dispose();
                    return 0;
                }
                else
                {
                    context.Dispose();
                    return -1;
                }
            }
        }

        /// <summary>
        /// Accepts friend request for the target user.
        /// </summary>
        /// <param name="userID">User identifier.</param>
        /// <param name="notificationId">Notification identifier that matches the friend request.</param>
        /// <returns>Return code, based on outcome. 0 for everything went smooth, -1 for not found</returns>
        public int RespondToFriendRequest(int userID, int notificationId)
        {
            SilverScreenContext context = new SilverScreenContext();
            var friendRequest = context.Notifications.Where(x => x.Id == notificationId && x.UserId == userID).Include(x => x.User);
            if(friendRequest.Any())
            {
                UserService userService = new UserService();
                switch(userService.AddFriend(friendRequest.FirstOrDefault().AuthorId, friendRequest.FirstOrDefault().UserId))
                {
                    case 0:
                        var username = friendRequest.FirstOrDefault().User.Username;
                        if(username.Length > 20)
                        {
                            username = username.Substring(0, 20) + "...";
                        }
                        Notification newNotification = new Notification()
                        {
                            Type = "TextOnly",
                            Content = username + " accepted your friend request.",
                            AuthorId = friendRequest.FirstOrDefault().UserId,
                            UserId = friendRequest.FirstOrDefault().AuthorId,
                            Active = true
                        };
                        context.Add(newNotification);
                        context.Remove(friendRequest.FirstOrDefault());
                        context.SaveChanges();
                        context.Dispose();
                        return 0;
                    default:
                        context.Dispose();
                        return -1;
                }
                
            }
            context.Dispose();
            return -1;
        }

        /// <summary>
        /// Toggle the read status of the notification for a targeted user.
        /// </summary>
        /// <param name="userID">User identifier.</param>
        /// <param name="notificationId">Notification identifier.</param>
        /// <returns>Return code, based on outcome. 0 for everything went smooth, -1 for not found, 401 for unauthorized.</returns>
        public int ToggleNotificationActivity(int userID, int notificationId)
        {
            SilverScreenContext context = new SilverScreenContext();
            var notification = context.Notifications.Find(notificationId);
            if(notification != null)
            {
                if (notification.UserId == userID)
                {
                    if (notification.Active.Value)
                    {
                        notification.Active = false;
                    }
                    else
                    {
                        notification.Active = true;
                    }
                    context.SaveChanges();
                    context.Dispose();
                    return 0;
                }
                else
                {
                    context.Dispose();
                    return 401;
                }
            }
            context.Dispose();
            return -1;
        }

        /// <summary>
        /// Deletes a notification for targeted user.
        /// </summary>
        /// <param name="userID">User identifier.</param>
        /// <param name="notificationId">Notification identifier.</param>
        /// <returns>Return code, based on outcome. 0 for everything went smooth, -1 for not found, 401 for unauthorized.</returns>
        public int DeleteNotification(int userID, int notificationId)
        {
            SilverScreenContext context = new SilverScreenContext();
            var notification = context.Notifications.Find(notificationId);
            if (notification != null)
            {
                if (notification.UserId == userID)
                {
                    context.Remove(notification);
                    context.SaveChanges();
                    context.Dispose();
                    return 0;
                }
                else
                {
                    context.Dispose();
                    return 401;
                }
            }
            context.Dispose();
            return -1;
        }
    }
}
