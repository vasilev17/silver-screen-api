using Microsoft.Extensions.Configuration;
using SilverScreen.Models.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SilverScreen.Services
{
    public class MainPageMovieInfoService
    {
        private IConfiguration configuration;

        public MainPageMovieInfoService(IConfiguration config)
        {
            configuration = config;
        }
        public List<Movie> GetMoviesByGenre(int genre)
        {
            SilverScreenContext context = new SilverScreenContext(configuration);
            List<MovieGenre> movieGenres = new List<MovieGenre>();
            List<Movie> movies = new List<Movie>();
            using (context)
            {

                movieGenres = context.MovieGenres.Where(s => s.GenreId == genre).ToList();
                foreach (var movieGenre in movieGenres)
                {
                    movies.Add(context.Movies.Where(s => s.Id == movieGenre.MovieId).FirstOrDefault());
                }
                return movies;

            }

        }
    }
}
