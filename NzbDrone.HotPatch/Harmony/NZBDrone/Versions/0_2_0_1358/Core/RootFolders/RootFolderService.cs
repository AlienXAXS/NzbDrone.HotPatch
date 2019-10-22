using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NLog;
using NzbDrone.Common;
using NzbDrone.Common.Disk;
using NzbDrone.Core.MediaFiles;
using NzbDrone.Core.Movies;
using NzbDrone.Core.RootFolders;

namespace NzbDrone.HotPatch.Harmony.NZBDrone.Versions._0_2_0_1358.Core.RootFolders
{
    class RootFolderService
    {

        public static bool GetUnmappedFolders(string path, ref List<UnmappedFolder> __result, ref IMovieRepository ____movieRepository, ref IDiskProvider ____diskProvider, ref Logger ____logger, ref HashSet<string> ___SpecialFolders)
        {
            // Setup our original names
            var _logger = ____logger;
            var _movieRepository = ____movieRepository;
            var _diskProvider = ____diskProvider;
            var SpecialFolders = ___SpecialFolders;

            _logger.Debug("Generating list of unmapped folders");
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("Invalid path provided", "path");
            }

            var results = new List<UnmappedFolder>();
            var movies = _movieRepository.All().ToList();

            if (!_diskProvider.FolderExists(path))
            {
                _logger.Debug("Path supplied does not exist: {0}", path);
                __result = results;
                return false;
            }

            var possibleMovieFolders = _diskProvider.GetDirectories(path).ToList();
            var unmappedFolders = possibleMovieFolders.Except(movies.Select(s => s.Path), PathEqualityComparer.Instance).ToList();

            foreach (string unmappedFolder in unmappedFolders)
            {
                var di = new DirectoryInfo(unmappedFolder.Normalize());
                if ((!di.Attributes.HasFlag(FileAttributes.System) && !di.Attributes.HasFlag(FileAttributes.Hidden)) || di.Attributes.ToString() == "-1")
                {
                    results.Add(new UnmappedFolder { Name = di.Name, Path = di.FullName });
                }

            }

            var setToRemove = SpecialFolders;
            results.RemoveAll(x => setToRemove.Contains(new DirectoryInfo(x.Path.ToLowerInvariant()).Name));

            // HOTPATCH:
            // Get all files that have an extension that radarr is expecting
            var possibleMovieFilesInRootFolder = _diskProvider.GetFiles(path, SearchOption.TopDirectoryOnly).ToList().Where(file => MediaFileExtensions.Extensions.Contains(Path.GetExtension(file)));

            // Scan all current movies, and remove files that Radarr already knows about
            foreach (var possibleMovieResult in possibleMovieFilesInRootFolder)
            {
                var foundMovie = movies.DefaultIfEmpty(null).FirstOrDefault(x =>
                    x.Path.Equals(path, StringComparison.OrdinalIgnoreCase) && x.HasFile &&
                    x.MovieFile.RelativePath.Equals(path, StringComparison.OrdinalIgnoreCase));

                if (foundMovie == null)
                {
                    results.Add(new UnmappedFolder() { Name = NzbDrone.Core.Parser.Parser.RemoveFileExtension(possibleMovieResult).Replace(path, ""), Path = path });
                }
            }

            _logger.Debug("{0} unmapped folders detected.", results.Count);
            __result = results;

            // NOP the original called method
            return false;
        }

    }
}
