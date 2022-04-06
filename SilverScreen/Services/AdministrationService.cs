using SilverScreen.Models;
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
                var commentReportQuery = context.CommentReports.Where(commentR => commentR.CommentId == comment.Id && commentR.Contents.Equals(comment.Content)); 
                if (commentReportQuery.Any())
                {
                    if(context.UserCommentReports.Where(report => report.ReportId == commentReportQuery.FirstOrDefault().Id && report.UserId == userID).Any())
                    {
                        allReportedComments.Add(commentReportQuery.FirstOrDefault().CommentId);
                    }
                }
            }

            return allReportedComments;
        }

        public void ReportComment(int userId, int commentId)
        {
            var context = new SilverScreenContext();
            var fetchedComment = context.Comments.Find(commentId);

            if (fetchedComment == null) throw new Exception("Comment doesn't exist!");

            var commentReportQuery = context.CommentReports.Where(comment => comment.CommentId == fetchedComment.Id && comment.Contents.Equals(fetchedComment.Content));
            
            if(commentReportQuery.Any())
            {
                var commentReport = commentReportQuery.FirstOrDefault();

                if (context.UserCommentReports.Where(report => report.ReportId == commentReport.Id && report.UserId == userId).Any())
                {
                    //If there is a record, there is no need to create a second one.
                    throw new Exception("Comment already reported!");
                }
                else
                {
                    //If comment is marked as false or already marked as reported, the don't count it.
                    if (commentReport.ReportedForFalsePositive) return;

                    //Insert only UserReports record 
                    var userReport = new UserCommentReport()
                    {
                        UserId = userId,
                        ReportId = commentReport.Id
                    };

                    context.Add(userReport);
                    context.SaveChanges();
                    context.Dispose();
                    return;
                }
            }
            else
            {
                //Insert new record in CommentReports, get ID and assign it to a new record in UserReports

                var commentReport = new CommentReport
                {
                    Id = context.CommentReports.Count() + 1,
                    UserId = fetchedComment.UserId,
                    MovieId = fetchedComment.MovieId,
                    CommentId = fetchedComment.Id,
                    Contents = fetchedComment.Content,
                    ReportedForFalsePositive = false,
                    ReportIsLegit = false
                };

                context.Add(commentReport);

                var userReport = new UserCommentReport()
                {
                    UserId = userId,
                    ReportId = commentReport.Id
                };

                context.Add(userReport);
                context.SaveChanges();
                context.Dispose();
                return;
            }
        }

        public ReviewComment LoadCommentForReview(int userId)
        {
            ReviewComment commentResult = null;
            var context = new SilverScreenContext();
            var commentForReview = context.CommentReports.Where(report => (report.UnderReview == null || report.UnderReview == userId) && report.ReportedForFalsePositive == false).FirstOrDefault();

            if (commentForReview != null)
            {
                commentForReview.UnderReview = userId;
                int reportAmount = context.UserCommentReports.Where(userR => userR.ReportId == commentForReview.Id).Count();
                commentResult = new ReviewComment()
                {
                    ReviewId = commentForReview.Id,
                    Contents = commentForReview.Contents,
                    TimesReported = reportAmount
                };
                context.SaveChanges();
                context.Dispose();
            }

            return commentResult;
        }

        public void ReportAsFalsePositive(int userId, int reportId)
        {
            var context = new SilverScreenContext();
            var report = context.CommentReports.Where(rprt => rprt.Id == reportId && rprt.UnderReview == userId).FirstOrDefault();
            
            if (report == null) throw new Exception("Report not found!");

            report.ReportedForFalsePositive = true;
            report.UnderReview = null;

            context.SaveChanges();

            var fakeRUsers = context.UserCommentReports.Where(user => user.ReportId == reportId).ToList();

            foreach(var user in fakeRUsers)
            {
                var findUserStats = context.AccountReports.Where(userS => userS.UserId == user.UserId).FirstOrDefault();
                if(findUserStats != null)
                {
                    //modify stats
                    findUserStats.FakeReports++;
                    context.SaveChanges();

                    //check for violation - if user passed the limit, then ban will happen
                    if (CheckForViolation(user.Id))
                    {
                        var userBan = new UserWarning()
                        {
                            IsItBan = true,
                            Reason = "Ban by the system for reaching the limits.",
                            UserId = user.UserId,
                        };
                        context.Add(userBan);
                        context.SaveChanges();
                        BanUser(userId, DateTime.UtcNow.AddDays(7));
                    }
                }
                else
                {
                    //create new stats
                    var userStat = new AccountReport()
                    {
                        UserId = user.UserId,
                        FakeReports = 1,
                        Reports = 0,
                    };
                    context.Add(userStat);
                    context.SaveChanges();
                }
            }
            context.Dispose();
        }

        private bool CheckForViolation(int userId)
        {
            var context = new SilverScreenContext();
            var config = context.BanConfigs.Find(1);
            if(config != null)
            {
                if (config.WarningsLimit != null)
                {
                    var userWarnings = context.UserWarnings.Where(userR => userR.UserId == userId).ToList();
                    if (userWarnings.Count() >= config.WarningsLimit)
                    {
                        return true;
                    }
                }
                if (config.FakeReportsLimit != null)
                {
                    var userAccountReports = context.AccountReports.Where(userR => userR.UserId == userId).FirstOrDefault();
                    if (userAccountReports != null)
                    {
                        if (userAccountReports.FakeReports < 4) return false;
                        float calcPercentage = ((userAccountReports.FakeReports - userAccountReports.Reports) / userAccountReports.Reports) * 100;
                        if(calcPercentage >= config.FakeReportsLimit) return true;
                    }
                }
            }
            return false;
        }

        private void BanUser(int userId, DateTime duration)
        {
            var context = new SilverScreenContext();
            var targetUser = context.Users.Find(userId);
            targetUser.Banned = duration;
            context.SaveChanges();
            context.Dispose();
        }
    }
}
