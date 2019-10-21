using System;
using System.Net.Sockets;
using NLog;
using NzbDrone.Common.EnvironmentInfo;
using NzbDrone.Common.Instrumentation;
using NzbDrone.HotPatch.Harmony;
using Radarr.Host;

namespace NzbDrone.HotPatch
{
    public static class Program
    {
        private static readonly Logger Logger = NzbDroneLogger.GetLogger(typeof(Program));

        private enum ExitCodes : int
        {
            Normal = 0,
            UnknownFailure = 1,
            RecoverableFailure = 2
        }

        static void Main(string[] args)
        {
            try
            {

                var oldColor = Console.ForegroundColor;

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("AGN Gaming Radarr Hotpatch Loading...");
                Console.WriteLine("Attempting to bootstrap Radarr, here goes!");
                Console.ForegroundColor = oldColor;

                var patchManager = new PatchManager();
                patchManager.BeginPatching();

                var startupArgs = new StartupContext(args);
                try
                {
                    NzbDroneLogger.Register(startupArgs, false, true);
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine("NLog Exception: " + ex.ToString());
                    throw;
                }

                Bootstrap.Start(startupArgs, new ConsoleAlerts());
            }
            catch (SocketException e)
            {
                System.Console.WriteLine("");
                System.Console.WriteLine("");
                Logger.Fatal(e.Message + ". This can happen if another instance of Radarr is already running another application is using the same port (default: 7878) or the user has insufficient permissions");
                Exit(ExitCodes.RecoverableFailure);
            }
            catch (Exception e)
            {
                System.Console.WriteLine("");
                System.Console.WriteLine("");
                Logger.Fatal(e, "EPIC FAIL!");
                Exit(ExitCodes.UnknownFailure);
            }
        }

        private static void Exit(ExitCodes exitCode)
        {
            LogManager.Shutdown();

            if (exitCode != ExitCodes.Normal)
            {
                System.Console.WriteLine("Press enter to exit...");

                System.Threading.Thread.Sleep(1000);

                // Please note that ReadLine silently succeeds if there is no console, KeyAvailable does not.
                System.Console.ReadLine();
            }

            Environment.Exit((int)exitCode);
        }
    }
}
