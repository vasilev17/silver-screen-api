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

        /// <summary>
        /// A GET request that calls the "GetMovieByID" method from the "MovieInfoService" service in order to get a movie based on its ID
        /// </summary>
        /// <param name="movieID">The ID of the movie, whose info should be retrieved</param>
        /// <returns>Returns a call to a method that provides a movie object that has the entered ID</returns>
        [HttpGet]
        [Route("MovieGetRequest")]
        public Movie GetMovieDetails(int movieID)
        {
            MovieInfoService service = new MovieInfoService(configuration);
            return service.GetMovieByID(movieID);
        }

        /// <summary>
        /// A GET request that calls the "GetCommentsByMovieID" method from the "MovieInfoService" service in order to get all the comments the movie has
        /// </summary>
        /// <param name="movieID">The ID of the movie, whose comments should be retrieved</param>
        /// <returns>Returns a call to a method that provides a list that contains all of the commnets a movie has attatched to it</returns>
        [HttpGet]
        [Route("CommentsGetRequest")]
        public List<Comment> GetComments(int movieID)
        {
            MovieInfoService service = new MovieInfoService(configuration);
            return service.GetCommentsByMovieID(movieID);
        }

        /// <summary>
        /// A GET request that calls the "GetFriendRatingByUser" method from the "MovieInfoService" service in order to get the average of all friend ratings
        /// </summary>
        /// <param name="movieID">The ID of the movie, whose average friend rating should be calculated</param>
        /// <returns>Returns a Json containing a call to a method that provides the average friend rating that a movie has or a response that says the user is Unauthorized</returns>
        [HttpGet]
        [Route("FriendRatingGetRequest")]
        [Authorize]
        public IActionResult GetFriendRating(int movieID)
        {
            var user = HttpContext.User;
            if (user.HasClaim(x => x.Type == "userID"))
            {
                MovieInfoService service = new MovieInfoService(configuration);
                return Json( service.GetFriendRatingByUser(int.Parse(user.Claims.FirstOrDefault(x => x.Type == "userID").Value), movieID));
            }
            return Unauthorized();
        }

        /// <summary>
        /// A POST request that calls the "ToggleMovieInMyList" method from the "MovieInfoService" service in order to add or delete a movie from a list of watched/desired movies (MyList)
        /// </summary>
        /// <param name="movieID">The ID of the movie which should be added or deleted from "MyList"</param>
        /// <param name="watched">A boolean, which shows whether the movie should be added as already watched or desired</param>
        /// <returns>Returns a Json containing a message with the outcome or a response that says the user is Unauthorized</returns>
        [HttpPost]
        [Route("AddOrRemoveMovieFromMyList")]
        [Authorize]
        public IActionResult AddOrRemoveMovieFromMyList(int movieID, bool watched)
        {
            var user = HttpContext.User;
            if (user.HasClaim(x => x.Type == "userID"))
            {
                    MovieInfoService service = new MovieInfoService(configuration);


                    int result = service.ToggleMovieInMyList(int.Parse(user.Claims.FirstOrDefault(x => x.Type == "userID").Value), movieID, watched);

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

        
        /// <summary>
        /// A POST request that calls the "GiveMovieRating" method from the "MovieInfoService" service in order to add/delete or modify a movie rating
        /// </summary>
        /// <param name="movieID">The ID of the movie, whose rating should be added/deleted or modified</param>
        /// <param name="rating">A number of type double that represents the rating that the user gives</param>
        /// <returns>Returns a Json containing a message with the outcome or a response that says the user is Unauthorized</returns>
        [HttpPost]
        [Route("RateMovie")]
        [Authorize]
        public IActionResult RateMovie(int movieID, double rating)
        {
            var user = HttpContext.User;
            if (user.HasClaim(x => x.Type == "userID"))
            {
                MovieInfoService service = new MovieInfoService(configuration);


                int result = service.GiveMovieRating(int.Parse(user.Claims.FirstOrDefault(x => x.Type == "userID").Value), movieID, rating);

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
