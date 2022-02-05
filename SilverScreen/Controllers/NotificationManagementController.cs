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
        public Notification[] GetNotifications(int userId) //should have authentication later (not responsible for that part)
        {
            NotificationService notificationService = new NotificationService(Configuration);
            var notifications = notificationService.GetAllNotificationsForUser(userId);
            return notifications; //It returns complex structure. Should I make another model for this?
        }

        [HttpPost]
        [Route("RespondToFriendRequest")]
        public JsonResult RespondToFriendRequest(int notificationId) 
        {
            NotificationService notificationService = new NotificationService(Configuration);
            switch (notificationService.RespondToFriendRequest(notificationId)) 
            {
                case 0:
                    return Json(new { code = 0 });
                case -1:
                    return Json(new { code = 404, errorMsg = "Notification not found!" });
                default:
                    return Json(new { code = -1, errorMsg = "Something went wrong!" });
            }
        }

        [HttpPatch]
        [Route("ToggleNotificationActivity")]
        public JsonResult ToggleNotificationActivity(int notificationId)
        {
            NotificationService notificationService = new NotificationService(Configuration);
            switch (notificationService.ToggleNotificationActivity(notificationId))
            {
                case 0:
                    return Json(new { code = 0 });
                case -1:
                    return Json(new { code = 404, errorMsg = "Notification not found!" });
                default:
                    return Json(new { code = -1, errorMsg = "Something went wrong!" });
            }
        }

        [HttpDelete]
        [Route("DeleteNotification")]
        public JsonResult DeleteNotifications(int notificationId)
        {
            NotificationService notificationService = new NotificationService(Configuration);
            switch (notificationService.DeleteNotification(notificationId))
            {
                case 0:
                    return Json(new { code = 0 });
                case -1:
                    return Json(new { code = 404, errorMsg = "Notification not found!" });
                default:
                    return Json(new { code = -1, errorMsg = "Something went wrong!" });
            }
        }
    }
}
