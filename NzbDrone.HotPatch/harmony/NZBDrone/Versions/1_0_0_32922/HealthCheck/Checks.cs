using NzbDrone.Core.HealthCheck;

namespace NzbDrone.HotPatch.Harmony.NZBDrone.Versions._1_0_0_32922.HealthCheck
{
    static class Checks
    {
        static bool Prefix(ref Core.HealthCheck.HealthCheck __result)
        {
            __result = new Core.HealthCheck.HealthCheck(typeof(NzbDrone.Core.HealthCheck.Checks.RootFolderCheck), HealthCheckResult.Ok, "");
            return false;
        }
    }
}
