using NzbDrone.Core.HealthCheck;

namespace NzbDrone.HotPatch.Harmony.NZBDrone.Versions._0_2_0_1358.HealthCheck
{
    static class Checks
    {
        static bool Prefix(ref NzbDrone.Core.HealthCheck.HealthCheck __result)
        {
            __result = new NzbDrone.Core.HealthCheck.HealthCheck(typeof(NzbDrone.Core.HealthCheck.Checks.RootFolderCheck), HealthCheckResult.Ok, "");
            return false;
        }
    }
}
