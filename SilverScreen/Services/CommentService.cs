using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SilverScreen.Models.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SilverScreen.Services
{
    public class CommentService
    {
        public List<Comment> FetchFriendCommentsForUser(int userId, int movieId)
        {
            SilverScreenContext context = new SilverScreenContext();
            List<Comment> comments = new List<Comment>();
            var userFriends = context.FriendLists.Where(x => x.UserId1 == userId).Include(x => x.User).ToList();

            foreach(var friend in userFriends)
            {
                var commentQuery = context.Comments.Where(x => x.UserId == friend.UserId && x.MovieId == movieId).ToList();
                if(commentQuery.Any())
                {
                    Comment comment = new Comment
                    {
                        Id = commentQuery.FirstOrDefault().Id,
                        MovieId = movieId,
                        Content = commentQuery.FirstOrDefault().Content,
                        User = new User
                        {
                            Id = friend.User.Id,
                            Username = friend.User.Username,
                            Avatar = friend.User.Avatar
                        },
                        IsFriendsOnly = true
                    };
                    comments.Add(comment);
                }
            }

            return comments;            
        }

        public List<Comment> FetchCommentsForMovie(int userId, int movieId)
        {
            SilverScreenContext context = new SilverScreenContext();
            List<Comment> comments = new List<Comment>();

            var fetchedComments = context.Comments.Where(x => x.MovieId == movieId && !x.IsFriendsOnly && x.UserId !=userId).Include(x => x.User);
            var userFriends = context.FriendLists.Where(x => x.UserId1 == userId).Include(x => x.User).ToList();

            foreach (var comment in fetchedComments)
            {
                bool ignoreFriend = false;
                for (int i = 0; i < userFriends.Count; i++)
                {
                    if (comment.UserId == userFriends[i].UserId) { ignoreFriend = true; break; }
                }
                if (!ignoreFriend)
                {
                    Comment commentResult = new Comment
                    {
                        Id = comment.Id,
                        MovieId = movieId,
                        Content = comment.Content,
                        User = new User
                        {
                            Id = comment.User.Id,
                            Username = comment.User.Username,
                            Avatar = comment.User.Avatar
                        },
                        IsFriendsOnly = false
                    };
                    comments.Add(commentResult);
                }
            }
            return comments;
        }

        public Comment GetUserComment(int userId, int movieId)
        {
            SilverScreenContext context = new SilverScreenContext();
            var commentQuery = context.Comments.Where(x => x.UserId == userId && x.MovieId == movieId);
            if(commentQuery.Any())
            {
                return commentQuery.FirstOrDefault();
            }
            else
            {
                return null;
            }
        }

        public void PostComment(int userId, int movieId, string message, bool friendsOnly)
        {
            SilverScreenContext context = new SilverScreenContext();

            if(context.Comments.Where(comment => comment.UserId == userId && comment.MovieId == movieId).Any())
            {
                throw new Exception("Comment already exists!");
            }
            
            Comment comment = new Comment
            {
                UserId = userId,
                Content = message,
                MovieId = movieId,
                IsFriendsOnly = friendsOnly
            };

            context.Add(comment);
            context.SaveChanges();
            context.Dispose();
        }

        public void UpdateComment(int userId, int movieId, string message, bool friendsOnly)
        {
            SilverScreenContext context = new SilverScreenContext();

            var commentQuery = context.Comments.Where(comment => comment.UserId == userId && comment.MovieId == movieId);
            if (!commentQuery.Any())
            {
                throw new Exception("Comment does not exist!");
            }

            var comment = commentQuery.FirstOrDefault();
            comment.Content = message;
            comment.IsFriendsOnly = friendsOnly;
            context.SaveChanges();
            context.Dispose();
        }


        public void DeleteComment(int userId, int movieId)
        {
            SilverScreenContext context = new SilverScreenContext();

            var commentQuery = context.Comments.Where(comment => comment.UserId == userId && comment.MovieId == movieId);
            if (!commentQuery.Any())
            {
                throw new Exception("Comment does not exist!");
            }

            context.Remove(commentQuery.First());
            context.SaveChanges();
            context.Dispose();
        }
    }
}
