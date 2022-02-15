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
        
        /// <summary>
        /// Gets the movie that corresponds to a perticular ID
        /// </summary>
        /// <param name="movieID">The ID, based on which the movie is retrieved</param>
        /// <returns>Returns the movie object that has the entered ID</returns>
        public Movie GetMovieByID(int movieID)
        {
            SilverScreenContext context = new SilverScreenContext(configuration);
            using (context)
            {
                var movie = context.Movies.Where(s => s.Id == movieID);
                    return movie.FirstOrDefault();
            }
        }

        /// <summary>
        /// Gets all the comments that a certain movie has.
        /// </summary>
        /// <param name="movieID">The ID, based on which the comments for the right movie are retrieved.</param>
        /// <returns>Returns a list containing all of the comments a movie has</returns>
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

        /// <summary>
        /// Gets all the ratings that friends have given to a certain movie and calculates the average
        /// </summary>
        /// <param name="userID">The ID of the user, based on which the right group of friends is selected.</param>
        /// <param name="movieID">The ID of the movie, whose friend rating is wanted</param>
        /// <returns>Returns a number of type double, which represents the average of all friend ratings given to that movie</returns>
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

        /// <summary>
        /// Adds or removes a perticular movie to a list of watched or desired movies (MyList), based on wether it already exists in the list for the user
        /// </summary>
        /// <param name="userID">The ID of the user, based on which the movie is added to the right "MyList"</param>
        /// <param name="movieID">The ID of the movie, which should be added to "MyList"</param>
        /// <param name="watched">A boolean, which shows whether the movie should be added as already watched or desired</param>
        /// <returns>Returns an integer number, that shows whether the movie was successfully added (1) / removed (0) or an error occurred (-1)</returns>
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

        /// <summary>
        /// Saves/Deletes or modifies a rating a user gives to a movie
        /// </summary>
        /// <param name="userID">The ID of the user that is giving/deleting or modifying the rating.</param>
        /// <param name="movieID">The ID of the movie that corresponds to the rating given/removed/changed</param>
        /// <param name="rating">A number of type double that represents the rating that the user selects.</param>
        /// <returns>Returns an integer number, that shows whether a new rating was successfully added (1) / an old rating was removed (0) /
        /// an old rating was modified (2) or an error occurred (-1)</returns>
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
