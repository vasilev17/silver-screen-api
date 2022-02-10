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
            return notifications; //It returns complex structure. Should I make another model for this?
        }

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
            return notifications; //It returns complex structure. Should I make another model for this?
        }

        [HttpPost]
        [Route("SetFilmReleaseNotification")]
        [Authorize]
        public IActionResult SetFilmReleaseNotification(int movieID, bool status) //by status I mean delete if true, create if false 
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
