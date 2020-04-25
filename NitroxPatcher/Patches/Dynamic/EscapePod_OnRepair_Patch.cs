﻿using System.Reflection;
using Harmony;
using NitroxClient.GameLogic;
using NitroxModel.Core;

namespace NitroxPatcher.Patches.Dynamic
{
    public class EscapePod_OnRepair_Patch : NitroxPatch, IDynamicPatch
    {
        public static readonly MethodInfo TARGET_METHOD = typeof(EscapePod).GetMethod("OnRepair", BindingFlags.Public | BindingFlags.Instance);

        public static bool Prefix(EscapePod __instance)
        {
            NitroxServiceLocator.LocateService<EscapePodManager>().OnRepairedByMe(__instance);
            return true;
        }

        public override void Patch(HarmonyInstance harmony)
        {
            PatchPrefix(harmony, TARGET_METHOD);
        }
    }
}
