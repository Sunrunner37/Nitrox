﻿using System.Reflection;
using Harmony;
using UWE;

namespace NitroxPatcher.Patches.Dynamic
{
    public class FreezeTime_Begin_Patch : NitroxPatch, IDynamicPatch
    {
        public static readonly MethodInfo TARGET_METHOD = typeof(FreezeTime).GetMethod("Begin", BindingFlags.Public | BindingFlags.Static);

        public static bool Prefix(string userId, bool dontPauseSound)
        {
            if (userId.Equals("FeedbackPanel"))
            {
                return true;
            }
            return false;
        }

        public override void Patch(HarmonyInstance harmony)
        {
            PatchPrefix(harmony, TARGET_METHOD);
        }
    }
}
