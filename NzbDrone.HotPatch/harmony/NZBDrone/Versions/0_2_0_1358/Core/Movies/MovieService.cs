using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NLog;
using NzbDrone.Common.EnsureThat;
using NzbDrone.Common.Extensions;
using NzbDrone.Core.Configuration;
using NzbDrone.Core.Messaging.Events;
using NzbDrone.Core.Movies;
using NzbDrone.Core.Movies.Events;
using NzbDrone.Core.Organizer;
using NzbDrone.Core.Parser;

namespace NzbDrone.HotPatch.Harmony.NZBDrone.Versions._0_2_0_1358.Core.Movies
{
    class MovieService
    {
        // Add Movie:
        // Called whenever a movie is added from the GUI (and i assume via RSS feeds also)

        // Reason for patch:
        // Movie.Path must be root directory, not a sub folder.

        public static bool AddMovie(ref Movie newMovie, ref NzbDrone.Core.Movies.MovieService __instance, 
            ref Logger ____logger, ref IConfigService ____configService, ref IMovieRepository ____movieRepository,
            ref IBuildFileNames ____fileNameBuilder, ref IEventAggregator ____eventAggregator, ref Movie __result)
        {
            var movie = newMovie;
            Ensure.That(newMovie, () => movie).IsNotNull();

            Utility.WriteToConsole("Patched AddMovie called!", ConsoleColor.Green);
            Utility.WriteToConsole($"New movie {movie.Title} added, patching file path to not have sub folders...", ConsoleColor.Green);

            MoviePathState defaultState = MoviePathState.Static;
            if (!____configService.PathsDefaultStatic)
            {
                defaultState = MoviePathState.Dynamic;
            }
            if (string.IsNullOrWhiteSpace(movie.Path))
            {
                // Call the original GetMovieFolder as it does other stuff too.
                var folderName = ____fileNameBuilder.GetMovieFolder(movie);

                // But replace our movie path with the root folder path, nothing more.
                movie.Path = newMovie.RootFolderPath;
                movie.PathState = defaultState;
            }
            else
            {
                movie.PathState = defaultState == MoviePathState.Dynamic ? MoviePathState.StaticOnce : MoviePathState.Static;
            }

            ____logger.Info("Adding Movie {0} Path: [{1}]", newMovie, newMovie.Path);

            movie.CleanTitle = newMovie.Title.CleanSeriesTitle();
            movie.SortTitle = MovieTitleNormalizer.Normalize(movie.Title, movie.TmdbId);
            movie.Added = DateTime.UtcNow;

            ____movieRepository.Insert(newMovie);
            ____eventAggregator.PublishEvent(new MovieAddedEvent(__instance.GetMovie(movie.Id)));

            __result = movie;

            Utility.WriteToConsole($"Patching complete, new root directory of movie {movie.Title} is {movie.Path}", ConsoleColor.Green);
            Utility.WriteToConsole("Patching complete.", ConsoleColor.Green);
            return false;
        }
    }
}
