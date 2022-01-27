using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SliverScreen.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AdministrationManagmentController : Controller
    {
        static List<string> comments = new List<string>();

        [HttpPost]
        [Route("PreloadComments")]
        public void PreloadComments()
        {
            comments.Add("I liked that film!");
            comments.Add("Go fuck yourself! I hate it!!!!");//Harrasment comment example
            comments.Add("Chill dude! This site is litteraly an IMDb ripoff.");
        }

        [HttpGet]
        [Route("CommentManagment")]
        public List<string> GetComments()
        {            
            return comments;
        }

        [HttpPost]
        [Route("CommentManagment")]
        public string PostComment(string comment)
        {
            comments.Add(comment);
            return "Comment added successfuly!";
        }

        [HttpPut]
        [Route("CommentManagment")]
        public string EditComment(int id, string comment)
        {
            comments[id] = comment;
            return "Comment edited successfuly!";
        }

        [HttpDelete]
        [Route("CommentManagment")]
        public string DeleteComment(int id)
        {
            comments.RemoveAt(id);
            return "Comment deleted successfuly!";
        }

    }
}
