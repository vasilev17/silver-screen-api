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
                    TimesReported = reportAmount,
                    UserId = commentForReview.UserId,
                    Username = context.Users.Where(user => user.Id == commentForReview.UserId).Select(user => user.Username).FirstOrDefault()
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
                            Reason = "Banned by the system for reaching the limits.",
                            UserId = user.UserId,
                        };
                        context.Add(userBan);
                        context.SaveChanges();
                        BanUser(user.UserId, DateTime.UtcNow.AddDays(7));
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

        public List<UserWBRHistory> LoadHistoryForUser(int targetId)
        {
            var userHistoryReturn = new List<UserWBRHistory>();
            var context = new SilverScreenContext();
            var userHistory = context.UserWarnings.Where(user => user.UserId == targetId).ToList();

            foreach(var userH in userHistory)
            {
                var user = context.Users.Find(targetId);
                if(user != null)
                {
                    var userReturn = new UserWBRHistory()
                    {
                        BanDate = userH.IsItBan ? user.Banned : null,
                        Content = userH.Reason,
                        Severity = userH.IsItBan ? "ban" : "warn",
                    };
                    userHistoryReturn.Add(userReturn);
                }
            }

            return userHistoryReturn;
        }

        public void PenalizeUser(int userId, int targetId, string reason, bool isItBan, int reportId)
        {
            var context = new SilverScreenContext();
            var targetedUser = context.Users.Find(targetId);
            
            if (targetedUser == null) throw new Exception("User not found!");

            if (isItBan)
            {
                var banWrn = new UserWarning()
                {
                    IsItBan = true,
                    Reason = reason,
                    UserId = targetId
                };
                context.Add(banWrn);
                context.SaveChanges();
                BanUser(targetId, DateTime.UtcNow.AddDays(7));
            }
            else
            {
                var warning = new UserWarning()
                {
                    IsItBan = false,
                    Reason = reason,
                    UserId = targetId
                };
                context.Add(warning);
                context.SaveChanges();
                if (CheckForViolation(targetId))
                {
                    var banWrn = new UserWarning()
                    {
                        Reason = "Banned by the system for reaching the limits.",
                        IsItBan = true,
                        UserId = targetId
                    };
                    context.Add(banWrn);
                    context.SaveChanges();
                    BanUser(targetId, DateTime.UtcNow.AddDays(7));
                }
            }

            if(reportId != -1)
            {
                var commentReport = context.CommentReports.Find(reportId);
                if (commentReport != null && commentReport.UnderReview == userId)
                {
                    var harmComment = context.Comments.Find(commentReport.CommentId);
                    if (harmComment != null) context.Remove(harmComment);
                    context.SaveChanges();

                    var goodUsers = context.UserCommentReports.Where(user => user.ReportId == reportId).ToList();
                    foreach(var userR in goodUsers)
                    {
                        var userRR = context.AccountReports.Where(user => user.UserId == userR.UserId).FirstOrDefault();
                        if(userRR != null)
                        {
                            userRR.Reports++;
                            context.SaveChanges();
                        }
                        else
                        {
                            userRR = new AccountReport()
                            {
                                Reports = 1,
                                UserId = userR.UserId,
                                FakeReports = 0
                            };
                            context.Add(userRR);
                            context.SaveChanges();
                        }
                    }
                    context.RemoveRange(goodUsers);
                    context.Remove(commentReport);
                    context.SaveChanges();
                }
                else throw new Exception("Comment report not found!");
            }

        }

        public List<UserStat> LoadAllBannedUsers()
        {
            var userStatReturn = new List<UserStat>();
            var context = new SilverScreenContext();

            var bannedUsers = context.Users.Where(user => user.Banned > System.DateTime.UtcNow).ToList();

            foreach(var user in bannedUsers)
            {
                var fetchStats = context.AccountReports.Where(userR => userR.UserId == user.Id).FirstOrDefault();
                int fr = 0, rp = 0;

                if(fetchStats != null)
                {
                    fr = fetchStats.FakeReports;
                    rp = fetchStats.Reports;
                }

                var userStat = new UserStat()
                {
                    UserId = user.Id,
                    FakeReports = fr,
                    Username = user.Username,
                    Reports = rp,
                    Warnings = context.UserWarnings.Where(userR => userR.UserId == user.Id && userR.IsItBan == false).ToList().Count,
                };
                userStatReturn.Add(userStat);
            }

            return userStatReturn;
        }

        public List<UserStat> LoadAllUsers(int userId)
        {
            var userStatReturn = new List<UserStat>();
            var context = new SilverScreenContext();

            var allUsers = context.Users.Where(user => (user.Banned <= System.DateTime.UtcNow || user.Banned == null) && user.Id != userId).ToList();

            foreach (var user in allUsers)
            {
                var fetchStats = context.AccountReports.Where(userR => userR.UserId == user.Id).FirstOrDefault();
                int fr = 0, rp = 0;
                int warnCount = context.UserWarnings.Where(userR => userR.UserId == user.Id && userR.IsItBan == false).ToList().Count;

                if (fetchStats != null)
                {
                    fr = fetchStats.FakeReports;
                    rp = fetchStats.Reports;
                }

                if(!(fetchStats == null && warnCount == 0))
                {
                    var userStat = new UserStat()
                    {
                        UserId = user.Id,
                        FakeReports = fr,
                        Username = user.Username,
                        Reports = rp,
                        Warnings = warnCount,
                    };
                    userStatReturn.Add(userStat);
                }         
            }

            return userStatReturn;
        }

        public void FullUnbanUser(int userId)
        {
            var context = new SilverScreenContext();
            var userForUnban = context.Users.Find(userId);
            
            if (userForUnban == null) throw new Exception("User not found!");

            if (userForUnban.Banned != null)
            {
                userForUnban.Banned = null;
                context.SaveChanges();
                context.Dispose();
                ClearStatsForUser(userId);
            }
            else throw new Exception("The targeted user is not banned to get unbanned.");
        }

        public void ClearStatsForUser(int userId)
        {
            var context = new SilverScreenContext();
            var userToClear = context.Users.Find(userId);

            if (userToClear == null) throw new Exception("User not found!");

            context.RemoveRange(context.UserWarnings.Where(user => user.IsItBan == false && user.UserId == userId));
            context.RemoveRange(context.AccountReports.Where(user => user.UserId == userId));
            context.SaveChanges();
            context.Dispose();
        }

        public bool AuthenticateUser(int userId)
        {
            var context = new SilverScreenContext();
            var user = context.Users.Find(userId);

            if (user == null) throw new Exception("User not found!");

            if (user.Banned == null && user.IsDeleted == false) return true;

            if (user.Banned <= DateTime.UtcNow && user.IsDeleted == false)
            {
                FullUnbanUser(userId);
                return true;
            }

            return false;
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
