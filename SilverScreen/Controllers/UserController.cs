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
            IActionResult response = Unauthorized();
            var user = userService.AuthenticateUser(login);

            if (user != null)
            {
                var tokenString = userService.GenerateJSONWebToken(user);
                response = Ok(new { token = tokenString });
            }

            return response;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("Register")]
        public IActionResult Register([FromBody] Login login)
        {
            UserService userService = new UserService(Config);
            var user = userService.RegisterUser(login);

            if (user != null)
            {
                var tokenString = userService.GenerateJSONWebToken(user);
                return Ok(new { token = tokenString });
            }
            return Ok(new { ErrorMessage = "Error" });
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

    }
}
