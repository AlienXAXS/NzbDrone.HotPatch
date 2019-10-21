using System;
using System.Diagnostics;

namespace NzbDrone.HotPatch.Harmony
{
    /// <summary>
    /// Handles Harmony patches depending on the version of Radarr detected
    /// </summary>
    class PatchManager
    {
        private PatchWrapper _patchWrapper = new PatchWrapper();


        private string GetRadarrVersion()
        {
            // Get the current version of Radarr
            if (System.IO.File.Exists("Radarr.Host.dll"))
            {
                return FileVersionInfo.GetVersionInfo("Radarr.Host.dll").ProductVersion;
            }
            else
            {
                throw new Exception("Unable to find Radarr.Host.dll");
            }
        }

        public bool BeginPatching()
        {
            Utility.WriteToConsole("Detecting the version of Radarr", ConsoleColor.DarkMagenta);
            var productVersion = GetRadarrVersion();
            Utility.WriteToConsole($"Radarr Version {productVersion} detected!", ConsoleColor.DarkMagenta);

#if DEBUG
            var patchesx = new NZBDrone.Versions._0_2_0_1358.Patches();
            return patchesx.ApplyPatches(_patchWrapper);
#endif

            switch (productVersion)
            {
                // 21/10/2019
                case "0.2.0.1358":
                    var patches = new NZBDrone.Versions._0_2_0_1358.Patches();
                    return patches.ApplyPatches(_patchWrapper);

                // Version is not supported
                default:
                    Utility.WriteToConsole($"Version {productVersion} is not supported by this version of HotPatch.", ConsoleColor.Red);
                    throw new Exception($"Version {productVersion} is not supported by this version of HotPatch.");

            }
        }
    }
}
