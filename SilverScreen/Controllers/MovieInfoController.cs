using Microsoft.AspNetCore.Authorization;
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
    public class MovieInfoController : Controller
    {

        private IConfiguration configuration;


        public MovieInfoController(IConfiguration config) {
            configuration = config;
        }
        

        [HttpGet]
        [Route("MovieGetRequest")]
        public Movie GetMovieDetails(int movieID)
        {
            MovieInfoService service = new MovieInfoService(configuration);
            return service.GetMovieByID(movieID);
        }


        [HttpGet]
        [Route("CommentsGetRequest")]
        public List<Comment> GetComments(int movieID)
        {
            MovieInfoService service = new MovieInfoService(configuration);
            return service.GetCommentsByMovieID(movieID);
        }


        [HttpGet]
        [Route("FriendRatingGetRequest")]
        public double GetFriendRating(int userID, int movieID)
        {
            MovieInfoService service = new MovieInfoService(configuration);
            return service.GetFriendRatingByUser(userID,movieID);
        }


        [HttpPost]
        [Route("AddOrRemoveMovieFromMyList")]
        [Authorize]
        public IActionResult AddOrRemoveMovieFromMyList(int userID, int movieID, bool watched)
        {
            var user = HttpContext.User;
            if (user.HasClaim(x => x.Type == "userID"))
            {
                    MovieInfoService service = new MovieInfoService(configuration);


                    int result = service.ToggleMovieInMyList(userID, movieID, watched);

                    switch (result)
                    {
                        case 1:
                            return Json(new { errorMsg = "Movie successfully ADDED to MyList!" });
                            break;
                        case 0:
                            return Json(new { errorMsg = "Movie successfully REMOVED from MyList!" });
                            break;
                        case -1:
                            return Json(new { errorMsg = "Something went wrong!" });
                            break;
                    }
            }
            return Unauthorized();
        }


        [HttpPost]
        [Route("RateMovie")]
        [Authorize]
        public IActionResult RateMovie(int userID, int movieID, double rating)
        {
            var user = HttpContext.User;
            if (user.HasClaim(x => x.Type == "userID"))
            {
                MovieInfoService service = new MovieInfoService(configuration);


                int result = service.GiveMovieRating(userID, movieID, rating);

                switch (result)
                {
                    case 1:
                        return Json(new { errorMsg = "Movie rating successful!" });
                        break;
                    case 0:
                        return Json(new { errorMsg = "Successfully REMOVED a rating!" });
                        break;
                    case 2:
                        return Json(new { errorMsg = "Successfully CHANGED a rating!" });
                        break;
                    case -1:
                        return Json(new { errorMsg = "Something went wrong!" });
                        break;
                }
            }
            return Unauthorized();
        }


    }
}
