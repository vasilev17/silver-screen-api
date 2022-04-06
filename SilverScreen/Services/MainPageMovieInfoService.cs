using Microsoft.EntityFrameworkCore;
using SilverScreen.Models;
using SilverScreen.Models.Tables;
using System.Collections.Generic;
using System.Linq;

namespace SilverScreen.Services
{
    public class MainPageMovieInfoService
    {
        /// <summary>
        /// This metod takes all movies based on a specific genre.
        /// </summary>
        /// <param name="genre">Take movies based on the genre you have chosen.</param>
        /// <returns>Returns list of movies by genre.</returns>
        public List<MovieDisplay> GetMoviesByGenre(string genre)
        {
            SilverScreenContext context = new SilverScreenContext();
            List<MovieDisplay> movies = new List<MovieDisplay>();



            var genreMovies = context.Genres.Where(s => s.Genre1.Equals(genre)).Include(s => s.MovieGenres).FirstOrDefault();
            context.Dispose();
            SilverScreenContext context1 = new SilverScreenContext();
            foreach (var genreMovie in genreMovies.MovieGenres)
            {
                
                movies.Add(context1.Movies.Where(s => s.Id == genreMovie.MovieId).Select(
                    m => new MovieDisplay()
                    {
                        Id = m.Id,
                        Thumbnail = m.Thumbnail
                    }).FirstOrDefault());

            }
            context1.Dispose();
            movies.Reverse();
            return movies;



        }
        /// <summary>
        /// This method takes a user's movies and put them in list to see if they have been watched or are for watching.
        /// </summary>
        /// <param name="userID">Takes a user id.</param>
        /// <param name="watched">Takes by true or false with movie is watched.</param>
        /// <returns>Returns list of movies based on with have been watched or are for watching.</returns>
        public List<MovieDisplay> GetMyListMovies(int userID, bool watched)
        {
            SilverScreenContext context = new SilverScreenContext();
            List<MyList> myListMovies = new List<MyList>();
            List<MovieDisplay> movies = new List<MovieDisplay>();
            using (context)
            {
                myListMovies = context.MyLists.Where(s => s.UserId == userID && s.Watched == watched).ToList();
                foreach (var myListMovie in myListMovies)
                {
                    movies.Add(context.Movies.Where(s => s.Id == myListMovie.MovieId).Select(
                    m => new MovieDisplay()
                    {
                        Id = m.Id,
                        Thumbnail = m.Thumbnail
                    }).FirstOrDefault());
                }
                return movies;

            }

        }
        /// <summary>
        /// This metod takes the movies and search them by title.
        /// </summary>
        /// <param name="searchString">The string based on which the searh is performed.</param>
        /// <returns>Returns a list that contains all movies with that title.</returns>
        public List<MovieDisplay> SearchMovieByTitle(string searchString)
        {
            SilverScreenContext context = new SilverScreenContext();
            List<MovieDisplay> searchMovies = new List<MovieDisplay>();
            using (context)
            {
                searchMovies = context.Movies.Where(s => s.Title.Contains(searchString))
                    .Select(
                    m => new MovieDisplay()
                    {
                        Id = m.Id,
                        Thumbnail = m.Thumbnail
                    }).ToList();
            }
            searchMovies.Reverse();
            return searchMovies;
        }
        public List<MovieDisplay> GetMoviesByContentAndGenre(string genre, string content)
        {
            SilverScreenContext context = new SilverScreenContext();
            List<MovieDisplay> movies = new List<MovieDisplay>();

            var genreMovies = context.Genres.Where(s => s.Genre1.Equals(genre)).Include(s => s.MovieGenres).FirstOrDefault();
            context.Dispose();
            SilverScreenContext context1 = new SilverScreenContext();
            foreach (var genreMovie in genreMovies.MovieGenres)
            {
                var movie = context1.Movies.Where(x => x.Id == genreMovie.MovieId && x.ContentType.Equals(content)).Select(
                    m => new MovieDisplay()
                    {
                        Id = m.Id,
                        Thumbnail = m.Thumbnail
                    });
                if (movie.Any())
                {
                    movies.Add(movie.FirstOrDefault());
                }
            }
            context1.Dispose();
            movies.Reverse();
            return movies;
        }
    }
}
