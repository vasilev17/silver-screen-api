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
        /// This method sends a get request to the imdb-api using rest sharp to get a movie from it's title
        /// with the movie data from the first request, it uses the imdbId to send two more get requests to get the trailer and the actors
        /// then it sends the data gathered to the database using entity framework
        /// </summary>
        /// <param name="title">The method uses this string to send a get request for the particular movie we want to add to the database</param>
        public void LoadMovieIntoDB(string title)
        {
            string API_KEY = "k_44lmaclu";
            SilverScreenContext context = new SilverScreenContext(); 

            string url = "https://imdb-api.com/API/AdvancedSearch/" + API_KEY;
            var client = new RestClient(url);
            var request = new RestRequest();
            request.AddParameter("title", title);
            request.AddParameter("count", "1");
            var response = client.Get(request);
            var extractedFilm = JsonSerializer.Deserialize<IMDBQuery>(response.Content);

            string imdbId = extractedFilm.results[0].id;
            string urlTrailer = $"https://imdb-api.com/en/API/Trailer/"+ API_KEY + "/" + imdbId;
            var clientTrailer = new RestClient(urlTrailer);
            var requestTrailer = new RestRequest();
            var responseTrailer = clientTrailer.Get(requestTrailer);
            var extractedTrailer = JsonSerializer.Deserialize<IMDBTrailerLink>(responseTrailer.Content);

            string urlCast = "https://imdb-api.com/en/API/FullCast/" + API_KEY + "/" + imdbId;
            var clientCast = new RestClient(urlCast);
            var requestCast = new RestRequest();
            var responseCast = clientCast.Get(requestCast);
            var extractedCast = JsonSerializer.Deserialize<IMDBMovieCast>(responseCast.Content);

            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";
            var movie = new Movie();
          
                movie.ImdbId = extractedFilm.results[0].id;
                movie.Title = extractedFilm.results[0].title;
                if (extractedFilm.results[0].plot == null)
                {
                    movie.Description = "You caught us! We don't have the description yet.";
                }
                else
                {
                    movie.Description = extractedFilm.results[0].plot;
                }

                if (extractedFilm.results[0].image == null)
                {
                    movie.Thumbnail = "https://iili.io/0pLhOX.png";
                }
                else
                {
                    movie.Thumbnail = extractedFilm.results[0].image;
                }
                movie.Rating = Double.Parse(extractedFilm.results[0].imDbRating, nfi);
                movie.Duration = int.Parse(extractedFilm.results[0].runtimeStr.Split(' ')[0]);
                movie.MaturityRating = extractedFilm.results[0].contentRating;
                movie.Trailer = extractedTrailer.linkEmbed;
                movie.ReleaseDate = extractedFilm.results[0].description;
  
            context.Add(movie);
            context.SaveChanges();

            var genresCount = 3;
            if (extractedFilm.results[0].genreList.Count<3)
            {
                genresCount = extractedFilm.results[0].genreList.Count;
            }
            for (int i = 0; i < genresCount; i++)
            {
                var genres = context.Genres.Where(x => x.Genre1.Equals(extractedFilm.results[0].genreList[i].value));
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
                        Genre1 = extractedFilm.results[0].genreList[i].value
                       
                    };
                    context.Add(genre);
                    context.SaveChanges();
                    genres = context.Genres.Where(x => x.Genre1.Equals(extractedFilm.results[0].genreList[i].value));
                    var movieGenre = new MovieGenre
                    {
                        MovieId = context.Movies.Where(x => x.ImdbId.Equals(movie.ImdbId)).FirstOrDefault().Id,
                        GenreId = genres.FirstOrDefault().Id

                    };
                    context.Add(movieGenre);
                }
            }
            var directorsCast = context.staff.Where(x => x.Name.Equals(extractedCast.directors.items[0].name) && x.Position.Equals(extractedCast.directors.job));
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
            for (int i = 0; i < 3; i++)
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
        }
        public void Load25MoviesIntoDB(string title)
        {
            string API_KEY = "k_44lmaclu";
            SilverScreenContext context = new SilverScreenContext();

            string url = "https://imdb-api.com/API/AdvancedSearch/" + API_KEY;
            var client = new RestClient(url);
            var request = new RestRequest();
            request.AddParameter("title", title);
            request.AddParameter("count", "25");
            var response = client.Get(request);
            var extractedFilm = JsonSerializer.Deserialize<IMDBQuery>(response.Content);
            for (int j = 0; j < 25; j++)
            {
                string imdbId = extractedFilm.results[j].id;
                string urlTrailer = $"https://imdb-api.com/en/API/Trailer/" + API_KEY + "/" + imdbId;
                var clientTrailer = new RestClient(urlTrailer);
                var requestTrailer = new RestRequest();
                var responseTrailer = clientTrailer.Get(requestTrailer);
                var extractedTrailer = JsonSerializer.Deserialize<IMDBTrailerLink>(responseTrailer.Content);

                string urlCast = "https://imdb-api.com/en/API/FullCast/" + API_KEY + "/" + imdbId;
                var clientCast = new RestClient(urlCast);
                var requestCast = new RestRequest();
                var responseCast = clientCast.Get(requestCast);
                var extractedCast = JsonSerializer.Deserialize<IMDBMovieCast>(responseCast.Content);

                NumberFormatInfo nfi = new NumberFormatInfo();
                nfi.NumberDecimalSeparator = ".";
                var movie = new Movie();

                movie.ImdbId = extractedFilm.results[j].id;
                movie.Title = extractedFilm.results[j].title;
                if (extractedFilm.results[j].plot == null)
                {
                    movie.Description = "You caught us! We don't have the description yet.";
                }
                else
                {
                    movie.Description = extractedFilm.results[0].plot;
                }

                if (extractedFilm.results[j].image == null)
                {
                    movie.Thumbnail = "https://iili.io/0pLhOX.png";
                }
                else
                {
                    movie.Thumbnail = extractedFilm.results[j].image;
                }
                movie.Rating = Double.Parse(extractedFilm.results[j].imDbRating, nfi);
                movie.Duration = int.Parse(extractedFilm.results[j].runtimeStr.Split(' ')[0]);
                movie.MaturityRating = extractedFilm.results[j].contentRating;
                movie.Trailer = extractedTrailer.linkEmbed;
                movie.ReleaseDate = extractedFilm.results[j].description;

                context.Add(movie);
                context.SaveChanges();

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
                var directorsCast = context.staff.Where(x => x.Name.Equals(extractedCast.directors.items[0].name) && x.Position.Equals(extractedCast.directors.job));
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
                for (int i = 0; i < 3; i++)
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
            }
            
        }
    }
}
