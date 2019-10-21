using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Harmony;
using Microsoft.Win32.SafeHandles;

namespace NzbDrone.HotPatch.Harmony
{
    class PatchWrapper
    {
        private HarmonyInstance _harmonyInstance = HarmonyInstance.Create("com.agngaming.nzbdrone.hotpatch");

        private readonly List<PatchMakeup> _patches = new List<PatchMakeup>();
        private class PatchMakeup
        {
            public MethodInfo originalMethod;
            public HarmonyMethod patchedMethod;

            public PatchMakeup(MethodInfo arg1, HarmonyMethod arg2)
            {
                originalMethod = arg1;
                patchedMethod = arg2;
            }
        }

        public void NewPostfixPatch(MethodInfo originalMethod, MethodInfo patchedMethod)
        {
            if (originalMethod != null)
            {
                if (patchedMethod != null)
                {
                    _patches.Add(new PatchMakeup(originalMethod, new HarmonyMethod(patchedMethod)));
                }
                else
                {
                    // Patched method is null
                }
            }
            else
            {
                // Original is null
            }
        }

        /// <summary>
        /// Applies all patches in the order they were sent in.
        /// </summary>
        /// <returns>Returns a bool value if all of the patches were successful or not</returns>
        public bool ApplyPatches()
        {
            //[HarmonyPatch(typeof(Core.HealthCheck.Checks.RootFolderCheck))]
            //[HarmonyPatch("Check")]

            try
            {
                foreach (var patch in _patches)
                {
                    Utility.WriteToConsole($"Attempting to patch {patch.originalMethod.Name}", ConsoleColor.Blue);
                    _harmonyInstance.Patch(patch.originalMethod, patch.patchedMethod, null);
                }

                return true;
            }
            catch (Exception ex)
            {
                Utility.WriteToConsole($"ERROR: {ex.Message}\r\n\r\n{ex.StackTrace}", ConsoleColor.Red);
                return false;
            }
        }
    }
}
