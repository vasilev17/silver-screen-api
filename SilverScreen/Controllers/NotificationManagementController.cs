using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SilverScreen.Models.Tables;
using SilverScreen.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SilverScreen.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NotificationManagementController : Controller
    {
        private IConfiguration Configuration;
        public NotificationManagementController(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// Gets all notifications to the corresponding user. Needs token to authenticate.
        /// </summary>
        /// <returns>Unauthorized or a list with notifications.</returns>
        [HttpGet]
        [Route("GetNotifications")]
        [Authorize]
        public Notification[] GetNotifications() 
        {
            var user = HttpContext.User;
            Notification[] notifications = null;

            if (user.HasClaim(x => x.Type == "userID"))
            {
                NotificationService notificationService = new NotificationService(Configuration);
                notifications = notificationService.GetAllNotificationsForUser(int.Parse(user.Claims.FirstOrDefault(x => x.Type == "userID").Value));
            }   
            return notifications;
        }

        /// <summary>
        /// Gets all notifications for upcoming movies based to the corresponding user. Needs token to authenticate.
        /// </summary>
        /// <returns>Unauthorized or a list with notifications.</returns>
        [HttpGet]
        [Route("GetMovieNotifications")]
        [Authorize]
        public MovieNotification[] GetMovieNotifications()
        {
            var user = HttpContext.User;
            MovieNotification[] notifications = null;

            if (user.HasClaim(x => x.Type == "userID"))
            {
                NotificationService notificationService = new NotificationService(Configuration);
                notifications = notificationService.GetAllMovieNotificationsForUser(int.Parse(user.Claims.FirstOrDefault(x => x.Type == "userID").Value));
            }
            return notifications;
        }

        /// <summary>
        /// Sets notifications for upcoming movie. Needs token to authenticate.
        /// </summary>
        /// <param name="movieID">Movie identifier.</param>
        /// <param name="status">If true, delete the notification. If false, add a notification.</param>
        /// <returns>Returns code, based on outcome. Return code 0 is the best outcome.</returns>
        [HttpPost]
        [Route("SetFilmReleaseNotification")]
        [Authorize]
        public IActionResult SetFilmReleaseNotification(int movieID, bool status)
        {
            var user = HttpContext.User;
            if (user.HasClaim(x => x.Type == "userID"))
            {
                NotificationService notificationService = new NotificationService(Configuration);
                try
                {
                    switch (notificationService.SetFilmReleaseNotification(int.Parse(user.Claims.FirstOrDefault(x => x.Type == "userID").Value), movieID, status))
                    {
                        case 0:
                            return Json(new { code = 0 });
                        case 404:
                            return Json(new { code = 404, errorMsg = "Notification not found!" });
                        case -1:
                            return Json(new { code = -1, errorMsg = "Notification was already set before!" });
                        default:
                            return Json(new { code = 500, errorMsg = "Something went wrong!" });
                    }
                }
                catch (Exception)
                {
                    return Json(new { code = 500, errorMsg = "Something went wrong!" });
                }
            }
            return Unauthorized();
        }

        /// <summary>
        /// Accepts friend request by calling the User Service via the Notification Service. Token authentication required. 
        /// </summary>
        /// <param name="notificationId">Notification identifier that matches that friend request.</param>
        /// <returns>Return code, based on outcome. Return code 0 is the best outcome.</returns>
        [HttpPost]
        [Route("RespondToFriendRequest")]
        [Authorize]
        public IActionResult RespondToFriendRequest(int notificationId) //For test
        {
            var user = HttpContext.User;
            if (user.HasClaim(x => x.Type == "userID"))
            {
                NotificationService notificationService = new NotificationService(Configuration);
                switch (notificationService.RespondToFriendRequest(int.Parse(user.Claims.FirstOrDefault(x => x.Type == "userID").Value), notificationId))
                {
                    case 0:
                        return Json(new { code = 0 });
                    case -1:
                        return Json(new { code = 404, errorMsg = "Notification not found!" });
                    default:
                        return Json(new { code = 500, errorMsg = "Something went wrong!" });
                }
            }
            return Unauthorized();
        }

        /// <summary>
        /// Recomend a movie to another user. Currently there are no checks if that user is his friend or not. Token authentication required.
        /// </summary>
        /// <param name="friendId">User identifier of the another user.</param>
        /// <param name="movieId">Movie identifier.</param>
        /// <param name="message">The message that the user wants to send to the another user.</param>
        /// <returns>Return code, based on outcome. Return code 0 is the best outcome.</returns>
        [HttpPost]
        [Route("RecommendMovieToAFriend")]
        [Authorize]
        public IActionResult RecommendMovieToAFriend(int friendId, int movieId, string message)
        {
            var user = HttpContext.User;
            if (user.HasClaim(x => x.Type == "userID"))
            {
                NotificationService notificationService = new NotificationService(Configuration);
                Console.WriteLine(user.Claims.FirstOrDefault(x => x.Type == "userID").Value);
                switch (notificationService.RecommendMovieToAFriend(int.Parse(user.Claims.FirstOrDefault(x => x.Type == "userID").Value), friendId, movieId, message))
                {
                    case 0:
                        return Json(new { code = 0 });
                    default:
                        return Json(new { code = 500, errorMsg = "Something went wrong!" });
                }
            }
            return Unauthorized();
        }


        /// <summary>
        /// Used for when the user read the notification. If the method is called again, then the notification is marked as unread. Needs authentication token.
        /// </summary>
        /// <param name="notificationId">Notification identifier.</param>
        /// <returns>Return code, based on outcome. Return code 0 is the best outcome.</returns>
        [HttpPatch]
        [Route("ToggleNotificationActivity")]
        [Authorize]
        public IActionResult ToggleNotificationActivity(int notificationId) //For test
        {
            var user = HttpContext.User;
            if (user.HasClaim(x => x.Type == "userID"))
            {
                NotificationService notificationService = new NotificationService(Configuration);
                switch (notificationService.ToggleNotificationActivity(int.Parse(user.Claims.FirstOrDefault(x => x.Type == "userID").Value), notificationId))
                {
                    case 0:
                        return Json(new { code = 0 });
                    case -1:
                        return Json(new { code = 404, errorMsg = "Notification not found!" });
                    case 401:
                        return Unauthorized();
                    default:
                        return Json(new { code = 500, errorMsg = "Something went wrong!" });
                }
            }
            return Unauthorized();
        }

        /// <summary>
        /// Deletes the notification from the database, which corresponds to the currenly logged in user. Token authentication required.
        /// </summary>
        /// <param name="notificationId">Notification identifier.</param>
        /// <returns>Return code, based on outcome. Return code 0 is the best outcome.</returns>
        [HttpDelete]
        [Route("DeleteNotification")]
        [Authorize]
        public IActionResult DeleteNotifications(int notificationId) //For test
        {
            var user = HttpContext.User;
            if (user.HasClaim(x => x.Type == "userID"))
            {
                NotificationService notificationService = new NotificationService(Configuration);
                switch (notificationService.DeleteNotification(int.Parse(user.Claims.FirstOrDefault(x => x.Type == "userID").Value), notificationId))
                {
                    case 0:
                        return Json(new { code = 0 });
                    case -1:
                        return Json(new { code = 404, errorMsg = "Notification not found!" });
                    case 401:
                        return Unauthorized();
                    default:
                        return Json(new { code = 500, errorMsg = "Something went wrong!" });
                }
            }
            return Unauthorized();
        }
    }
}
