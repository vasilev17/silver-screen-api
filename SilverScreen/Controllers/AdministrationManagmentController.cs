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

        [HttpPost]
        [Authorize]
        [Route("GrantAdminToUser")]
        public IActionResult GrantAdminToUser(string username)
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
                        adminService.GrantUserAdminByUsername(username);
                        return Ok(new { message = $"Admin granted to user {username} successfully!" });
                    }
                    catch (Exception ex)
                    {
                        return BadRequest(new { message = ex.Message });
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
        [Authorize]
        [Route("RevokeAdminToUser")]
        public IActionResult RevokeAdminToUser(string username)
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
                        adminService.RevokeUserAdminByUsername(username);
                        return Ok(new { message = $"Revoked admin privileges to user {username} successfully!" });
                    }
                    catch (Exception ex)
                    {
                        return BadRequest(new { message = ex.Message });
                    }
                }
                else
                {
                    return Unauthorized();
                }
            }

            return Unauthorized();
        }

        [HttpGet]
        [Authorize]
        [Route("ListAdministrators")]
        public IActionResult ListAdministrators()
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
                        return Ok(new { AdminList = adminService.ListAdmins(userId) });
                    }
                    catch (Exception ex)
                    {
                        return BadRequest(new { message = ex.Message });
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
        [Authorize]
        [Route("SaveConfig")]
        public IActionResult SaveConfig(bool isFakeReportsSelected, bool isThereALimit, int fakeReports, int warningsLimit)
        {
            var user = HttpContext.User;

            if (user.HasClaim(x => x.Type == "userID"))
            {
                var adminService = new AdministrationService();
                int userId = int.Parse(user.Claims.FirstOrDefault(x => x.Type == "userID").Value);

                if (adminService.isUserAdministrator(userId))
                {
                    try
                    {
                        adminService.SaveConfig(isFakeReportsSelected, isThereALimit, fakeReports, warningsLimit);
                        return Ok(new { message = $"Config updated successfully!" });
                    }
                    catch (Exception)
                    {
                        return BadRequest(new { message = $"Config failed to update!" });
                    }
                }
                else
                {
                    return Unauthorized();
                }
            }

            return Unauthorized();
        }

        [HttpGet]
        [Authorize]
        [Route("LoadConfig")]
        public IActionResult LoadConfig()
        {
            var user = HttpContext.User;

            if (user.HasClaim(x => x.Type == "userID"))
            {
                var adminService = new AdministrationService();
                int userId = int.Parse(user.Claims.FirstOrDefault(x => x.Type == "userID").Value);

                if (adminService.isUserAdministrator(userId))
                {
                    try
                    {
                        return Ok(adminService.LoadConfig());
                    }
                    catch (Exception)
                    {
                        return BadRequest(new { message = $"Config failed to fetch!" });
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
