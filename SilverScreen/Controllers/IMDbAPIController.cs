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
        /// <summary>
        /// A GET request that calls the "LoadMovieIntoDB" method from the "IMDbAPIService" service in order to save a movie into the database based on its title
        /// </summary>
        /// <param name="title">The method uses this string to send a get request for the particular movie we want to add to the database</param>
        [HttpPost]
        [Route("AddMovieToDB")]
        [Authorize]
        public IActionResult AddMovieToDB(string title)
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
                        iMDbAPIService.LoadMovieIntoDB(title);
                        return Ok();
                    }
                    catch (MySql.Data.MySqlClient.MySqlException)
                    {
                        return StatusCode(500);
                    }
                }
                else
                {
                    return Unauthorized();
                }
            }

            return Unauthorized();
        }
        [HttpPost]
        [Route("AddMoviesToDB")]
        [Authorize]
        public IActionResult AddMoviesToDB(string title,  int count)
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
                        return Json(iMDbAPIService.LoadMoviesIntoDB(title, count));
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
