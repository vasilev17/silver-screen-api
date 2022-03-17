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
        public IActionResult GetComments(int movieId) //Rethink this method, because it sorts friends only if they are marked!
        {
            var user = HttpContext.User;
            try
            {
                CommentService service = new CommentService();
                List<Comment> comments = new List<Comment>();
                int userId = 0;
                if (user.HasClaim(x => x.Type == "userID"))
                {
                    userId = int.Parse(user.Claims.FirstOrDefault(x => x.Type == "userID").Value);                 
                    comments.AddRange(service.FetchFriendCommentsForUser(userId, movieId));
                }
                comments.AddRange(service.FetchCommentsForMovie(userId, movieId));
                return Ok(comments);
                
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
            if (user.HasClaim(x => x.Type == "userID"))
            {
                int userId = int.Parse(user.Claims.FirstOrDefault(x => x.Type == "userID").Value);
                try
                {
                    return Ok( new { comment = service.GetUserComment(userId, movieId) });
                }
                catch(System.Exception ex)
                {
                    return BadRequest(ex.Message);
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
            if (user.HasClaim(x => x.Type == "userID"))
            {
                int userId = int.Parse(user.Claims.FirstOrDefault(x => x.Type == "userID").Value);
                try
                {
                    return Ok();
                }
                catch (System.Exception ex)
                {
                    return BadRequest(ex.Message);
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
            if (user.HasClaim(x => x.Type == "userID"))
            {
                int userId = int.Parse(user.Claims.FirstOrDefault(x => x.Type == "userID").Value);
                try
                {
                    return Ok();
                }
                catch (System.Exception ex)
                {
                    return BadRequest(ex.Message);
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
            if (user.HasClaim(x => x.Type == "userID"))
            {
                int userId = int.Parse(user.Claims.FirstOrDefault(x => x.Type == "userID").Value);
                try
                {
                    return Ok();
                }
                catch (System.Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            return Unauthorized();
        }

    }
}
