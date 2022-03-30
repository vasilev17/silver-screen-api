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
        /// and if we did we add the amount found, but if its more we only add the wanted amount. Then we run 3 more request with the TmdbId we got for every
        /// movie we got from the previus request. The 3 new request get the Trailer, Staff, Description and Content type which are essential.
        /// we do the same checks as the previus request for all of them. Then we check if the content type is either movie or tv series because we can get games
        /// comercials and etc. and we dont want to add them to the db also we check if the movie is already in the db by the TmdbId. If the movie is already in the db go to the
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

        public int LoadMoviesIntoDBviaTMDB(string title, int count, string contentType)
        {
            SilverScreenContext context = new SilverScreenContext();
            if (count < 1)
            {
                throw new Exception("Please enter a valid number");
            }
            int addedMovies = 0;
            var movieCount = count;
            string currentContentType;
            if (contentType == "tv")
            {
                currentContentType = "TVSeries";
            }
            else
            {
                currentContentType = "Movie";
            }
            string API_KEY = "990b1ebdae34eb39da96a2a37e0bbaf9";
            string url = "http://api.themoviedb.org/3/search/" + contentType + "/";
            var client = new RestClient(url);
            var request = new RestRequest();
            request.AddParameter("api_key", API_KEY);
            request.AddParameter("query", title);
            var response = client.Get(request);
            var extractedFilm = JsonSerializer.Deserialize<TMDBQuery>(response.Content);


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
                    if (!context.Movies.Any(x => x.TmdbId == extractedFilm.results[j].id && x.ContentType == currentContentType))
                    {
                        string urlDescription = "https://api.themoviedb.org/3/" + contentType + "/" + extractedFilm.results[j].id;
                        var clientDescription = new RestClient(urlDescription);
                        var requestDescription = new RestRequest();
                        requestDescription.AddParameter("api_key", API_KEY);
                        var responseDescription = clientDescription.Get(requestDescription);
                        var extractedDescription = JsonSerializer.Deserialize<TMDBDescription>(responseDescription.Content);
                        if (extractedDescription.adult == false)
                        {
                            string urlTrailer = $"https://api.themoviedb.org/3/" + contentType + "/" + extractedFilm.results[j].id + "/videos";
                            var clientTrailer = new RestClient(urlTrailer);
                            var requestTrailer = new RestRequest();
                            requestTrailer.AddParameter("api_key", API_KEY);
                            var responseTrailer = clientTrailer.Get(requestTrailer);
                            var extractedTrailer = JsonSerializer.Deserialize<TMDBTrailerLink>(responseTrailer.Content);

                            string urlCast = "https://api.themoviedb.org/3/" + contentType + "/" + extractedFilm.results[j].id + "/credits";
                            var clientCast = new RestClient(urlCast);
                            var requestCast = new RestRequest();
                            requestCast.AddParameter("api_key", API_KEY);
                            var responseCast = clientCast.Get(requestCast);
                            var extractedCast = JsonSerializer.Deserialize<TMDBCast>(responseCast.Content);

                            NumberFormatInfo nfi = new NumberFormatInfo();
                            nfi.NumberDecimalSeparator = ".";

                            var movie = new Movie();

                            movie.TmdbId = extractedFilm.results[j].id;
                            movie.ContentType = currentContentType;
                            if (contentType == "movie")
                            {
                                if (extractedDescription.runtime == 0)
                                {
                                    movie.Duration = null;
                                }
                                else
                                {
                                    movie.Duration = extractedDescription.runtime;
                                }
                                movie.ReleaseDate = extractedFilm.results[j].release_date;
                                movie.Title = extractedFilm.results[j].title;
                            }
                            else
                            {
                                if (extractedDescription.episode_run_time.Count == 0)
                                {
                                    movie.Duration = null;
                                }
                                else if (extractedDescription.episode_run_time[0] != 0)
                                {
                                    movie.Duration = extractedDescription.episode_run_time[0];
                                }
                                movie.ReleaseDate = extractedFilm.results[j].first_air_date;
                                movie.Title = extractedFilm.results[j].name;
                            }
                            if (extractedFilm.results[j].overview == null || extractedFilm.results[j].overview == "")
                            {
                                movie.Description = "You caught us! We don't have the description yet.";
                            }
                            else
                            {
                                if (extractedFilm.results[j].overview.Length > 500)
                                {
                                    string shortDescription = extractedFilm.results[j].overview.Substring(0, 500);
                                    for (int i = shortDescription.Length - 1; i >= 0; i--)
                                    {
                                        if (shortDescription[i] == '.')
                                        {
                                            extractedFilm.results[j].overview = shortDescription.Substring(0, i + 1);
                                            break;
                                        }
                                    }
                                }
                                movie.Description = extractedFilm.results[j].overview;
                            }

                            if (extractedFilm.results[j].poster_path == null)
                            {
                                movie.Thumbnail = "https://iili.io/0pLhOX.png";
                            }
                            else
                            {
                                movie.Thumbnail = "https://image.tmdb.org/t/p/original/" + extractedFilm.results[j].poster_path;
                            }
                            if (extractedFilm.results[j].backdrop_path == null)
                            {
                                movie.Bgimage = movie.Thumbnail;
                            }
                            else
                            {
                                movie.Bgimage = "https://image.tmdb.org/t/p/original/" + extractedFilm.results[j].backdrop_path;
                            }

                            if (extractedFilm.results[j].vote_average == 0.0 || extractedFilm.results[j].vote_average == 0)
                            {
                                movie.Rating = null;
                            }
                            else
                            {
                                movie.Rating = extractedFilm.results[j].vote_average;
                            }
                            if (extractedTrailer.results.Count != 0)
                            {
                                if (extractedTrailer.results[0].site == "YouTube")
                                {
                                    movie.Trailer = "https://www.youtube.com/embed/" + extractedTrailer.results[0].key;
                                }
                                else
                                {
                                    movie.Trailer = "https://player.vimeo.com/video/" + extractedTrailer.results[0].key;
                                }

                            }
                            context.Add(movie);
                            context.SaveChanges();
                            if (extractedDescription.genres != null)
                            {
                                for (int i = 0; i < extractedDescription.genres.Count; i++)
                                {
                                    if (extractedDescription.genres[i].name.Contains("&"))
                                    {
                                        char[] genreSeparator = { ' ', '&' };
                                        String[] extractedGenreList = extractedDescription.genres[i].name.Split(genreSeparator, StringSplitOptions.RemoveEmptyEntries);
                                        for (int k = 0; k < extractedGenreList.Length; k++)
                                        {
                                            if (i + k >= extractedDescription.genres.Count)
                                            {
                                                extractedDescription.genres.Add(new TMDBGenres { name = extractedGenreList[k] });
                                            }
                                            else
                                            {
                                                extractedDescription.genres[i + k].name = extractedGenreList[k];
                                            }
                                        }
                                    }
                                }
                                var genresCount = 3;
                                if (extractedDescription.genres.Count < 3)
                                {
                                    genresCount = extractedDescription.genres.Count;
                                }
                                for (int i = 0; i < genresCount; i++)
                                {
                                    var genres = context.Genres.Where(x => x.Genre1.Equals(extractedDescription.genres[i].name));
                                    if (genres.Any())
                                    {
                                        var movieGenre = new MovieGenre
                                        {
                                            MovieId = context.Movies.Where(x => x.TmdbId.Equals(movie.TmdbId)).FirstOrDefault().Id,
                                            GenreId = genres.FirstOrDefault().Id

                                        };
                                        context.Add(movieGenre);
                                    }
                                    else
                                    {
                                        var genre = new Genre
                                        {
                                            Genre1 = extractedDescription.genres[i].name

                                        };
                                        context.Add(genre);
                                        context.SaveChanges();
                                        genres = context.Genres.Where(x => x.Genre1.Equals(extractedDescription.genres[i].name));
                                        var movieGenre = new MovieGenre
                                        {
                                            MovieId = context.Movies.Where(x => x.TmdbId.Equals(movie.TmdbId)).FirstOrDefault().Id,
                                            GenreId = genres.FirstOrDefault().Id

                                        };
                                        context.Add(movieGenre);
                                    }
                                }
                            }
                            int actorCount = 3;
                            var actors = extractedCast.cast.Where(x => x.known_for_department.Equals("Acting")).ToList();
                            if (actors.Count < actorCount)
                            {
                                actorCount = actors.Count;
                            }
                            for (int i = 0; i < actorCount; i++)
                            {
                                var actorsCast = context.staff.Where(x => x.Name.Equals(actors[i].name) && x.Position.Equals("Actor"));

                                if (actorsCast.Any())
                                {
                                    var movieStaff = new MovieStaff
                                    {
                                        MovieId = context.Movies.Where(x => x.TmdbId.Equals(movie.TmdbId)).FirstOrDefault().Id,
                                        StaffId = actorsCast.FirstOrDefault().Id
                                    };
                                    context.Add(movieStaff);
                                }
                                else
                                {
                                    var actor = new staff
                                    {
                                        Name = actors[i].name,
                                        Position = "Actor"
                                    };
                                    context.staff.Add(actor);
                                    context.SaveChanges();
                                    actorsCast = context.staff.Where(x => x.Name.Equals(actors[i].name) && x.Position.Equals("Actor"));
                                    var movieStaff = new MovieStaff
                                    {
                                        MovieId = context.Movies.Where(x => x.TmdbId.Equals(movie.TmdbId)).FirstOrDefault().Id,
                                        StaffId = actorsCast.FirstOrDefault().Id
                                    };
                                    context.Add(movieStaff);
                                }
                            }
                            var directors = extractedCast.crew.Where(x => x.job != null ? x.job.Equals("Director") : false).ToList();
                            if (directors.Count != 0)
                            {
                                var directorsCast = context.staff.Where(x => x.Name.Equals(directors[0].name) && x.Position.Equals("Director"));
                                if (directorsCast != null)
                                {
                                    if (directorsCast.Any())
                                    {
                                        var movieStaff = new MovieStaff
                                        {
                                            MovieId = context.Movies.Where(x => x.TmdbId.Equals(movie.TmdbId)).FirstOrDefault().Id,
                                            StaffId = directorsCast.FirstOrDefault().Id
                                        };
                                        context.Add(movieStaff);

                                    }
                                    else
                                    {
                                        var director = new staff
                                        {
                                            Name = directors[0].name,
                                            Position = "Director"
                                        };
                                        context.Add(director);
                                        context.SaveChanges();
                                        directorsCast = context.staff.Where(x => x.Name.Equals(directors[0].name) && x.Position.Equals("Director"));
                                        var movieStaff = new MovieStaff
                                        {
                                            MovieId = context.Movies.Where(x => x.TmdbId.Equals(movie.TmdbId)).FirstOrDefault().Id,
                                            StaffId = directorsCast.FirstOrDefault().Id
                                        };
                                        context.Add(movieStaff);
                                    }
                                }
                            }
                            var writers = extractedCast.crew.Where(x => x.job != null ? (x.job.Equals("Writer") || x.job.Equals("Novel") || x.job.Equals("Author")) : false).ToList();
                            if (writers.Count != 0)
                            {
                                var writersCast = context.staff.Where(x => x.Name.Equals(writers[0].name) && x.Position.Equals("Writer"));
                                if (writersCast.Any())
                                {
                                    var movieStaff = new MovieStaff
                                    {
                                        MovieId = context.Movies.Where(x => x.TmdbId.Equals(movie.TmdbId)).FirstOrDefault().Id,
                                        StaffId = writersCast.FirstOrDefault().Id
                                    };
                                    context.Add(movieStaff);
                                }
                                else
                                {
                                    var writer = new staff
                                    {
                                        Name = writers[0].name,
                                        Position = "Writer"
                                    };
                                    context.Add(writer);
                                    context.SaveChanges();
                                    writersCast = context.staff.Where(x => x.Name.Equals(writers[0].name) && x.Position.Equals("Writer"));
                                    var movieStaff = new MovieStaff
                                    {
                                        MovieId = context.Movies.Where(x => x.TmdbId.Equals(movie.TmdbId)).FirstOrDefault().Id,
                                        StaffId = writersCast.FirstOrDefault().Id
                                    };
                                    context.Add(movieStaff);
                                }
                            }
                            context.SaveChanges();
                            addedMovies++;
                        }

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


