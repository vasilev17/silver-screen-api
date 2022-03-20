using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SilverScreen.Models;
using SilverScreen.Models.Tables;
using SilverScreen.Services;
using System.Collections.Generic;
using System.Linq;

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
                return Json(service.GetFriendRatingByUser(int.Parse(user.Claims.FirstOrDefault(x => x.Type == "userID").Value), movieID));
            }
            return Unauthorized();
        }

        /// <summary>
        /// A POST request that calls the "AddMovieToMyList" method from the "MovieInfoService" service in order to add a movie to a list of watched/desired movies (MyList)
        /// </summary>
        /// <param name="movieID">The ID of the movie which should be added to "MyList"</param>
        /// <param name="watched">A boolean, which shows whether the movie should be added as already watched or desired</param>
        /// <returns>Returns a Json containing a message with the outcome or a response that says the user is Unauthorized</returns>
        [HttpPost]
        [Route("AddMovieToMyList")]
        [Authorize]
        public IActionResult AddMovieToMyList(int movieID, bool watched)
        {
            var user = HttpContext.User;
            if (user.HasClaim(x => x.Type == "userID"))
            {
                MovieInfoService service = new MovieInfoService();


                int result = service.AddMovieToMyList(int.Parse(user.Claims.FirstOrDefault(x => x.Type == "userID").Value), movieID, watched);

                switch (result)
                {
                    case 1:
                        return Json(new { msg = "Movie successfully ADDED to MyList!" });
                    case 2:
                        return Json(new { msg = "MyList section successfully changed!" });
                    case 0:
                        return Json(new { msg = "Movie already exists in this section of MyList!" });
                    case -1:
                        return Json(new { errorMsg = "Something went wrong!" });
                }
            }
            return Unauthorized();
        }


        /// <summary>
        /// A POST request that calls the "GiveMovieRating" method from the "MovieInfoService" service in order to add or modify a movie rating
        /// </summary>
        /// <param name="movieID">The ID of the movie, whose rating should be added or modified</param>
        /// <param name="rating">A number of type integer that represents the rating that the user gives</param>
        /// <returns>Returns a Json containing a message with the outcome or a response that says the user is Unauthorized</returns>
        [HttpPost]
        [Route("RateMovie")]
        [Authorize]
        public IActionResult RateMovie(int movieID, int rating)
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
                        return Json(new { msg = "This rating already exists!" });
                    case 2:
                        return Json(new { msg = "Successfully CHANGED a rating!" });
                    case -1:
                        return Json(new { errorMsg = "Something went wrong!" });
                }
            }
            return Unauthorized();
        }

        /// <summary>
        /// A POST request that calls the "RemoveMovieRating" method from the "MovieInfoService" service in order to remove a rating
        /// </summary>
        /// <param name="movieID">The ID of the movie, whose rating should be removed</param>
        /// <returns>Returns a Json containing a message with the outcome or a response that says the user is Unauthorized</returns>
        [HttpPost]
        [Route("RemoveRating")]
        [Authorize]
        public IActionResult RemoveRating(int movieID)
        {
            var user = HttpContext.User;
            if (user.HasClaim(x => x.Type == "userID"))
            {
                MovieInfoService service = new MovieInfoService();


                int result = service.RemoveMovieRating(int.Parse(user.Claims.FirstOrDefault(x => x.Type == "userID").Value), movieID);

                switch (result)
                {
                    case 1:
                        return Json(new { msg = "Movie rating successfully DELETED!" });
                    case -1:
                        return Json(new { errorMsg = "Something went wrong!" });
                }
            }
            return Unauthorized();
        }

        /// <summary>
        /// A POST request that calls the "RemoveMovieFromMyList" method from the "MovieInfoService" service in order to remove a movie from MyList
        /// </summary>
        /// <param name="movieID">The ID of the movie, which should be removed</param>
        /// <returns>Returns a Json containing a message with the outcome or a response that says the user is Unauthorized</returns>
        [HttpPost]
        [Route("RemoveMovieFromMyList")]
        [Authorize]
        public IActionResult RemoveMovieFromMyList(int movieID)
        {
            var user = HttpContext.User;
            if (user.HasClaim(x => x.Type == "userID"))
            {
                MovieInfoService service = new MovieInfoService();


                int result = service.RemoveMovieFromMyList(int.Parse(user.Claims.FirstOrDefault(x => x.Type == "userID").Value), movieID);

                switch (result)
                {
                    case 1:
                        return Json(new { msg = "Movie successfully DELETED from MyList!" });
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


        /// <summary>
        /// A GET request that calls the "GetPersonalRatingByUser" method from the "MovieInfoService" service in order to get the personal rating a user as given to a movie in the past
        /// </summary>
        /// <param name="movieID">The ID of the movie, whose personal rating should be retrieved</param>
        /// <returns>Returns a Json containing a call to a method that provides the personal rating that a movie has or a response that says the user is Unauthorized</returns>
        [HttpGet]
        [Route("PersonalRatingGetRequest")]
        [Authorize]
        public IActionResult GetPersonalRating(int movieID)
        {
            var user = HttpContext.User;
            if (user.HasClaim(x => x.Type == "userID"))
            {
                MovieInfoService service = new MovieInfoService();
                return Json(service.GetPersonalRatingByUser(int.Parse(user.Claims.FirstOrDefault(x => x.Type == "userID").Value), movieID));
            }
            return Unauthorized();
        }

        /// <summary>
        /// A GET request that calls methods from the "MovieInfoService" service in order to get the friend rating and the personal rating of a movie
        /// </summary>
        /// <param name="movieID">The ID of the movie, whose information should be retrieved</param>
        /// <returns>Returns a double number representing the friend rating and an integer number representing the personal rating</returns>
        [HttpGet]
        [Route("MovieRatingsGetRequest")]
        [Authorize]
        public MovieRatings GetMovieRatings(int movieID)
        {
            var user = HttpContext.User;
            MovieInfoService service = new MovieInfoService();
            MovieRatings movieRating = new MovieRatings()
            {
                FriendRating = service.GetFriendRatingByUser(int.Parse(user.Claims.FirstOrDefault(x => x.Type == "userID").Value), movieID),
                PersonalRating = service.GetPersonalRatingByUser(int.Parse(user.Claims.FirstOrDefault(x => x.Type == "userID").Value), movieID),

            };

            return movieRating;
        }

        /// <summary>
        /// A GET request that calls a method from the "MovieInfoService" service in order to get whether the movie is added to MyList and in which section
        /// </summary>
        /// <param name="movieID">The ID of the movie, whose information should be retrieved</param>
        /// <returns>Returns a string representing whether the movie exists and in which section of MyList</returns>
        [HttpGet]
        [Route("MyListInfoGetRequest")]
        [Authorize]
        public IActionResult GetMyListInfo(int movieID)
        {
            var user = HttpContext.User;
            if (user.HasClaim(x => x.Type == "userID"))
            {
                MovieInfoService service = new MovieInfoService();
                return Json(service.GetMyListInfoByMovieAndUser(int.Parse(user.Claims.FirstOrDefault(x => x.Type == "userID").Value), movieID));
            }
            return Unauthorized();
        }

    }
}
