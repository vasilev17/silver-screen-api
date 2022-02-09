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
        public Movie GetMovieDetails()
        {
            MovieInfoService service = new MovieInfoService(configuration);
            return service.GetMovieByID(1);
        }

        [HttpGet]
        [Route("CommentsGetRequest")]
        public List<Comment>  GetComments()
        {
            MovieInfoService service = new MovieInfoService(configuration);
            return service.GetCommentsByMovieID(1);
        }

        [HttpGet]
        [Route("FriendRatingGetRequest")]
        public double GetFriendRating()
        {
            MovieInfoService service = new MovieInfoService(configuration);
            return service.GetFriendRating(1,1);
        }

    }
}
