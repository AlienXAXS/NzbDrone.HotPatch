using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Harmony;

namespace NzbDrone.HotPatch.Harmony
{
    class PatchWrapper
    {
        private HarmonyInstance _harmonyInstance = HarmonyInstance.Create("com.agngaming.nzbdrone.hotpatch");

        private readonly List<PatchMakeup> _patches = new List<PatchMakeup>();
        private class PatchMakeup
        {
            public readonly MethodInfo originalMethod;
            public readonly HarmonyMethod patchedMethod;
            public bool isPostFix = false;

            public PatchMakeup(MethodInfo arg1, HarmonyMethod arg2, bool isPostFix = false)
            {
                originalMethod = arg1;
                patchedMethod = arg2;
                this.isPostFix = isPostFix;
            }
        }

        public void NewPrefixPatch(MethodInfo originalMethod, MethodInfo patchedMethod)
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

        public void NewPostfixPatch(MethodInfo originalMethod, MethodInfo patchedMethod)
        {
            if (originalMethod != null)
            {
                if (patchedMethod != null)
                {
                    _patches.Add(new PatchMakeup(originalMethod, new HarmonyMethod(patchedMethod), true));
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
            try
            {
                foreach (var patch in _patches)
                {
                    Utility.WriteToConsole($"Attempting to patch {patch.originalMethod.Name} in {patch.originalMethod.DeclaringType.FullName}", ConsoleColor.Blue);
                    if (patch.isPostFix)
                        _harmonyInstance.Patch(patch.originalMethod, postfix: patch.patchedMethod);
                    else
                        _harmonyInstance.Patch(patch.originalMethod, patch.patchedMethod, null);
                    Utility.WriteToConsole($"Patching {patch.originalMethod.Name} successful.", ConsoleColor.Blue);
                }

                return true;
            }
            catch (Exception ex)
            {
                Utility.WriteToConsole($"ERROR: {ex.Message}\r\n\r\n{ex.InnerException?.Message}", ConsoleColor.Red);
                return false;
            }
        }
    }
}
