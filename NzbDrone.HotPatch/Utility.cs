using System;

namespace NzbDrone.HotPatch
{
    public static class Utility
    {
        public static void WriteToConsole(string message, ConsoleColor color)
        {
            var oldColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine($"[{DateTime.Now}] HOTPATCH: {message}");
            Console.ForegroundColor = oldColor;
        }
    }
}
