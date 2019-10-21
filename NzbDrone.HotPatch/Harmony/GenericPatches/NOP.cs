using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NzbDrone.HotPatch.Harmony.GenericPatches
{
    class NOP
    {
        /// <summary>
        /// Used as a NOP, stops the original method from executing at all.
        /// </summary>
        /// <returns></returns>
        public static bool NoOperation()
        {
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
