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
    [Route("[controller]")]
    public class UserController : Controller
    {
        public UserController(IConfiguration config)
        {
            Config = config;
        }
        private IConfiguration Config;

        [HttpGet]
        [Route("UserGetRequest")]
        [Authorize]
        public User GetUserDetails(int userID)
        {
            UserService userService = new UserService(Config);
            return userService.GetUserByID(userID);
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("Login")]
        public IActionResult Login([FromBody] Login login)
        {
            UserService userService = new UserService(Config);
            try
            {
                IActionResult response = Unauthorized();
                var user = userService.AuthenticateUser(login);

                if (user != null)
                {
                    var tokenString = userService.GenerateJSONWebToken(user);
                    response = Ok(new { token = tokenString });
                }


                return response;
            }
            catch (Exception ex)
            {
                return Ok(new { ErrorMessage = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("Register")]
        public IActionResult Register([FromBody] Login login)
        {
            UserService userService = new UserService(Config);
            try
            {
                var user = userService.RegisterUser(login);
                if (user != null)
                {
                    var tokenString = userService.GenerateJSONWebToken(user);
                    return Ok(new { token = tokenString });
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch(Exception ex)
            {
                return Ok(new { ErrorMessage = ex.Message });
            }
            

            
            
            //return 500
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("UploadAvatar")]
        public IActionResult UploadAvatar([FromBody] Login login)
        {
            var user = HttpContext.User;

            if (user.HasClaim(x => x.Type == "userID"))
            {
               UserService userService = new UserService(Config);
               // userService.UploadAvatar();            
            }

           
            return Ok(new { ErrorMessage = "Error" });

        }

        [Authorize]
        [HttpPost]
        [Route("SendFriendRequest")]
        public IActionResult SendFriendRequest(int friendID, string message)
        {
            var user = HttpContext.User;

            if (user.HasClaim(x => x.Type == "userID"))
            {
                NotificationService notificationService = new NotificationService(Config);
                notificationService.SendFriendNotification(int.Parse(user.Claims.FirstOrDefault(x => x.Type == "userID").Value), friendID,  message);
                return Ok(new { Message = "Sent request" });
            }


            return Ok(new { ErrorMessage = "Error" });

        }


    }
}
