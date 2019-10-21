using System;
using NzbDrone.Core.Movies;

namespace NzbDrone.HotPatch.Harmony.NZBDrone.Versions._0_2_0_1358.Core.Organizer
{
    class FileNameBuilder
    {
        public static bool GetMovieFolder(ref Movie ___movie, ref string __result)
        {
            Utility.WriteToConsole($"Patching path for movie {___movie.Title}", ConsoleColor.Green);
            __result = ".";
            return false;
        }

        public static bool BuildMoviePath(ref Movie ___movie, ref string __result)
        {
            Utility.WriteToConsole($"Patching movie path for movie {___movie.Title} to not have a sub-folder.", ConsoleColor.Green);
            __result = ___movie.Path;
            return false;
        }
    }
}
