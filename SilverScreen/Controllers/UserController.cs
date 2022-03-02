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
        /// <summary>
        /// A GET request that calls the "GetUserByID" method from the "UserService" service in order to get the user information based on his ID
        /// </summary>
        /// <param name = "userID" >The ID of the user, whose info should be retrieved</param>
        /// <returns>Returns a call to a method that provides a user object that has the entered ID</returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("UserGetRequest")]
        [Authorize]
        public User GetUserDetails(int userID)
        {
            UserService userService = new UserService();
            return userService.GetUserByID(userID);
        }

        /// <summary>
        /// A Delete request that calls the "GetUserByID" method from the "UserService" service in order to delete the user and his information based on his ID
        /// </summary>
        // <param name = "userID" >The ID of the user, whose info should be retrieved</param>
        [AllowAnonymous]
        [HttpDelete]
        [Route("UserDeleteRequest")]
        [Authorize]
        public IActionResult DeleteUserDetails()
        {
            var user = HttpContext.User;

            if (user.HasClaim(x => x.Type == "userID"))
            {
                UserService userService = new UserService();
                userService.DeleteUserByID(int.Parse(user.Claims.FirstOrDefault(x => x.Type == "userID").Value));
                return Ok();
            }
            return Unauthorized();
        }

        /// <summary>
        /// A POST request that calls the "AuthenticateUser" method from the "UserService" service in order to validate the user credentials
        /// </summary>
        /// <param name="login">Object from the Login class. It contains (Email, Password, Username)</param>
        /// <returns>returns a new token for the specific user (using the GenerateJSONWebToken method) or (if the credentials are not identical) shows an exception message</returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("Login")]
        public IActionResult Login([FromBody] Login login)
        {
            UserService userService = new UserService();
            var rememberMe = login.RememberMe;
            try
            {
                IActionResult response = Unauthorized();
                var user = userService.AuthenticateUser(login);

                if (user != null)
                {
                    var tokenString = userService.GenerateJSONWebToken(user, rememberMe);
                    response = Ok(new { token = tokenString });
                }


                return response;
            }
            catch (Exception ex)
            {
                return Ok(new { ErrorMessage = ex.Message });
            }
        }

        /// <summary>
        /// A POST request that calls the "RegisterUser" method from the "UserService" service in order to create a new user (register)
        /// </summary>
        /// <param name="login">Object from the Login class. It contains (Email, Password, Username)</param>
        /// <returns>returns a new token for the specific user (using the GenerateJSONWebToken method) or (if the credentials are already used) shows an exception message</returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("Register")]
        public IActionResult Register([FromBody] Login login)
        {
            UserService userService = new UserService();
            try
            {
                var user = userService.RegisterUser(login);
                if (user != null)
                {
                    var tokenString = userService.GenerateJSONWebToken(user, false);
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
               UserService userService = new UserService();
               // userService.UploadAvatar();            
            }

           
            return Ok(new { ErrorMessage = "Error" });

        }

        /// <summary>
        /// A POST request that calls the "SendFriendNotification" method from the "NotificationService" service in order to send a notification (friend request)
        /// </summary>      
        /// <returns>Returns a Json containing a message with the outcome or a response that says the user is Unauthorized</returns>
        // <param name="friendID">The ID of the friend you are sending the request to</param>
        // <param name="message">The message displayed if the request is sent </param>
        [Authorize]
        [HttpPost]
        [Route("SendFriendRequest")]
        public IActionResult SendFriendRequest(AddFriendRequest request)
        {
            var user = HttpContext.User;

            if (user.HasClaim(x => x.Type == "userID"))
            {
                NotificationService notificationService = new NotificationService();
                notificationService.SendFriendNotification(int.Parse(user.Claims.FirstOrDefault(x => x.Type == "userID").Value), request.friendID, request.message);
                return Ok(new { Message = "Sent request" });
            }


            return Ok(new { ErrorMessage = "Error" });

        }


    }
}
