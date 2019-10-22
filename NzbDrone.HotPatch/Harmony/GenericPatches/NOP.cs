using System;

namespace NzbDrone.HotPatch.Harmony.GenericPatches
{
    class NOP
    {
        /// <summary>
        /// Used as a NOP, stops the original method from executing at all.
        /// </summary>
        /// <returns></returns>
        public static bool NoOperation(ref object __instance)
        {
            Utility.WriteToConsole($"Skipped execution of {__instance.GetType().FullName} via NOP", ConsoleColor.Red);
            return false;
        }

        /// <summary>
        /// Used as a NOP, stops the original method from executing at all, but also returns null
        /// </summary>
        /// <param name="_result"></param>
        /// <returns></returns>
        public static bool NoOperationWithNullResult(ref object _result)
        {
            _result = null;
            return false;
        }
    }
}
