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
    public class MainPageMovieInfoController : Controller
    {
        private IConfiguration configuration;


        public MainPageMovieInfoController(IConfiguration config)
        {
            configuration = config;
        }

        [HttpGet]
        [Route("GetMoviesForMainPage")]
        public List<Movie> GetMoviesByGenreForMainPage(int genreID)
        {
            MainPageMovieInfoService service = new MainPageMovieInfoService(configuration);
            return service.GetMoviesByGenre(genreID);
        }
    }
}
