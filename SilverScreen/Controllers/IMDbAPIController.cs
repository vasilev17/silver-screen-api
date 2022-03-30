using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SilverScreen.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SilverScreen.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IMDbAPIController : Controller
    {
        [HttpGet]
        [Route("TestNotification")]
        public string GetTestNotification()
        {
            return "Get method";
        }
        [HttpDelete]
        [Route("TestNotification")]
        public string DeleteTestNotification()
        {
            return "Delete method";
        }
        [HttpPut]
        [Route("TestNotification")]
        public string PutTestNotification()
        {
            return "Put method";
        }


        [HttpPost]
        [Route("AddMoviesToDB")]
        [Authorize]
        public IActionResult LoadMoviesIntoDBviaTMDB(string title, int count, string contentType)
        {
            var user = HttpContext.User;

            if (user.HasClaim(x => x.Type == "userID"))
            {
                var adminService = new AdministrationService();
                var userService = new UserService();
                int userId = int.Parse(user.Claims.FirstOrDefault(x => x.Type == "userID").Value);

                if (adminService.isUserAdministrator(userId))
                {
                    try
                    {
                        IMDbAPIService iMDbAPIService = new IMDbAPIService();
                        return Json(iMDbAPIService.LoadMoviesIntoDBviaTMDB(title, count, contentType));
                    }
                    catch (Exception ex)
                    {
                        return BadRequest(new { errorMessage = ex.Message });
                    }
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
