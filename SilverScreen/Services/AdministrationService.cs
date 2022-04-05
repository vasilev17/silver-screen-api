using SilverScreen.Models.Tables;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SilverScreen.Services
{
    public class AdministrationService
    {
        public bool isUserAdministrator(int userId)
        {
            var context = new SilverScreenContext();
            var user = context.Users.Find(userId);
            if (user.IsAdmin && !(user.IsDeleted || user.Banned > System.DateTime.UtcNow))
            {
                return true;
            }
            return false;
        }

        public void GrantUserAdminByUsername(string username)
        {
            var context = new SilverScreenContext();
            var user = context.Users.Where(user => user.Username.Equals(username)).FirstOrDefault();
            if(user != null)
            {
                if (!user.IsAdmin)
                {
                    user.IsAdmin = true;
                    context.SaveChanges();
                    context.Dispose();
                }
                else
                {
                    context.Dispose();
                    throw new Exception("User is already admin!");
                }
            }
            else
            {
                context.Dispose();
                throw new Exception($"User with username {username} does not exist!");
            }
        }

        public void RevokeUserAdminByUsername(string username)
        {
            var context = new SilverScreenContext();
            var user = context.Users.Where(user => user.Username.Equals(username)).FirstOrDefault();
            if (user != null)
            {
                if (user.IsAdmin)
                {
                    user.IsAdmin = false;
                    context.SaveChanges();
                    context.Dispose();
                }
                else
                {
                    context.Dispose();
                    throw new Exception("User is not admin!");
                }
            }
            else
            {
                context.Dispose();
                throw new Exception($"User with username {username} does not exist!");
            }
        }

        public List<User> ListAdmins(int userId)
        {
            var context = new SilverScreenContext();
            var adminUserQuery = context.Users.Where(user => user.Id != userId && user.IsAdmin);
            var adminUsers = new List<User>();

            foreach (var user in adminUserQuery)
            {
                var adminUser = new User
                {
                    Id = user.Id,
                    Username = user.Username
                };
                adminUsers.Add(adminUser);
            }

            return adminUsers;
        }

        public void SaveConfig(bool isFakeReportsSelected, bool isThereALimit, int fakeReports, int warningsLimit)
        {
            if (fakeReports > 100 || fakeReports < 0 || warningsLimit > 99 || warningsLimit < 1)
            {
                throw new Exception("Invalid settings!");
            }

            var context = new SilverScreenContext();
            var currentConfig = context.BanConfigs.Find(1);
            if (currentConfig != null)
            {
                currentConfig.FakeReportsLimit = isFakeReportsSelected ? fakeReports : null;
                currentConfig.WarningsLimit = isThereALimit ? warningsLimit : null;
                context.SaveChanges();
                context.Dispose();
            }
            else
            {
                BanConfig banConfig = new BanConfig()
                {
                    Id = 1,
                    FakeReportsLimit = isFakeReportsSelected ? fakeReports : null,
                    WarningsLimit = isThereALimit ? warningsLimit : null,
                };
                context.Add(banConfig);
                context.SaveChanges();
                context.Dispose();
            }
        }

        public BanConfig LoadConfig()
        {
            var context = new SilverScreenContext();
            var currentConfig = context.BanConfigs.Find(1);
            if(currentConfig != null)
            {
                return currentConfig;
            }
            else
            {
                return new BanConfig
                {
                    FakeReportsLimit = null,
                    WarningsLimit = null,
                };
            }
        }

        public List<int> ReportedCommentsForMovie(int userID, List<Comment> comments)
        {
            List<int> allReportedComments = new List<int>();
            var context = new SilverScreenContext();
            foreach(var comment in comments)
            {
                var commentReportQuery = context.CommentReports.Where(comment => comment.CommentId == comment.Id); //&& (comment.ReportedForFalsePositive == true || comment.ReportIsLegit == true)
                if (commentReportQuery.Any())
                {
                    if(context.UserCommentReports.Where(report => report.ReportId == commentReportQuery.FirstOrDefault().Id && report.UserId == userID).Any())
                    {
                        if (commentReportQuery.FirstOrDefault().Contents.Equals(comment.Content))
                        {
                            allReportedComments.Add(commentReportQuery.FirstOrDefault().CommentId);
                        } 
                    }
                }
            }

            return allReportedComments;
        }

        public void ReportComment(int userId, int commentId)
        {
            
        }
    }
}
