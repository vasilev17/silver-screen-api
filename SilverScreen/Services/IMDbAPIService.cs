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

namespace SilverScreen.Services
{
    public class IMDbAPIService
    {
        private IConfiguration configuration;

        public IMDbAPIService(IConfiguration config)
        {
            configuration = config;
        }

        public static void LoadMovieIntoDB(string title)
        {
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
            Console.WriteLine(extractedTrailer.linkEmbed);
            

        }
        
        }
}
