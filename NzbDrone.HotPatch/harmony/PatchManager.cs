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
            switch (productVersion)
            {
                // 21/10/2019
                case "10.0.0.32922":
                    var patches = new NZBDrone.Versions._1_0_0_32922.Patches();
                    return patches.ApplyPatches();

                // Version is not supported
                default:
                    Utility.WriteToConsole($"Version {productVersion} is not supported by this version of HotPatch.", ConsoleColor.Red);
                    throw new Exception($"Version {productVersion} is not supported by this version of HotPatch.");

            }
        }
    }
}
