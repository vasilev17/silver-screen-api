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
using System.Globalization;

namespace SilverScreen.Services
{
    public class IMDbAPIService
    {
        
        /// <summary>
        /// This method sends a get request to the imdb-api using rest sharp to get a number of movie/s based from the input we get from
        /// the title and count, then the method checks if the count is less than 1 because we can't add a negative number of movies, if the
        /// count is less then 1 we throw and exception. To use the api we need api keys, the api keys we have have a limit of 100 requests
        /// per day so we've made and array with 6 keys. We use the variable keyCount to know on which key we are at the moment.
        /// after every request we check if we got an error message if we do we add 1 to keyCount and we change the key
        /// to the next one in the array we make the same request with the new key and do the check again this repeats until we get a key that has request,  but if we 
        /// dont have any keys with request we throw an exception. If we've successfully completed the first request that gets most of the data we need.
        /// We check if we got any movies at all if we did not we trow an exception. We check if we got less movies than the wanted amount by the user
        /// and if we did we add the amount found, but if its more we only add the wanted amount. Then we run 3 more request with the imdbId we got for every
        /// movie we got from the previus request. The 3 new request get the Trailer, Staff, Description and Content type which are essential.
        /// we do the same checks as the previus request for all of them. Then we check if the content type is either movie or tv series because we can get games
        /// comercials and etc. and we dont want to add them to the db also we check if the movie is already in the db by the imdbId. If the movie is already in the db go to the
        /// movie and do the same checks. If everything is ok then we check if the data is ok first we check if there is discription, because some old 
        /// movies dont have descriptions if there isnt a description we set a defaut one. If we have a description we check if its longer than 500 charecters if it is
        /// we create a substring of 500 charecters and we find the last full stop and we save that. Then we check if we got a thumbnail if we do not 
        /// we add a defaut one. Then we check if we have a maturity rating if we dont we set it as null, if we do , we check if the maturity rating is longer than 5 charecters
        /// because if the data is 5 or more chars. its not maturity rating and we dont want to save that so we set it as null if everything is ok with the
        /// maturity rating we set it as it is. Then we check if the imdb rating is null if its not we parse it into double and save it. Then we check if the 
        /// duration is null if it isnt we split the string from the first " " we get because we want to save it as int and the api returns it in this format 122 min, then we
        /// parse it into int and save it. Then we start adding the genres, we want to add 3 or less genres so we check if we got less and we do we add less
        /// first we check if the genres are already in the genres db table if they are we dont add them there we just make the connection between the genre and the movie
        /// in the moviegenre db table. We do more or less the same thing with the staff but we check if the specific person is in the staff
        /// table with the same role so someone can be a writer, actor or director. We only want to add 1 director and writer and 3 actors we go have less
        /// we add less if we dont have any we dont any. last of all we send all the data we got via entity framework and we add 1 to added movies 
        /// so we can return the number of movies added . We check if that number is less than one if it is we throw an exception
        /// 
        /// 
        /// 
        /// </summary>
        /// <param name="title">the title of the movie/s we want to add</param>
        /// <param name="count">the number of the movie/s we want to add</param>
        /// <returns></returns>
        public int LoadMoviesIntoDB(string title ,int count)
        {
            SilverScreenContext context = new SilverScreenContext();
            if(count < 1)
            {
                throw new Exception("Please enter a valid number");
            }
            int addedMovies = 0; 
            var movieCount = count;
            string[] apiKeys = {"k_t9h2vl7k", "k_mfd5skue", "k_xahgruqu", "k_44lmaclu" , "k_k15dcusr" , "k_faxyw40f" };
            int keyCount = 0;
            string API_KEY = apiKeys[keyCount];
            string url = "https://imdb-api.com/API/AdvancedSearch/" + API_KEY;
            var client = new RestClient(url);
            var request = new RestRequest();
            request.AddParameter("title", title);
            request.AddParameter("count", count);
            var response = client.Get(request);
            var extractedFilm = JsonSerializer.Deserialize<IMDBQuery>(response.Content);

            while (extractedFilm.errorMessage != null && extractedFilm.errorMessage.Contains(" ") == true)
            {
                keyCount++;
                if (keyCount >= apiKeys.Length)
                {
                    throw new Exception("Sorry we ran out of API calls");
                }
                API_KEY = apiKeys[keyCount];
                url = "https://imdb-api.com/API/AdvancedSearch/" + API_KEY;
                client = new RestClient(url);
                request = new RestRequest();
                request.AddParameter("title", title);
                request.AddParameter("count", count);
                response = client.Get(request);
                extractedFilm = JsonSerializer.Deserialize<IMDBQuery>(response.Content);

            }
            if (extractedFilm.results.Count == 0)
            {
                throw new Exception("Sorry we didnt find any movies with that title");
            }
            else
            {
                if (extractedFilm.results.Count < count)
                {
                    movieCount = extractedFilm.results.Count;
                }
                for (int j = 0; j < movieCount; j++)
                {
                    string imdbId = extractedFilm.results[j].id;
                    string urlTrailer = $"https://imdb-api.com/en/API/Trailer/" + API_KEY + "/" + imdbId;
                    var clientTrailer = new RestClient(urlTrailer);
                    var requestTrailer = new RestRequest();
                    var responseTrailer = clientTrailer.Get(requestTrailer);
                    var extractedTrailer = JsonSerializer.Deserialize<IMDBTrailerLink>(responseTrailer.Content);
                    while (extractedTrailer.errorMessage != null && extractedTrailer.errorMessage.Contains(" ") == true)
                    {
                        keyCount++;
                        if (keyCount >= apiKeys.Length)
                        {
                            throw new Exception("Sorry we ran out of API calls");
                        }
                        API_KEY = apiKeys[keyCount];
                        imdbId = extractedFilm.results[j].id;
                        urlTrailer = $"https://imdb-api.com/en/API/Trailer/" + API_KEY + "/" + imdbId;
                        clientTrailer = new RestClient(urlTrailer);
                        requestTrailer = new RestRequest();
                        responseTrailer = clientTrailer.Get(requestTrailer);
                        extractedTrailer = JsonSerializer.Deserialize<IMDBTrailerLink>(responseTrailer.Content);

                    }
                    string urlCast = "https://imdb-api.com/en/API/FullCast/" + API_KEY + "/" + imdbId;
                    var clientCast = new RestClient(urlCast);
                    var requestCast = new RestRequest();
                    var responseCast = clientCast.Get(requestCast);
                    var extractedCast = JsonSerializer.Deserialize<IMDBMovieCast>(responseCast.Content);
                    while (extractedCast.errorMessage != null && extractedCast.errorMessage.Contains(" ") == true)
                    {
                        keyCount++;
                        if (keyCount >= apiKeys.Length)
                        {
                            throw new Exception("Sorry we ran out of API calls");
                        }
                        API_KEY = apiKeys[keyCount];
                        urlCast = "https://imdb-api.com/en/API/FullCast/" + API_KEY + "/" + imdbId;
                        clientCast = new RestClient(urlCast);
                        requestCast = new RestRequest();
                        responseCast = clientCast.Get(requestCast);
                        extractedCast = JsonSerializer.Deserialize<IMDBMovieCast>(responseCast.Content);
                    }

                    string urlDescription = "https://imdb-api.com/en/API/Title/" + API_KEY + "/" + imdbId;
                    var clientDescription = new RestClient(urlDescription);
                    var requestDescription = new RestRequest();
                    var responseDescription = clientDescription.Get(requestDescription);
                    var extractedDescription = JsonSerializer.Deserialize<IMDBDescription>(responseDescription.Content);
                    while (extractedDescription.errorMessage != null && extractedDescription.errorMessage.Contains(" ") == true)
                    {
                        keyCount++;
                        if (keyCount >= apiKeys.Length)
                        {
                            throw new Exception("Sorry we ran out of API calls");
                        }
                        API_KEY = apiKeys[keyCount];
                        urlDescription = "https://imdb-api.com/en/API/Title/" + API_KEY + "/" + imdbId;
                        clientDescription = new RestClient(urlDescription);
                        requestDescription = new RestRequest();
                        responseDescription = clientDescription.Get(requestDescription);
                        extractedDescription = JsonSerializer.Deserialize<IMDBDescription>(responseDescription.Content);
                    }
                    NumberFormatInfo nfi = new NumberFormatInfo();
                    nfi.NumberDecimalSeparator = ".";


                    if ((extractedDescription.type == "TVSeries" || extractedDescription.type == "Movie") && !context.Movies.Where(x => x.ImdbId.Equals(extractedFilm.results[j].id)).Any())
                    {

                        var movie = new Movie();
                        movie.ImdbId = extractedFilm.results[j].id;
                        movie.Title = extractedFilm.results[j].title;
                        movie.ContentType = extractedDescription.type;
                        if (extractedDescription.plot == null || extractedDescription.plot == "")
                        {
                            movie.Description = "You caught us! We don't have the description yet.";
                        }
                        else
                        {
                            if (extractedDescription.plot.Length > 500)
                            {
                                string shortDescription = extractedDescription.plot.Substring(0, 500);
                                for (int i = shortDescription.Length - 1; i >= 0; i--)
                                {
                                    if (shortDescription[i] == '.')
                                    {
                                        extractedDescription.plot = shortDescription.Substring(0, i + 1);
                                        break;
                                    }


                                }
                            }
                            movie.Description = extractedDescription.plot;
                        }

                        if (extractedFilm.results[j].image == null || extractedFilm.results[j].image == "https://imdb-api.com/images/original/nopicture.jpg")
                        {
                            movie.Thumbnail = "https://iili.io/0pLhOX.png";
                        }
                        else
                        {
                            movie.Thumbnail = extractedFilm.results[j].image;
                        }
                        if (extractedFilm.results[j].contentRating == null)
                        {
                            movie.MaturityRating = extractedFilm.results[j].contentRating;
                        }
                        else
                        {
                            if (extractedFilm.results[j].contentRating.Length > 5)
                            {
                                movie.MaturityRating = null;
                            }
                            else
                            {
                                movie.MaturityRating = extractedFilm.results[j].contentRating;

                            }
                        }
                        if (extractedFilm.results[j].imDbRating == null)
                        {
                            movie.Rating = null;
                        }
                        else
                        {
                            movie.Rating = Double.Parse(extractedFilm.results[j].imDbRating, nfi);
                        }
                        if (extractedFilm.results[j].runtimeStr == null)
                        {
                            movie.Duration = null;
                        }
                        else
                        {
                            movie.Duration = int.Parse(extractedFilm.results[j].runtimeStr.Split(' ')[0]);
                        }

                        movie.Trailer = extractedTrailer.linkEmbed;
                        movie.ReleaseDate = extractedFilm.results[j].description;

                        context.Add(movie);
                        context.SaveChanges();
                        if (extractedFilm.results[j].genreList != null)
                        {
                            var genresCount = 3;
                            if (extractedFilm.results[j].genreList.Count < 3)
                            {
                                genresCount = extractedFilm.results[j].genreList.Count;
                            }
                            for (int i = 0; i < genresCount; i++)
                            {
                                var genres = context.Genres.Where(x => x.Genre1.Equals(extractedFilm.results[j].genreList[i].value));
                                if (genres.Any())
                                {
                                    var movieGenre = new MovieGenre
                                    {
                                        MovieId = context.Movies.Where(x => x.ImdbId.Equals(movie.ImdbId)).FirstOrDefault().Id,
                                        GenreId = genres.FirstOrDefault().Id

                                    };
                                    context.Add(movieGenre);
                                }
                                else
                                {
                                    var genre = new Genre
                                    {
                                        Genre1 = extractedFilm.results[j].genreList[i].value

                                    };
                                    context.Add(genre);
                                    context.SaveChanges();
                                    genres = context.Genres.Where(x => x.Genre1.Equals(extractedFilm.results[j].genreList[i].value));
                                    var movieGenre = new MovieGenre
                                    {
                                        MovieId = context.Movies.Where(x => x.ImdbId.Equals(movie.ImdbId)).FirstOrDefault().Id,
                                        GenreId = genres.FirstOrDefault().Id

                                    };
                                    context.Add(movieGenre);
                                }
                            }
                        }
                        
                        
                        if (extractedCast.directors.items.Count != 0)
                        {
                            var directorsCast = context.staff.Where(x => x.Name.Equals(extractedCast.directors.items[0].name) && x.Position.Equals(extractedCast.directors.job));
                            if (directorsCast != null)
                            {
                                if (directorsCast.Any())
                                {
                                    var movieStaff = new MovieStaff
                                    {
                                        MovieId = context.Movies.Where(x => x.ImdbId.Equals(movie.ImdbId)).FirstOrDefault().Id,
                                        StaffId = directorsCast.FirstOrDefault().Id
                                    };
                                    context.Add(movieStaff);

                                }
                                else
                                {
                                    var director = new staff
                                    {
                                        Name = extractedCast.directors.items[0].name,
                                        Position = "Director"
                                    };
                                    context.Add(director);
                                    context.SaveChanges();
                                    directorsCast = context.staff.Where(x => x.Name.Equals(extractedCast.directors.items[0].name) && x.Position.Equals(extractedCast.directors.job));
                                    var movieStaff = new MovieStaff
                                    {
                                        MovieId = context.Movies.Where(x => x.ImdbId.Equals(movie.ImdbId)).FirstOrDefault().Id,
                                        StaffId = directorsCast.FirstOrDefault().Id
                                    };
                                    context.Add(movieStaff);
                                }
                            }
                        }

                        if (extractedCast.writers.items.Count != 0)
                        {
                            var writersCast = context.staff.Where(x => x.Name.Equals(extractedCast.writers.items[0].name) && x.Position.Equals(extractedCast.writers.job));
                            if (writersCast.Any())
                            {
                                var movieStaff = new MovieStaff
                                {
                                    MovieId = context.Movies.Where(x => x.ImdbId.Equals(movie.ImdbId)).FirstOrDefault().Id,
                                    StaffId = writersCast.FirstOrDefault().Id
                                };
                                context.Add(movieStaff);
                            }
                            else
                            {
                                var writer = new staff
                                {
                                    Name = extractedCast.writers.items[0].name,
                                    Position = "Writer"
                                };
                                context.Add(writer);
                                context.SaveChanges();
                                writersCast = context.staff.Where(x => x.Name.Equals(extractedCast.writers.items[0].name) && x.Position.Equals(extractedCast.writers.job));
                                var movieStaff = new MovieStaff
                                {
                                    MovieId = context.Movies.Where(x => x.ImdbId.Equals(movie.ImdbId)).FirstOrDefault().Id,
                                    StaffId = writersCast.FirstOrDefault().Id
                                };
                                context.Add(movieStaff);
                            }
                        }
                        var actorCount = 3;
                        if (extractedCast.actors.Count < 3)
                        {
                            actorCount = extractedCast.actors.Count;
                        }
                        for (int i = 0; i < actorCount; i++)
                        {
                            var actorsCast = context.staff.Where(x => x.Name.Equals(extractedCast.actors[i].name) && x.Position.Equals("Actor"));

                            if (actorsCast.Any())
                            {
                                var movieStaff = new MovieStaff
                                {
                                    MovieId = context.Movies.Where(x => x.ImdbId.Equals(movie.ImdbId)).FirstOrDefault().Id,
                                    StaffId = actorsCast.FirstOrDefault().Id
                                };
                                context.Add(movieStaff);
                            }
                            else
                            {
                                var actor = new staff
                                {
                                    Name = extractedCast.actors[i].name,
                                    Position = "Actor"
                                };
                                context.staff.Add(actor);
                                context.SaveChanges();
                                actorsCast = context.staff.Where(x => x.Name.Equals(extractedCast.actors[i].name) && x.Position.Equals("Actor"));
                                var movieStaff = new MovieStaff
                                {
                                    MovieId = context.Movies.Where(x => x.ImdbId.Equals(movie.ImdbId)).FirstOrDefault().Id,
                                    StaffId = actorsCast.FirstOrDefault().Id
                                };
                                context.Add(movieStaff);
                            }


                        }
                        context.SaveChanges();
                        addedMovies++;
                    }

                }
            }
            if (addedMovies<1)
            {
                throw new Exception("Sorry we didnt find any movies that aren't already in the DB");
            }
            return addedMovies;


        }
        /// <summary>
        /// This method sends a get request to the imdb-api using rest sharp to get a number of movie/s based on the inputed count.
        /// then the method checks if the count is less than 1 because we can't add a negative number of movies, if the
        /// count is less then 1 we throw and exception. We want to add up coming movies we do that when we set the parameters of todays date 
        /// and to 1 year later. That way the api call only adds movies that are about to release from now to one year later
        /// To use the api we need api keys, the api keys we have have a limit of 100 requests
        /// per day so we've made and array with 6 keys. We use the variable keyCount to know on which key we are at the moment.
        /// after every request we check if we got an error message if we do we add 1 to keyCount and we change the key
        /// to the next one in the array we make the same request with the new key and do the check again this repeats until we get a key that has request,  but if we 
        /// dont have any keys with request we throw an exception. If we've successfully completed the first request that gets most of the data we need.
        /// We check if we got any movies at all if we did not we trow an exception. We check if we got less movies than the wanted amount by the user
        /// and if we did we add the amount found, but if its more we only add the wanted amount. Then we run 3 more request with the imdbId we got for every
        /// movie we got from the previus request. The 3 new request get the Trailer, Staff, Description and Content type which are essential.
        /// we do the same checks as the previus request for all of them. Then we check if the content type is either movie or tv series because we can get games
        /// comercials and etc. and we dont want to add them to the db also we check if the movie is already in the db by the imdbId. If the movie is already in the db go to the
        /// movie and do the same checks. If everything is ok then we check if the data is ok first we check if there is discription, because some old 
        /// movies dont have descriptions if there isnt a description we set a defaut one. If we have a description we check if its longer than 500 charecters if it is
        /// we create a substring of 500 charecters and we find the last full stop and we save that. Then we check if we got a thumbnail if we do not 
        /// we add a defaut one. Then we check if we have a maturity rating if we dont we set it as null, if we do , we check if the maturity rating is longer than 5 charecters
        /// because if the data is 5 or more chars. its not maturity rating and we dont want to save that so we set it as null if everything is ok with the
        /// maturity rating we set it as it is. Then we check if the imdb rating is null if its not we parse it into double and save it. Then we check if the 
        /// duration is null if it isnt we split the string from the first " " we get because we want to save it as int and the api returns it in this format 122 min, then we
        /// parse it into int and save it. Then we start adding the genres, we want to add 3 or less genres so we check if we got less and we do we add less
        /// first we check if the genres are already in the genres db table if they are we dont add them there we just make the connection between the genre and the movie
        /// in the moviegenre db table. We do more or less the same thing with the staff but we check if the specific person is in the staff
        /// table with the same role so someone can be a writer, actor or director. We only want to add 1 director and writer and 3 actors we go have less
        /// we add less if we dont have any we dont any. last of all we send all the data we got via entity framework and we add 1 to added movies 
        /// so we can return the number of movies added . We check if that number is less than one if it is we throw an exception
        /// 
        /// 
        /// 
        /// </summary>
        /// <param name="count">the number of the movie/s we want to add</param>
        /// <returns></returns>
        public int LoadUpComingMoviesIntoDB(int count)
        {
            SilverScreenContext context = new SilverScreenContext();
            if (count < 1)
            {
                throw new Exception("Please enter a valid number");
            }
            int addedMovies = 0;
            var movieCount = count;
            string[] apiKeys = { "k_t9h2vl7k", "k_mfd5skue", "k_xahgruqu", "k_44lmaclu", "k_k15dcusr", "k_faxyw40f" };
            int keyCount = 0;
            string dateToday = DateTime.Now.ToString("yyyy-MM-dd");
            string dateOneYearLater = DateTime.Now.AddYears(1).ToString("yyyy-MM-dd");
            string dates = dateToday+","+dateOneYearLater;
            string API_KEY = apiKeys[keyCount];
            string url = "https://imdb-api.com/API/AdvancedSearch/" + API_KEY;
            var client = new RestClient(url);
            var request = new RestRequest();
            request.AddParameter("release_date", dates);
            request.AddParameter("count", count);
            var response = client.Get(request);
            var extractedFilm = JsonSerializer.Deserialize<IMDBQuery>(response.Content);

            while (extractedFilm.errorMessage != null && extractedFilm.errorMessage.Contains(" ") == true)
            {
                keyCount++;
                if (keyCount >= apiKeys.Length)
                {
                    throw new Exception("Sorry we ran out of API calls");
                }
                API_KEY = apiKeys[keyCount];
                url = "https://imdb-api.com/API/AdvancedSearch/" + API_KEY;
                client = new RestClient(url);
                request = new RestRequest();
                request.AddParameter("release_date", dates);
                request.AddParameter("count", count);
                response = client.Get(request);
                extractedFilm = JsonSerializer.Deserialize<IMDBQuery>(response.Content);

            }
            if (extractedFilm.results.Count == 0)
            {
                throw new Exception("Sorry we didnt find any up coming movies");
            }
            else
            {
                if (extractedFilm.results.Count < count)
                {
                    movieCount = extractedFilm.results.Count;
                }
                for (int j = 0; j < movieCount; j++)
                {
                    string imdbId = extractedFilm.results[j].id;
                    string urlTrailer = $"https://imdb-api.com/en/API/Trailer/" + API_KEY + "/" + imdbId;
                    var clientTrailer = new RestClient(urlTrailer);
                    var requestTrailer = new RestRequest();
                    var responseTrailer = clientTrailer.Get(requestTrailer);
                    var extractedTrailer = JsonSerializer.Deserialize<IMDBTrailerLink>(responseTrailer.Content);
                    while (extractedTrailer.errorMessage != null && extractedTrailer.errorMessage.Contains(" ") == true)
                    {
                        keyCount++;
                        if (keyCount >= apiKeys.Length)
                        {
                            throw new Exception("Sorry we ran out of API calls");
                        }
                        API_KEY = apiKeys[keyCount];
                        imdbId = extractedFilm.results[j].id;
                        urlTrailer = $"https://imdb-api.com/en/API/Trailer/" + API_KEY + "/" + imdbId;
                        clientTrailer = new RestClient(urlTrailer);
                        requestTrailer = new RestRequest();
                        responseTrailer = clientTrailer.Get(requestTrailer);
                        extractedTrailer = JsonSerializer.Deserialize<IMDBTrailerLink>(responseTrailer.Content);

                    }
                    string urlCast = "https://imdb-api.com/en/API/FullCast/" + API_KEY + "/" + imdbId;
                    var clientCast = new RestClient(urlCast);
                    var requestCast = new RestRequest();
                    var responseCast = clientCast.Get(requestCast);
                    var extractedCast = JsonSerializer.Deserialize<IMDBMovieCast>(responseCast.Content);
                    while (extractedCast.errorMessage != null && extractedCast.errorMessage.Contains(" ") == true)
                    {
                        keyCount++;
                        if (keyCount >= apiKeys.Length)
                        {
                            throw new Exception("Sorry we ran out of API calls");
                        }
                        API_KEY = apiKeys[keyCount];
                        urlCast = "https://imdb-api.com/en/API/FullCast/" + API_KEY + "/" + imdbId;
                        clientCast = new RestClient(urlCast);
                        requestCast = new RestRequest();
                        responseCast = clientCast.Get(requestCast);
                        extractedCast = JsonSerializer.Deserialize<IMDBMovieCast>(responseCast.Content);
                    }

                    string urlDescription = "https://imdb-api.com/en/API/Title/" + API_KEY + "/" + imdbId;
                    var clientDescription = new RestClient(urlDescription);
                    var requestDescription = new RestRequest();
                    var responseDescription = clientDescription.Get(requestDescription);
                    var extractedDescription = JsonSerializer.Deserialize<IMDBDescription>(responseDescription.Content);
                    while (extractedDescription.errorMessage != null && extractedDescription.errorMessage.Contains(" ") == true)
                    {
                        keyCount++;
                        if (keyCount >= apiKeys.Length)
                        {
                            throw new Exception("Sorry we ran out of API calls");
                        }
                        API_KEY = apiKeys[keyCount];
                        urlDescription = "https://imdb-api.com/en/API/Title/" + API_KEY + "/" + imdbId;
                        clientDescription = new RestClient(urlDescription);
                        requestDescription = new RestRequest();
                        responseDescription = clientDescription.Get(requestDescription);
                        extractedDescription = JsonSerializer.Deserialize<IMDBDescription>(responseDescription.Content);
                    }
                    NumberFormatInfo nfi = new NumberFormatInfo();
                    nfi.NumberDecimalSeparator = ".";


                    if ((extractedDescription.type == "TVSeries" || extractedDescription.type == "Movie") && !context.Movies.Where(x => x.ImdbId.Equals(extractedFilm.results[j].id)).Any())
                    {

                        var movie = new Movie();
                        movie.ImdbId = extractedFilm.results[j].id;
                        movie.Title = extractedFilm.results[j].title;
                        movie.ContentType = extractedDescription.type;
                        if(extractedDescription.releaseDate!=null || extractedDescription.releaseDate != "")
                        {
                            
                            movie.SpecificReleaseDate = Convert.ToDateTime(extractedDescription.releaseDate);
                           
                        }   
                        if (extractedDescription.plot == null || extractedDescription.plot == "")
                        {
                            movie.Description = "You caught us! We don't have the description yet.";
                        }
                        else
                        {
                            if (extractedDescription.plot.Length > 500)
                            {
                                string shortDescription = extractedDescription.plot.Substring(0, 500);
                                for (int i = shortDescription.Length - 1; i >= 0; i--)
                                {
                                    if (shortDescription[i] == '.')
                                    {
                                        extractedDescription.plot = shortDescription.Substring(0, i + 1);
                                        break;
                                    }


                                }
                            }
                            movie.Description = extractedDescription.plot;
                        }

                        if (extractedFilm.results[j].image == null || extractedFilm.results[j].image == "https://imdb-api.com/images/original/nopicture.jpg")
                        {
                            movie.Thumbnail = "https://iili.io/0pLhOX.png";
                        }
                        else
                        {
                            movie.Thumbnail = extractedFilm.results[j].image;
                        }
                        if (extractedFilm.results[j].contentRating == null)
                        {
                            movie.MaturityRating = extractedFilm.results[j].contentRating;
                        }
                        else
                        {
                            if (extractedFilm.results[j].contentRating.Length > 5)
                            {
                                movie.MaturityRating = null;
                            }
                            else
                            {
                                movie.MaturityRating = extractedFilm.results[j].contentRating;

                            }
                        }
                        if (extractedFilm.results[j].imDbRating == null)
                        {
                            movie.Rating = null;
                        }
                        else
                        {
                            movie.Rating = Double.Parse(extractedFilm.results[j].imDbRating, nfi);
                        }
                        if (extractedFilm.results[j].runtimeStr == null)
                        {
                            movie.Duration = null;
                        }
                        else
                        {
                            movie.Duration = int.Parse(extractedFilm.results[j].runtimeStr.Split(' ')[0]);
                        }

                        movie.Trailer = extractedTrailer.linkEmbed;
                        movie.ReleaseDate = extractedFilm.results[j].description;

                        context.Add(movie);
                        context.SaveChanges();
                        if (extractedFilm.results[j].genreList != null)
                        {
                            var genresCount = 3;
                            if (extractedFilm.results[j].genreList.Count < 3)
                            {
                                genresCount = extractedFilm.results[j].genreList.Count;
                            }
                            for (int i = 0; i < genresCount; i++)
                            {
                                var genres = context.Genres.Where(x => x.Genre1.Equals(extractedFilm.results[j].genreList[i].value));
                                if (genres.Any())
                                {
                                    var movieGenre = new MovieGenre
                                    {
                                        MovieId = context.Movies.Where(x => x.ImdbId.Equals(movie.ImdbId)).FirstOrDefault().Id,
                                        GenreId = genres.FirstOrDefault().Id

                                    };
                                    context.Add(movieGenre);
                                }
                                else
                                {
                                    var genre = new Genre
                                    {
                                        Genre1 = extractedFilm.results[j].genreList[i].value

                                    };
                                    context.Add(genre);
                                    context.SaveChanges();
                                    genres = context.Genres.Where(x => x.Genre1.Equals(extractedFilm.results[j].genreList[i].value));
                                    var movieGenre = new MovieGenre
                                    {
                                        MovieId = context.Movies.Where(x => x.ImdbId.Equals(movie.ImdbId)).FirstOrDefault().Id,
                                        GenreId = genres.FirstOrDefault().Id

                                    };
                                    context.Add(movieGenre);
                                }
                            }
                        }


                        if (extractedCast.directors.items.Count != 0)
                        {
                            var directorsCast = context.staff.Where(x => x.Name.Equals(extractedCast.directors.items[0].name) && x.Position.Equals(extractedCast.directors.job));
                            if (directorsCast != null)
                            {
                                if (directorsCast.Any())
                                {
                                    var movieStaff = new MovieStaff
                                    {
                                        MovieId = context.Movies.Where(x => x.ImdbId.Equals(movie.ImdbId)).FirstOrDefault().Id,
                                        StaffId = directorsCast.FirstOrDefault().Id
                                    };
                                    context.Add(movieStaff);

                                }
                                else
                                {
                                    var director = new staff
                                    {
                                        Name = extractedCast.directors.items[0].name,
                                        Position = "Director"
                                    };
                                    context.Add(director);
                                    context.SaveChanges();
                                    directorsCast = context.staff.Where(x => x.Name.Equals(extractedCast.directors.items[0].name) && x.Position.Equals(extractedCast.directors.job));
                                    var movieStaff = new MovieStaff
                                    {
                                        MovieId = context.Movies.Where(x => x.ImdbId.Equals(movie.ImdbId)).FirstOrDefault().Id,
                                        StaffId = directorsCast.FirstOrDefault().Id
                                    };
                                    context.Add(movieStaff);
                                }
                            }
                        }

                        if (extractedCast.writers.items.Count != 0)
                        {
                            var writersCast = context.staff.Where(x => x.Name.Equals(extractedCast.writers.items[0].name) && x.Position.Equals(extractedCast.writers.job));
                            if (writersCast.Any())
                            {
                                var movieStaff = new MovieStaff
                                {
                                    MovieId = context.Movies.Where(x => x.ImdbId.Equals(movie.ImdbId)).FirstOrDefault().Id,
                                    StaffId = writersCast.FirstOrDefault().Id
                                };
                                context.Add(movieStaff);
                            }
                            else
                            {
                                var writer = new staff
                                {
                                    Name = extractedCast.writers.items[0].name,
                                    Position = "Writer"
                                };
                                context.Add(writer);
                                context.SaveChanges();
                                writersCast = context.staff.Where(x => x.Name.Equals(extractedCast.writers.items[0].name) && x.Position.Equals(extractedCast.writers.job));
                                var movieStaff = new MovieStaff
                                {
                                    MovieId = context.Movies.Where(x => x.ImdbId.Equals(movie.ImdbId)).FirstOrDefault().Id,
                                    StaffId = writersCast.FirstOrDefault().Id
                                };
                                context.Add(movieStaff);
                            }
                        }
                        var actorCount = 3;
                        if (extractedCast.actors.Count < 3)
                        {
                            actorCount = extractedCast.actors.Count;
                        }
                        for (int i = 0; i < actorCount; i++)
                        {
                            var actorsCast = context.staff.Where(x => x.Name.Equals(extractedCast.actors[i].name) && x.Position.Equals("Actor"));

                            if (actorsCast.Any())
                            {
                                var movieStaff = new MovieStaff
                                {
                                    MovieId = context.Movies.Where(x => x.ImdbId.Equals(movie.ImdbId)).FirstOrDefault().Id,
                                    StaffId = actorsCast.FirstOrDefault().Id
                                };
                                context.Add(movieStaff);
                            }
                            else
                            {
                                var actor = new staff
                                {
                                    Name = extractedCast.actors[i].name,
                                    Position = "Actor"
                                };
                                context.staff.Add(actor);
                                context.SaveChanges();
                                actorsCast = context.staff.Where(x => x.Name.Equals(extractedCast.actors[i].name) && x.Position.Equals("Actor"));
                                var movieStaff = new MovieStaff
                                {
                                    MovieId = context.Movies.Where(x => x.ImdbId.Equals(movie.ImdbId)).FirstOrDefault().Id,
                                    StaffId = actorsCast.FirstOrDefault().Id
                                };
                                context.Add(movieStaff);
                            }


                        }
                        context.SaveChanges();
                        addedMovies++;
                    }

                }
            }
            if (addedMovies < 1)
            {
                throw new Exception("Sorry we didnt find any movies that aren't already in the DB");
            }
            return addedMovies;


        }
    }
}
