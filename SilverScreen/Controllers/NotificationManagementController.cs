using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SilverScreen.Models;
using SilverScreen.Models.Tables;
using SilverScreen.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SilverScreen.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationManagementController : Controller
    {
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
                NotificationService notificationService = new NotificationService();
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
                NotificationService notificationService = new NotificationService();
                notifications = notificationService.GetAllMovieNotificationsForUser(int.Parse(user.Claims.FirstOrDefault(x => x.Type == "userID").Value));
            }
            return notifications;
        }

        /// <summary>
        /// Sets notifications for upcoming movie. Needs token to authenticate.
        /// </summary>
        /// <returns>Returns code, based on outcome. Return code 0 is the best outcome.</returns>
        // <param name="movieId">Movie identifier.</param>
        // <param name="status">If true, delete the notification. If false, add a notification.</param>
        [HttpPost]
        [Route("SetFilmReleaseNotification")]
        [Authorize]
        public IActionResult SetFilmReleaseNotification(UpcomingFilmRequest request)
        {
            var user = HttpContext.User;
            if (user.HasClaim(x => x.Type == "userID"))
            {
                NotificationService notificationService = new NotificationService();
                try
                {
                    switch (notificationService.SetFilmReleaseNotification(int.Parse(user.Claims.FirstOrDefault(x => x.Type == "userID").Value), request.movieId, request.status))
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

        [HttpGet]
        [Route("GetSubscribedFilmStatus")]
        [Authorize]
        public IActionResult GetSubscribedFilmStatus(int movieId)
        {
            var user = HttpContext.User;
            if (user.HasClaim(x => x.Type == "userID"))
            {
                int userId = int.Parse(user.Claims.FirstOrDefault(x => x.Type == "userID").Value);
                NotificationService notificationService = new NotificationService();
                try
                {
                    return Ok(new { status = notificationService.GetSubscribedStatusMovie(userId, movieId) });
                }
                catch (Exception)
                {
                    return BadRequest(new { errorMsg = "Something went wrong!" });
                }
            }
            return Unauthorized();
        }


        /// <summary>
        /// Accepts friend request by calling the User Service via the Notification Service. Token authentication required. 
        /// </summary>
        /// <returns>Return code, based on outcome. Return code 0 is the best outcome.</returns>
        // <param name="notificationId">Notification identifier that matches that friend request.</param>
        [HttpPost]
        [Route("RespondToFriendRequest")]
        [Authorize]
        public IActionResult RespondToFriendRequest(BasicNotificationRequest request) //For test
        {
            var user = HttpContext.User;
            if (user.HasClaim(x => x.Type == "userID"))
            {
                NotificationService notificationService = new NotificationService();
                switch (notificationService.RespondToFriendRequest(int.Parse(user.Claims.FirstOrDefault(x => x.Type == "userID").Value), request.notificationId))
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
        /// <returns>Return code, based on outcome. Return code 0 is the best outcome.</returns>
        // <param name="friendId">User identifier of the another user.</param>
        // <param name="movieId">Movie identifier.</param>
        // <param name="message">The message that the user wants to send to the another user.</param>
        [HttpPost]
        [Route("RecommendMovieToAFriend")]
        [Authorize]
        public IActionResult RecommendMovieToAFriend(RecommendMovieToUserRequest request)
        {
            var user = HttpContext.User;
            if (user.HasClaim(x => x.Type == "userID"))
            {
                NotificationService notificationService = new NotificationService();
                switch (notificationService.RecommendMovieToAFriend(int.Parse(user.Claims.FirstOrDefault(x => x.Type == "userID").Value), request.friendIds, request.movieId, request.message))
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
        /// <returns>Return code, based on outcome. Return code 0 is the best outcome.</returns>
        // <param name="notificationId">Notification identifier.</param>
        [HttpPut]
        [Route("ToggleNotificationActivity")]
        [Authorize]
        public IActionResult ToggleNotificationActivity(BasicNotificationRequest request)
        {
            var user = HttpContext.User;
            if (user.HasClaim(x => x.Type == "userID"))
            {
                NotificationService notificationService = new NotificationService();
                switch (notificationService.ToggleNotificationActivity(int.Parse(user.Claims.FirstOrDefault(x => x.Type == "userID").Value), request.notificationId))
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
        /// Deletes the notification from the database, which corresponds to the currently logged in user. Token authentication required.
        /// </summary>
        /// <returns>Return code, based on outcome. Return code 0 is the best outcome.</returns>
        // <param name="notificationId">Notification identifier.</param>
        [HttpDelete]
        [Route("DeleteNotification")]
        [Authorize]
        public IActionResult DeleteNotifications(BasicNotificationRequest request) //For test
        {
            var user = HttpContext.User;
            if (user.HasClaim(x => x.Type == "userID"))
            {
                NotificationService notificationService = new NotificationService();
                switch (notificationService.DeleteNotification(int.Parse(user.Claims.FirstOrDefault(x => x.Type == "userID").Value), request.notificationId))
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
