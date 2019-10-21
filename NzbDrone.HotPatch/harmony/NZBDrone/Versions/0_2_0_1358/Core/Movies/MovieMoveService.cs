using System;
using NzbDrone.Core.Movies.Commands;

namespace NzbDrone.HotPatch.Harmony.NZBDrone.Versions._0_2_0_1358.Core.Movies
{
    class MovieMoveService
    {
        public static void Execute(ref MoveMovieCommand message, ref NzbDrone.Core.Movies.IMovieService ____movieService)
        {
            var movie = ____movieService.GetMovie(message.MovieId);
            Utility.WriteToConsole($"Post Move Movie for {movie.Title}", ConsoleColor.Green);
            Utility.WriteToConsole($"MetaData:\r\n" +
                                   $"Source Path: {message.SourcePath}\r\n" +
                                   $"Dest Path: {message.DestinationPath}\r\n" +
                                   $"DestinationRootFolder: {message.DestinationRootFolder}", ConsoleColor.Green);
        }
    }
}
