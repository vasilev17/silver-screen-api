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
    [Route("api/[controller]")]
    public class MainPageMovieInfoController : Controller
    {
        /// <summary>
        /// A get request that calls "GetMyListMovies" to get movies by specific genre.
        /// </summary>
        /// <param name="genre">The genre based on witch movies are retrieved.</param>
        /// <returns>Returns a call to "GetMoviesByGenre".</returns>
        [HttpGet]
        [Route("GetMoviesForMainPage")]
        public List<Movie> GetMoviesByGenreForMainPage(string genre)
        {
            MainPageMovieInfoService service = new MainPageMovieInfoService();
            return service.GetMoviesByGenre(genre);
        }
        /// <summary>
        /// A get request that calls "GetMyListMovies" to get watched movies.
        /// </summary>
        /// <param name="watched">The watched is based on with movie is completed or not.</param>
        /// <returns>Returns a call to "GetMyListMovies".</returns>
        [HttpGet]
        [Route("GetMoviesForMyList")]
        [Authorize]
        public IActionResult GetMoviesForMyList(bool watched)
        {
            var user = HttpContext.User;
            if (user.HasClaim(x => x.Type == "userID"))
            {
                MainPageMovieInfoService service = new MainPageMovieInfoService();
                return Json(service.GetMyListMovies(int.Parse(user.Claims.FirstOrDefault(x => x.Type == "userID").Value), watched));

            }
            return Unauthorized();
        }
        /// <summary>
        /// A get request that calls "SearchMovieByTitle" to get searched movies.
        /// </summary>
        /// <param name="searchString">The string based on which the searh is performed.</param>
        /// <returns>Returns a call to "SearchMovieByTitle".</returns>
        [HttpGet]
        [Route("GetMoviesBySearch")]
        public List<Movie> GetMoviesBySearchForMainPage(string searchString)
        {
            MainPageMovieInfoService service = new MainPageMovieInfoService();
            return service.SearchMovieByTitle(searchString);
        }
    }
}
