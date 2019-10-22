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

            // Patches the movie scanner to no longer work
            // This is required as Radarr assumes that all movies within the movie folder is relevant to the movie being scanned for.
            // While this is a duct tape fix, fixing it in the long run is a headache and this is an easier approach.
            /*
            patchWrapper.NewPrefixPatch(
                AccessTools.Method(
                    typeof(NzbDrone.Core.Movies.RefreshMovieService),"Execute"),
                
                AccessTools.Method(
                    typeof(GenericPatches.NOP), "NoOperation")
                );
            */

            // Patches the GetUnmappedFolders method, so that a library scan will also scan all movie files that reside inside the root folder.
            // Usually this only scans sub directories, but seeing as we're patching that out, we need to patch this too.
            patchWrapper.NewPrefixPatch(
                AccessTools.Method(
                    typeof(NzbDrone.Core.RootFolders.RootFolderService), "GetUnmappedFolders"),
                AccessTools.Method(
                    typeof(NZBDrone.Versions._0_2_0_1358.Core.RootFolders.RootFolderService), "GetUnmappedFolders"));


            // Patches the bulk import movie search functions to ensure it always searches the TVDB, fixes root folder issues.
            patchWrapper.NewPrefixPatch(
                AccessTools.Method(
                    typeof(NzbDrone.Api.Movies.MovieBulkImportModule), "Search"),
                AccessTools.Method(
                    typeof(NZBDrone.Versions._0_2_0_1358.Api.Movies.MovieBulkImportModule), "Search"));

            return patchWrapper.ApplyPatches();
        }
    }
}