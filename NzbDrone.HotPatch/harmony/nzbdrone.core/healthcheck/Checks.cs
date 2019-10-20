using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Harmony;
using NzbDrone.Core.HealthCheck;
using NzbDrone.Common.Disk;
using NzbDrone.Core.Movies;

namespace NzbDrone.HotPatch.harmony.nzbdrone.core.healthcheck
{
    [HarmonyPatch]
    static class Checks
    {

        struct resultTest
        {
            public bool OK;
        }

        static bool Check(ref HealthCheck __result)
        {
            __result = new HealthCheck(typeof(NzbDrone.Core.HealthCheck.Checks.RootFolderCheck), HealthCheckResult.Ok, "");
            Console.WriteLine("This is hotpatched!");
            return false;
        }
    }
}
