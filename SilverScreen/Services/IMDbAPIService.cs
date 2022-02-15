using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using System.ComponentModel.DataAnnotations;
using System.Text;
using RestSharp;
using SilverScreen.Models;
using SilverScreen.Models.Tables;

namespace SilverScreen.Services
{
    public class IMDbAPIService
    {
        private IConfiguration configuration;

        public IMDbAPIService(IConfiguration config)
        {
            configuration = config;
        }

        public void LoadMovieIntoDB(string title)
        {
            SilverScreenContext context = new SilverScreenContext(configuration); 

            string url = "https://imdb-api.com/API/AdvancedSearch/k_faxyw40f";
            var client = new RestClient(url);
            var request = new RestRequest();
            request.AddParameter("title", title);
            request.AddParameter("count", "1");
            var response = client.Get(request);
            var extractedFilm = JsonSerializer.Deserialize<IMDBQuery>(response.Content);
            string imdbId = extractedFilm.results[0].id;
            string urlTrailer = $"https://imdb-api.com/en/API/Trailer/k_faxyw40f/{imdbId}";
            var clientTrailer = new RestClient(urlTrailer);
            var requestTrailer = new RestRequest();
            var responseTrailer = clientTrailer.Get(requestTrailer);
            var extractedTrailer = JsonSerializer.Deserialize<IMDBTrailerLink>(responseTrailer.Content);
            //Console.WriteLine(response.Content.ToString());
            //Console.WriteLine(extractedFilm.results[0].id);
            //Console.WriteLine(extractedFilm.results[0].description);
            //Console.WriteLine(extractedFilm.results[0].genres);
            //Console.WriteLine(extractedFilm.results[0].image);
            //Console.WriteLine(extractedFilm.results[0].imDbRating);
            //Console.WriteLine(extractedFilm.results[0].plot);
            //Console.WriteLine(extractedFilm.results[0].runtimeStr);
            //Console.WriteLine(extractedFilm.results[0].stars);
            //Console.WriteLine(extractedFilm.results[0].contentRating);


            var movie = new Movie()
            {
                ImdbId = extractedFilm.results[0].id,
                Title = extractedFilm.results[0].title,
                Description = extractedFilm.results[0].plot,
                Thumbnail = extractedFilm.results[0].image,
                Rating = Double.Parse(extractedFilm.results[0].imDbRating),
                Duration = int.Parse(extractedFilm.results[0].runtimeStr.Split(' ')[0]),
                MaturityRating = extractedFilm.results[0].contentRating,
                Trailer = extractedTrailer.linkEmbed,
                ReleaseDate = extractedFilm.results[0].description
            };
            using (context)
            {
                context.Movies.Add(movie);
                context.SaveChanges();
            }

        }
        

    }
}
