using System.Collections.Generic;
using System.Linq;
using Apollo.Domain.Entity;
using Apollo.Import.IMBD;
using Apollo.Util;
using Microsoft.Extensions.Configuration;
using RestSharp;

namespace Apollo.Import.Services
{
    public static class ImdbHttpService
    {
        private const string Top250 = "https://imdb-api.com/en/API/Top250Movies/";
        private const string MovieURL = "https://imdb-api.com/en/API/Title/";
        private const string OptionPostfix = "/FullActor,Images,Trailer";
        private static readonly string ApiKey;
        private const int MaxRating = 5;


        static ImdbHttpService()
        {
            ConfigurationHelper.ConfigurationRoot = new ConfigurationBuilder()
                .AddJsonFile("appsettings.import.json")
                .Build();
            var values = ConfigurationHelper.GetValues("API_KEY");
            if (values.Length > 0)
            {
                ApiKey = values[0];
            }
        }

        public static IList<Movie> GetImdbData(out ISet<Genre> genres, out IDictionary<string, Actor> actors, out ISet<MovieActor> movieActors)
        {
            var titles = FetchMovies().ToList();
            genres = new HashSet<Genre>();
            actors = new Dictionary<string, Actor>();
            movieActors = new HashSet<MovieActor>();

            var genresCopy = genres; //necessary to manipulate the output values inside LINQ expression
            var actorsCopy = actors; //necessary to manipulate the output values inside LINQ expression
            var movieActorsCopy = movieActors; //necessary to manipulate the output values inside LINQ expression

            titles.ForEach(title =>
            {
                ExtractGenres(title, genresCopy);
                ExtractActors(title, actorsCopy);

            });

            return titles.Select(title =>
            {
                var movieId = ParseImDbId(title.Id);
                ExtractMovieActors(title, movieId, movieActorsCopy, actorsCopy);
                return new Movie()
                {
                    Id = movieId,
                    Description = title?.Plot,
                    Duration = long.Parse(title?.RuntimeMins ?? ""),
                    Image = MapImageToByteArray(title?.Image),
                    Title = title?.Title,
                    Trailer = title?.Trailer?.Link,
                    Rating = (int)(double.Parse(title?.IMDbRating ?? "") % MaxRating) + 1,
                    GenreId = genresCopy?.FirstOrDefault(g => g.Name == title?.GenreList?.FirstOrDefault()?.Value)?.Id ?? 0L
                };
            }).ToList();
        }

        private static IEnumerable<TitleData> FetchMovies()
        {
            var titleResponse = GetTop250Movies();
            var dataTop250Data = JsonMapper.Map<Top250Data>(titleResponse.Content);
            var titles = dataTop250Data.Items.Select(i => i.Id).ToList();
            return titles.Select(t => JsonMapper.Map<TitleData>(GetMovie(t).Content));
        }

        private static void ExtractMovieActors(TitleData title, long movieId, ISet<MovieActor> movieActorsCopy, IDictionary<string, Actor> actors)
        {
            var keyActor = new Actor();
            var newMovieActors = title.ActorList.Select(a =>
            {
                DetermineNames(a, out var firstName, out var lastName);
                keyActor.LastName = lastName;
                keyActor.FirstName = firstName;

                actors.TryGetValue(keyActor.Name, out var actor);
                return new MovieActor
                {
                    ActorId = actor.Id,
                    MovieId = movieId,
                };
            }).ToList();
            newMovieActors?.ForEach(movieActor =>
            {
                movieActor.Id = movieActorsCopy.Count + 1;
                movieActorsCopy.Add(movieActor);
                
            });


        }

        private static long ParseImDbId(string titleId)
        {
            return long.Parse(titleId.Substring(2));
        }

        private static void ExtractActors(TitleData title, IDictionary<string, Actor> actorCopy)
        {
            var newActors = title?.ActorList.Select(actor =>
            {
                DetermineNames(actor, out var firstName, out var lastName);
                return new Actor()
                {
                    FirstName = firstName,
                    LastName = lastName
                };
            }).ToList();
            newActors?.ForEach(actor =>
            {
                if (!actorCopy.ContainsKey(actor.Name))
                {
                    actor.Id = actorCopy.Count + 1;
                    actorCopy.Add(actor.Name, actor);
                }
            });
        }

        private static void DetermineNames(ActorShort actor, out string firstName, out string lastName)
        {
            var names = actor.Name.Split(" "); 
            firstName = "";
            lastName = "";
            switch (names.Length)
            {
                case 1:
                    firstName = names[0];
                    break;
                case 2:
                    firstName = names[0];
                    lastName = names[1];
                    break;
                case 3:
                    firstName = $"{names[0]} {names[1]}";
                    lastName = names[2];
                    break;
                case 4:
                    firstName = $"{names[0]} {names[1]}";
                    lastName = $"{names[2]} {names[3]}";
                    break;
                default:
                    firstName = string.Join(" ", names);
                    break;
            }
        }

        private static void ExtractGenres(TitleData title, ISet<Genre> genreCopy)
        {
            var newGenre = title?.GenreList.Select(gl => new Genre()
            {
                Name = gl?.Value
            }).ToList();
            newGenre?.ForEach(g =>
            {
                if (genreCopy.All(genre => genre.Name != g.Name))
                {
                    g.Id = genreCopy.Count + 1;
                    genreCopy.Add(g);
                }
            });
        }

        private static byte[] MapImageToByteArray(string url)
        {
            return Download(url);
        }

        private static byte[] Download(string url)
        {
            var restClient = new RestClient(url);
            var request = new RestRequest("#", Method.GET);
            return restClient.DownloadData(request);
        }


        private static IRestResponse Get(string url)
        {
            var restClient = new RestClient(url);
            var request = new RestRequest(Method.GET);
            return restClient.Execute(request);
        }

        private static IRestResponse GetTop250Movies()
        {
            return Get($"{Top250}{ApiKey}");
        }

        private static IRestResponse GetMovie(string title)
        {

            return Get($"{MovieURL}{ApiKey}/{title}{OptionPostfix}");
        }


    }
}
