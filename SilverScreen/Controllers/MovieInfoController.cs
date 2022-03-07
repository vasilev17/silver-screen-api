using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SilverScreen.Models;
using SilverScreen.Models.Tables;
using SilverScreen.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SilverScreen.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MovieInfoController : Controller
    {
        /// <summary>
        /// A GET request that calls the "GetMovieByID" method from the "MovieInfoService" service in order to get a movie based on its ID
        /// </summary>
        /// <param name="movieID">The ID of the movie, whose info should be retrieved</param>
        /// <returns>Returns a call to a method that provides a movie object that has the entered ID</returns>
        [HttpGet]
        [Route("MovieGetRequest")]
        public Movie GetMovieDetails(int movieID)
        {
            MovieInfoService service = new MovieInfoService();
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
            MovieInfoService service = new MovieInfoService();
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
                MovieInfoService service = new MovieInfoService();
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
                    MovieInfoService service = new MovieInfoService();


                    int result = service.ToggleMovieInMyList(int.Parse(user.Claims.FirstOrDefault(x => x.Type == "userID").Value), movieID, watched);

                    switch (result)
                    {
                        case 1:
                            return Json(new { msg = "Movie successfully ADDED to MyList!" });
                        case 0:
                            return Json(new { msg = "Movie successfully REMOVED from MyList!" });
                        case -1:
                            return Json(new { errorMsg = "Something went wrong!" });
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
                MovieInfoService service = new MovieInfoService();


                int result = service.GiveMovieRating(int.Parse(user.Claims.FirstOrDefault(x => x.Type == "userID").Value), movieID, rating);

                switch (result)
                {
                    case 1:
                        return Json(new { msg = "Movie rating successful!" });
                    case 0:
                        return Json(new { msg = "Successfully REMOVED a rating!" });
                    case 2:
                        return Json(new { msg = "Successfully CHANGED a rating!" });
                    case -1:
                        return Json(new { errorMsg = "Something went wrong!" });
                }
            }
            return Unauthorized();
        }

        /// <summary>
        /// A GET request that calls the "GetGenresByMovieID" method from the "MovieInfoService" service in order to get all the genres of a movie
        /// </summary>
        /// <param name="movieID">The ID of the movie, whose genres should be retrieved</param>
        /// <returns>Returns a list of strings containing all the genres a particular movie has</returns>
        [HttpGet]
        [Route("GenresGetRequest")]
        public List<string> GetMovieGenres(int movieID)
        {
            MovieInfoService service = new MovieInfoService();
            return service.GetGenresByMovieID(movieID);
        }

        /// <summary>
        /// A GET request that calls the "GetStaffByMovieID" method from the "MovieInfoService" service in order to get all the staff of a movie
        /// </summary>
        /// <param name="movieID">The ID of the movie, whose staff should be retrieved</param>
        /// <returns>Returns a list of staff members a particular movie has</returns>
        [HttpGet]
        [Route("StaffGetRequest")]
        public List<staff> GetMovieStaff(int movieID)
        {
            MovieInfoService service = new MovieInfoService();
            return service.GetStaffByMovieID(movieID);
        }

        /// <summary>
        /// A GET request that calls methods from the "MovieInfoService" service in order to get all the information needed for a movie
        /// </summary>
        /// <param name="movieID">The ID of the movie, whose information should be retrieved</param>
        /// <returns>Returns a movie object, a list of strings containing the movie genres and a list of staff members</returns>
        [HttpGet]
        [Route("MovieInfoGetRequest")]
        public MovieInfo GetMovieInfo(int movieID)
        {
            MovieInfoService service = new MovieInfoService();
            MovieInfo movieInfo = new MovieInfo()
            {
                Movie = service.GetMovieByID(movieID),
                Genres = service.GetGenresByMovieID(movieID),
                Staff = service.GetStaffByMovieID(movieID),


            };

            return movieInfo;
        }

    }
}
