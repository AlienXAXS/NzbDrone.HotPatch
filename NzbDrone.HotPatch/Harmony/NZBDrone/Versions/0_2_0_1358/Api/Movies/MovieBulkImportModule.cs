using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Nancy;
using NzbDrone.Api;
using NzbDrone.Api.Extensions;
using NzbDrone.Api.Movies;
using NzbDrone.Common.Cache;
using NzbDrone.Common.Extensions;
using NzbDrone.Core.Datastore;
using NzbDrone.Core.MediaCover;
using NzbDrone.Core.MediaFiles;
using NzbDrone.Core.MediaFiles.MovieImport;
using NzbDrone.Core.MetadataSource;
using NzbDrone.Core.Parser;
using NzbDrone.Core.Profiles;
using NzbDrone.Core.RootFolders;

namespace NzbDrone.HotPatch.Harmony.NZBDrone.Versions._0_2_0_1358.Api.Movies
{
    class MovieBulkImportModule : NancyModule
    {
        public static bool Search(ref IProfileService ____profileService,
            ref ICached<NzbDrone.Core.Movies.Movie> ____mappedMovies,
            ref MovieBulkImportModule __instance, ref IRootFolderService ____rootFolderService,
            ref IParsingService ____parsingService,
            ref IDiskScanService ____diskScanService, ref IMakeImportDecision ____importDecisionMaker,
            ref ISearchForNewMovie ____searchProxy, ref Response __result)
        {
            // Setup vars for original code below
            var Request = __instance.Request;
            var _profileService = ____profileService;
            var _rootFolderService = ____rootFolderService;
            var _parsingService = ____parsingService;
            var _mappedMovies = ____mappedMovies;
            var _diskScanService = ____diskScanService;
            var _importDecisionMaker = ____importDecisionMaker;
            var _searchProxy = ____searchProxy;


            Profile tempProfile = _profileService.All().First();
            RootFolder rootFolder = _rootFolderService.Get(Request.Query.Id);

            int page = Request.Query.page;
            int per_page = Request.Query.per_page;

            int min = (page - 1) * per_page;

            int max = page * per_page;

            var unmapped = rootFolder.UnmappedFolders.OrderBy(f => f.Name).ToList();

            int total_count = unmapped.Count;

            if (Request.Query.total_entries.HasValue)
            {
                total_count = Request.Query.total_entries;
            }

            max = total_count >= max ? max : total_count;

            var paged = unmapped.GetRange(min, max - min);

            var mapped = paged.Select(f =>
            {
                NzbDrone.Core.Movies.Movie m = null;

                var mappedMovie = _mappedMovies.Find(f.Name);

                if (mappedMovie != null)
                {
                    return mappedMovie;
                }

                var parsedTitle = _parsingService.ParseMinimalPathMovieInfo(f.Name);
                if (parsedTitle == null)
                {
                    m = new NzbDrone.Core.Movies.Movie
                    {
                        Title = f.Name.Replace(".", " ").Replace("-", " "),
                        Path = f.Path,
                        Profile = tempProfile
                    };
                }
                else
                {
                    parsedTitle.ImdbId = Parser.ParseImdbId(parsedTitle.SimpleReleaseTitle);

                    m = new NzbDrone.Core.Movies.Movie
                    {
                        Title = parsedTitle.MovieTitle,
                        Year = parsedTitle.Year,
                        ImdbId = parsedTitle.ImdbId,
                        Path = f.Path,
                        Profile = tempProfile
                    };
                }

                // Removed this check due to it not working with root directory
                //var files = _diskScanService.GetVideoFiles(f.Path);
                var files = new string[]{f.Path};
                var decisions = _importDecisionMaker.GetImportDecisions(files.ToList(), m, true);
                var decision = decisions.Where(d => d.Approved && !d.Rejections.Any()).FirstOrDefault();

                if (decision != null)
                {
                    var local = decision.LocalMovie;

                    m.MovieFile = new MovieFile
                    {
                        Path = local.Path,
                        Edition = local.ParsedMovieInfo.Edition,
                        Quality = local.Quality,
                        MediaInfo = local.MediaInfo,
                        ReleaseGroup = local.ParsedMovieInfo.ReleaseGroup,
                        RelativePath = f.Path.GetRelativePath(local.Path)
                    };
                }
                

                mappedMovie = _searchProxy.MapMovieToTmdbMovie(m);

                if (mappedMovie != null)
                {
                    mappedMovie.Monitored = true;

                    _mappedMovies.Set(f.Name, mappedMovie, TimeSpan.FromDays(2));

                    return mappedMovie;
                }

                return null;
            });

            var result = new PagingResource<MovieResource>
            {
                Page = page,
                PageSize = per_page,
                SortDirection = SortDirection.Ascending,
                SortKey = Request.Query.sort_by,
                TotalRecords = total_count - mapped.Where(m => m == null).Count(),
                Records = MapToResource(mapped.Where(m => m != null)).ToList()
            }.AsResponse();

            __result = result;
            return false;
        }

        private static IEnumerable<MovieResource> MapToResource(IEnumerable<NzbDrone.Core.Movies.Movie> movies)
        {
            foreach (var currentMovie in movies)
            {
                var resource = currentMovie.ToResource();
                var poster = currentMovie.Images.FirstOrDefault(c => c.CoverType == MediaCoverTypes.Poster);
                if (poster != null)
                {
                    resource.RemotePoster = poster.Url;
                }

                yield return resource;
            }
        }
    }
}
