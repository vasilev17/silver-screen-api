using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SilverScreen.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SilverScreen.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdministrationManagmentController : Controller
    {
        [HttpGet]
        [Authorize]
        [Route("UserAuthentication")]
        public IActionResult UserAuthentication()
        {
            var user = HttpContext.User;

            if (user.HasClaim(x => x.Type == "userID"))
            {
                var adminService = new AdministrationService();
                var userService = new UserService();
                int userId = int.Parse(user.Claims.FirstOrDefault(x => x.Type == "userID").Value);

                if (adminService.isUserAdministrator(userId))
                {
                    var userObj = userService.GetUserByID(userId);
                    return Json(new {username = userObj.Username, avatar = userObj.Avatar});
                }
                else
                {
                    return Unauthorized();
                }
            }
            
            return Unauthorized();
        }

    }
}
