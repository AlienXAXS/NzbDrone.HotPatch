using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Radarr.Host;

namespace NzbDrone.HotPatch
{
    public class ConsoleAlerts : IUserAlert
    {
        public void Alert(string message)
        {
            System.Console.WriteLine();
            System.Console.WriteLine(message);
            System.Console.WriteLine("Press enter to continue");
            System.Console.ReadLine();
        }
    }
}
