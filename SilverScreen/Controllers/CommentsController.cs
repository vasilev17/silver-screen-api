using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SilverScreen.Models.Tables;
using SilverScreen.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SilverScreen.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        [HttpGet]
        [Route("GetComments")]
        public IActionResult GetComments(int movieId)
        {
            var user = HttpContext.User;
            try
            {
                CommentService service = new CommentService();
                AdministrationService administrationService = new AdministrationService();
                List<Comment> comments = new List<Comment>();
                int userId = 0;
                bool authorized = false;
                if (user.HasClaim(x => x.Type == "userID"))
                {
                    userId = int.Parse(user.Claims.FirstOrDefault(x => x.Type == "userID").Value);                 
                    comments.AddRange(service.FetchFriendCommentsForUser(userId, movieId));
                    authorized = true;
                }
                comments.AddRange(service.FetchCommentsForMovie(userId, movieId));
                
                return Ok(new { authorized=authorized, comments = comments, reportedComments = administrationService.ReportedCommentsForMovie(userId, comments) });
                
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Authorize]
        [Route("GetOwnComment")]
        public IActionResult GetOwnComment(int movieId)
        {
            var user = HttpContext.User;
            CommentService service = new CommentService();
            var adminService = new AdministrationService();
            if (user.HasClaim(x => x.Type == "userID"))
            {
                int userId = int.Parse(user.Claims.FirstOrDefault(x => x.Type == "userID").Value);
                if (adminService.AuthenticateUser(userId))
                {
                    try
                    {
                        return Ok(new { comment = service.GetUserComment(userId, movieId) });
                    }
                    catch (System.Exception ex)
                    {
                        return BadRequest(ex.Message);
                    }
                }       
            }
            return Unauthorized();
        }

        [HttpPost]
        [Authorize]
        [Route("PostComment")]
        public IActionResult PostComment(int movieId, string message, bool friendsOnly)
        {
            var user = HttpContext.User;
            CommentService service = new CommentService();
            var adminService = new AdministrationService();
            if (user.HasClaim(x => x.Type == "userID"))
            {
                int userId = int.Parse(user.Claims.FirstOrDefault(x => x.Type == "userID").Value);
                if (adminService.AuthenticateUser(userId))
                {
                    try
                    {
                        service.PostComment(userId, movieId, message, friendsOnly);
                        return Ok();
                    }
                    catch (System.Exception ex)
                    {
                        return BadRequest(ex.Message);
                    }
                }  
            }
            return Unauthorized();
        }

        [HttpPut]
        [Authorize]
        [Route("UpdateComment")]
        public IActionResult UpdateComment(int movieId, string message, bool friendsOnly)
        {
            var user = HttpContext.User;
            CommentService service = new CommentService();
            var adminService = new AdministrationService();
            if (user.HasClaim(x => x.Type == "userID"))
            {
                int userId = int.Parse(user.Claims.FirstOrDefault(x => x.Type == "userID").Value);
                if (adminService.AuthenticateUser(userId))
                {
                    try
                    {
                        service.UpdateComment(userId, movieId, message, friendsOnly);
                        return Ok();
                    }
                    catch (System.Exception ex)
                    {
                        return BadRequest(ex.Message);
                    }
                }             
            }
            return Unauthorized();
        }

        [HttpDelete]
        [Authorize]
        [Route("DeleteComment")]
        public IActionResult DeleteComment(int movieId)
        {
            var user = HttpContext.User;
            CommentService service = new CommentService();
            var adminService = new AdministrationService();
            if (user.HasClaim(x => x.Type == "userID"))
            {
                int userId = int.Parse(user.Claims.FirstOrDefault(x => x.Type == "userID").Value);
                if (adminService.AuthenticateUser(userId))
                {
                    try
                    {
                        service.DeleteComment(userId, movieId);
                        return Ok();
                    }
                    catch (System.Exception ex)
                    {
                        return BadRequest(ex.Message);
                    }
                }               
            }
            return Unauthorized();
        }

        [HttpPost]
        [Authorize]
        [Route("ReportComment")]
        public IActionResult ReportComment(int commentId)
        {
            var user = HttpContext.User;
            var adminService = new AdministrationService();

            if (user.HasClaim(x => x.Type == "userID"))
            {
                int userId = int.Parse(user.Claims.FirstOrDefault(x => x.Type == "userID").Value);
                var service = new AdministrationService();
                if (adminService.AuthenticateUser(userId))
                {
                    try
                    {
                        service.ReportComment(userId, commentId);
                        return Ok();
                    }
                    catch (Exception ex)
                    {
                        return BadRequest(ex.Message);
                    }
                }             
            }

            return Unauthorized();
        }

    }
}
