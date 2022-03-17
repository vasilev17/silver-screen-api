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
                var commentQuery = context.Comments.Where(x => x.UserId == friend.UserId && x.IsFriendsOnly && x.MovieId == movieId).ToList();
                if(commentQuery.Any())
                {
                    Comment comment = new Comment
                    {
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

            foreach(var comment in fetchedComments)
            {
                Comment commentResult = new Comment
                {
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
    }
}
