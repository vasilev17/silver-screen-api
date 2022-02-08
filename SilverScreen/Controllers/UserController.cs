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
    public class UserController : Controller
    {
        public UserController(IConfiguration config)
        {
            Config = config;
        }
        private IConfiguration Config;

        [HttpGet]
        [Route("UserGetRequest")]
        public User GetUserDetails(int userID)
        {
            UserService userService = new UserService(Config);
            return userService.GetUserByID(userID);
        }
      
    }
}
