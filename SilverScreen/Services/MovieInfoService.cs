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

        public double GetFriendRating(int userID, int movieID)
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

    }
}
