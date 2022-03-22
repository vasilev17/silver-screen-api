using SilverScreen.Models.Tables;
using System;
using System.Collections.Generic;
using System.Linq;


namespace SilverScreen.Services
{
    public class MovieInfoService
    {
        /// <summary>
        /// Gets the movie that corresponds to a perticular ID
        /// </summary>
        /// <param name="movieID">The ID, based on which the movie is retrieved</param>
        /// <returns>Returns the movie object that has the entered ID</returns>
        public Movie GetMovieByID(int movieID)
        {
            SilverScreenContext context = new SilverScreenContext();
            using (context)
            {
                var movie = context.Movies.Where(s => s.Id == movieID);
                return movie.FirstOrDefault();
            }
        }

        /// <summary>
        /// Gets all the ratings that friends have given to a certain movie and calculates the average
        /// </summary>
        /// <param name="userID">The ID of the user, based on which the right group of friends is selected</param>
        /// <param name="movieID">The ID of the movie, whose friend rating is wanted</param>
        /// <returns>Returns a number of type double, which represents the average of all friend ratings given to that movie</returns>
        public double GetFriendRatingByUser(int userID, int movieID)
        {
            SilverScreenContext context = new SilverScreenContext();
            List<FriendList> friends = new List<FriendList>();
            List<double> ratings = new List<double>();
            double friendRating;

            using (context)
            {

                friends = context.FriendLists.Where(s => s.UserId == userID).ToList();

                foreach (var friend in friends)
                {
                    if (context.MovieRatings.Where(s => s.UserId == friend.UserId1 && s.MovieId == movieID).Any())
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
                return 0;
            }

        }

        /// <summary>
        /// Gets the rating that the user has given to a particular movie in the past
        /// </summary>
        /// <param name="userID">The ID of the user, based on which the right personal rating is selected</param>
        /// <param name="movieID">The ID of the movie, whose personal rating is wanted</param>
        /// <returns>Returns a number of type integer, which represents the rating given to that movie</returns>
        public int GetPersonalRatingByUser(int userID, int movieID)
        {
            SilverScreenContext context = new SilverScreenContext();
            int personalRating = 0;

            using (context)
            {

                if (context.MovieRatings.Where(s => s.UserId == userID && s.MovieId == movieID).Any())
                    personalRating = context.MovieRatings.Where(s => s.UserId == userID && s.MovieId == movieID).FirstOrDefault().Rating;
            }
            return personalRating;

        }

        /// <summary>
        /// Gets whether a movie is present and in which section of MyList
        /// </summary>
        /// <param name="userID">The ID of the user, based on which the right MyList is selected</param>
        /// <param name="movieID">The ID of the movie, which we want to know whether is present in MyList</param>
        /// <returns>Returns a string, which shows whether the movie is present in the Watchlist/Completed or is not</returns>
        public string GetMyListInfoByMovieAndUser(int userID, int movieID)
        {
            SilverScreenContext context = new SilverScreenContext();
            string response= "Not added";

            using (context)
            {

                if (context.MyLists.Where(s => s.UserId == userID && s.MovieId == movieID).Any()) {
                    if (context.MyLists.Where(s => s.UserId == userID && s.MovieId == movieID).FirstOrDefault().Watched == true)
                        response = "Completed"; 
                    else
                        response = "Watchlist";
                }
            }
            return response;

        }

        /// <summary>
        /// Adds a perticular movie to a list of watched or desired movies (MyList)
        /// </summary>
        /// <param name="userID">The ID of the user, based on which the movie is added to the right "MyList"</param>
        /// <param name="movieID">The ID of the movie, which should be added to "MyList"</param>
        /// <param name="watched">A boolean, which shows whether the movie should be added as already watched or desired</param>
        /// <returns>Returns an integer number, that shows whether the movie was successfully added (1) / already exists (0) / exists in a different section (2) or an error occurred (-1)</returns>
        public int AddMovieToMyList(int userID, int movieID, bool watched)
        {
            SilverScreenContext context = new SilverScreenContext();

            var movie = new MyList()
            {
                UserId = userID,
                MovieId = movieID,
                Watched = watched,
            };

            using (context)
            {

                var checkIfExists = context.MyLists.Where(s => s.UserId == userID && s.MovieId == movieID && s.Watched == watched);
                var checkIfAddedToDifferentMyListSection = context.MyLists.Where(s => s.UserId == userID && s.MovieId == movieID);
                try
                {
                    if (checkIfExists.Any())
                    {
                        return 0;
                    }
                    else if (checkIfAddedToDifferentMyListSection.Any())
                    {
                        checkIfAddedToDifferentMyListSection.FirstOrDefault().Watched = watched;
                        context.SaveChanges();
                        return 2;

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
        /// Removes a movie from the user's list of completed or desired movies
        /// </summary>
        /// <param name="userID">The ID of the user that is removing the movie</param>
        /// <param name="movieID">The ID of the movie which should be removed</param>
        /// <returns>Returns an integer number, that shows whether the movie was successfully removed (1) or an error occurred (-1)</returns>
        public int RemoveMovieFromMyList(int userID, int movieID)
        {
            SilverScreenContext context = new SilverScreenContext();


            using (context)
            {

                var checkIfExists = context.MyLists.Where(s => s.UserId == userID && s.MovieId == movieID);
                if (checkIfExists.Any())
                {
                    context.MyLists.Remove(checkIfExists.FirstOrDefault());
                    context.SaveChanges();
                    return 1;
                }
                else
                {
                    return -1;

                }
            }
        }

        /// <summary>
        /// Saves or modifies a rating a user gives to a movie
        /// </summary>
        /// <param name="userID">The ID of the user that is giving or modifying the rating</param>
        /// <param name="movieID">The ID of the movie that corresponds to the rating given/changed</param>
        /// <param name="rating">A number of type integer that represents the rating that the user selects</param>
        /// <returns>Returns an integer number, that shows whether a new rating was successfully added (1) / the rating already exists (0) /
        /// an old rating was modified (2) or an error occurred (-1)</returns>
        public int GiveMovieRating(int userID, int movieID, int rating)
        {
            SilverScreenContext context = new SilverScreenContext();

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

                        return 0;
                    }
                    else if (checkIfDifferentRatingExists.Any())
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


        /// <summary>
        /// Removes the rating a user has given to a movie
        /// </summary>
        /// <param name="userID">The ID of the user that is removing the rating</param>
        /// <param name="movieID">The ID of the movie whose rating should be removed</param>
        /// <returns>Returns an integer number, that shows whether a rating was successfully removed (1) or an error occurred (-1)</returns>
        public int RemoveMovieRating(int userID, int movieID)
        {
            SilverScreenContext context = new SilverScreenContext();


            using (context)
            {

                var checkIfRatingExists = context.MovieRatings.Where(s => s.UserId == userID && s.MovieId == movieID);
                if (checkIfRatingExists.Any())
                {
                    context.MovieRatings.Remove(checkIfRatingExists.FirstOrDefault());
                    context.SaveChanges();
                    return 1;
                }
                else
                {
                    return -1;

                }
            }
        }


        /// <summary>
        /// Gets all the genres that a certain movie has
        /// </summary>
        /// <param name="movieID">The ID, based on which the genres for the right movie are retrieved</param>
        /// <returns>Returns a list containing all of the genres a movie has</returns>
        public List<string> GetGenresByMovieID(int movieID)
        {
            SilverScreenContext context = new SilverScreenContext();

            List<MovieGenre> movieGenres = new List<MovieGenre>();
            List<string> genres = new List<string>();

            using (context)
            {

                movieGenres = context.MovieGenres.Where(s => s.MovieId == movieID).ToList();

                foreach (var movieGenre in movieGenres)
                {
                    genres.Add(context.Genres.Where(s => s.Id == movieGenre.GenreId).FirstOrDefault().Genre1);
                }
            }

            return genres;
        }

        /// <summary>
        /// Gets all the staff members that a certain movie has
        /// </summary>
        /// <param name="movieID">The ID, based on which the staff for the right movie is retrieved</param>
        /// <returns>Returns a list containing all of the staff members a movie has</returns>
        public List<staff> GetStaffByMovieID(int movieID)
        {
            SilverScreenContext context = new SilverScreenContext();

            List<MovieStaff> movieStaffs = new List<MovieStaff>();
            List<staff> staffs = new List<staff>();

            using (context)
            {

                movieStaffs = context.MovieStaffs.Where(s => s.MovieId == movieID).ToList();

                foreach (var movieStaff in movieStaffs)
                {
                    staffs.Add(context.staff.Where(s => s.Id == movieStaff.StaffId).FirstOrDefault());
                }
            }

            return staffs;
        }

    }
}
