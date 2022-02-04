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




    }
}
