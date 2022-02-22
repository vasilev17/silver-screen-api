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
        public List<Movie> GetMoviesByGenre(int genre)
        {
            SilverScreenContext context = new SilverScreenContext();
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

        public List<Movie> GetMyListMovies(int userID, bool watched)
        {
            SilverScreenContext context = new SilverScreenContext();
            List<MyList> myListMovies = new List<MyList>();
            List<Movie> movies = new List<Movie>();
            using (context)
            {
                myListMovies = context.MyLists.Where(s => s.UserId == userID && s.Watched == watched).ToList();
                foreach (var myListMovie in myListMovies)
                {
                    movies.Add(context.Movies.Where(s => s.Id == myListMovie.MovieId).FirstOrDefault());
                }                  
                return movies;

            }

        }
    }
}
