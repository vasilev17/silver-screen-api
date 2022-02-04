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




    }
}
