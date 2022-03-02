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
        public void LoadMoviesIntoDB(string title ,int count)
        {
            string API_KEY = "k_mfd5skue"; //k_faxyw40f //k_mfd5skue k_44lmaclu
            SilverScreenContext context = new SilverScreenContext();

            string url = "https://imdb-api.com/API/AdvancedSearch/" + API_KEY;
            var client = new RestClient(url);
            var request = new RestRequest();
            request.AddParameter("title", title);
            request.AddParameter("count", count);
            var response = client.Get(request);
            var extractedFilm = JsonSerializer.Deserialize<IMDBQuery>(response.Content);
            var movieCount = count;
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

                string urlCast = "https://imdb-api.com/en/API/FullCast/" + API_KEY + "/" + imdbId;
                var clientCast = new RestClient(urlCast);
                var requestCast = new RestRequest();
                var responseCast = clientCast.Get(requestCast);
                var extractedCast = JsonSerializer.Deserialize<IMDBMovieCast>(responseCast.Content);

                string urlDescription = "https://imdb-api.com/en/API/Title/" + API_KEY + "/" + imdbId;
                var clientDescription = new RestClient(urlDescription);
                var requestDescription = new RestRequest();
                var responseDescription = clientDescription.Get(requestDescription);
                var extractedDescription = JsonSerializer.Deserialize<IMDBDescription>(responseDescription.Content);

                NumberFormatInfo nfi = new NumberFormatInfo();
                nfi.NumberDecimalSeparator = ".";
                
                
                if(extractedDescription.type == "TVSeries" || extractedDescription.type == "Movie")
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
                            for (int i = shortDescription.Length-1; i >= 0; i--)
                            {
                                if (shortDescription[i] == '.')
                                {
                                    extractedDescription.plot = shortDescription.Substring(0, i+1);
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
   
                    if(extractedCast.writers.items.Count != 0 )
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
                }
                
            }
            
        }
    }
}
