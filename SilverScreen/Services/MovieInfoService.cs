using Microsoft.Extensions.Configuration;
using SilverScreen.Models.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace SilverScreen.Services
{
    public class MovieInfoService
    {
        private IConfiguration configuration;

        public MovieInfoService(IConfiguration config)
        {
            configuration = config;
        }
        

        public Movie GetMovieByID(int movieID)
        {
            SilverScreenContext context = new SilverScreenContext(configuration);
            using (context)
            {
                var movie = context.Movies.Where(s => s.Id == movieID);
                if (movie.Any())
                {
                    return movie.FirstOrDefault();
                }
                else
                {
                    return null;
                }
            }
        }

        public List<Comment> GetCommentsByMovieID(int movieID)
        {
            SilverScreenContext context = new SilverScreenContext(configuration);
            List<Comment> comments = new List<Comment>();

            using (context)
            {

                comments = context.Comments.Where(s => s.MovieId == movieID).ToList();
                return comments;


            }

        }

        public double GetFriendRatingByUser(int userID, int movieID)
        {
            SilverScreenContext context = new SilverScreenContext(configuration);
            List<FriendList> friends = new List<FriendList>();
            List<double> ratings = new List<double>();
            double friendRating;

            using (context)
            {

                friends = context.FriendLists.Where(s => s.UserId == userID).ToList();

                foreach (var friend in friends)
                {
                    ratings.Add(context.MovieRatings.Where(s => s.UserId == friend.UserId1 && s.MovieId == movieID).FirstOrDefault().Rating);
                }
            }
            try
            {
                friendRating = ratings.Average();
                return friendRating;
            }
            catch (Exception)
            {
                return 0.0;
            }
            
        }

        public int ToggleMovieInMyList(int userID, int movieID, bool watched)
        {
            SilverScreenContext context = new SilverScreenContext(configuration);

            var movie = new MyList()
            {
                UserId = userID,
                MovieId = movieID,
                Watched = watched,
            };

            using (context)
            {

                var checkIfExists = context.MyLists.Where(s => s.UserId == userID && s.MovieId == movieID);
                try
                {
                    if (checkIfExists.Any())
                    {
                        context.MyLists.Remove(checkIfExists.FirstOrDefault());
                        context.SaveChanges();
                        return 0;
                    }
                    else
                    {
                        context.MyLists.Add(movie);
                        context.SaveChanges();
                        return 1;
                    }
                }
                catch (Exception)
                {

                    return -1;
                }
            }
        }

        public int GiveMovieRating(int userID, int movieID, double rating)
        {
            SilverScreenContext context = new SilverScreenContext(configuration);

            var movieRating = new MovieRating()
            {
                UserId = userID,
                MovieId = movieID,
                Rating = rating,
            };

            using (context)
            {

                var checkIfRatingExists = context.MovieRatings.Where(s => s.UserId == userID && s.MovieId == movieID && s.Rating == rating);
                var checkIfDifferentRatingExists = context.MovieRatings.Where(s => s.UserId == userID && s.MovieId == movieID);
                try
                {
                    if (checkIfRatingExists.Any())
                    {
                        context.MovieRatings.Remove(checkIfRatingExists.FirstOrDefault());
                        context.SaveChanges();
                        return 0;
                    }
                    else if(checkIfDifferentRatingExists.Any())
                    {
                        checkIfDifferentRatingExists.FirstOrDefault().Rating = rating;
                        context.SaveChanges();
                        return 2;
                    }
                    else
                    {
                        context.MovieRatings.Add(movieRating);
                        context.SaveChanges();
                        return 1;
                    }
                }
                catch (Exception)
                {

                    return -1;
                }
            }
        }


    }
}
