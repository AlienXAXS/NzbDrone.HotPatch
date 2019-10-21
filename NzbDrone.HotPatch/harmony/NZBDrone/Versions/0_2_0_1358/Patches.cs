using Harmony;
using NzbDrone.HotPatch.Harmony.NZBDrone.Versions._0_2_0_1358.Core.Movies;

namespace NzbDrone.HotPatch.Harmony.NZBDrone.Versions._0_2_0_1358
{
    class Patches
    {
        public bool ApplyPatches(PatchWrapper patchWrapper)
        {
            // Disables the auto updater
            patchWrapper.NewPrefixPatch(
                AccessTools.Method(
                    typeof(NzbDrone.Core.Update.CheckUpdateService),
                    "AvailableUpdate"),

                AccessTools.Method(
                    typeof(GenericPatches.NOP),
                    "NoOperation")
            );

            /*
            // Patches GetMovieFolder to always return a dot for current directory.
            patchWrapper.NewPrefixPatch(
                AccessTools.Method(
                    typeof(NzbDrone.Core.Organizer.FileNameBuilder),
                    "GetMovieFolder"),

                AccessTools.Method(
                    typeof(NZBDrone.Versions._1_0_0_32922.Core.Organizer.FileNameBuilder),
                    "GetMovieFolder")
            );
            */

            patchWrapper.NewPostfixPatch(
                AccessTools.Method(
                    typeof(NzbDrone.Core.Movies.MoveMovieService), "Execute"),
                AccessTools.Method(
                    typeof(MovieMoveService), "Execute"));


            patchWrapper.NewPrefixPatch(
                AccessTools.Method(
                    typeof(NzbDrone.Core.Movies.MovieService), 
                    "AddMovie"),
                
                AccessTools.Method(
                    typeof(NZBDrone.Versions._0_2_0_1358.Core.Movies.MovieService),
                    "AddMovie"));
            

            return patchWrapper.ApplyPatches();
        }
    }
}