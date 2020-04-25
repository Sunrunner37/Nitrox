﻿using System;
using System.Reflection;
using Harmony;
using NitroxClient.GameLogic;
using NitroxModel.Core;

namespace NitroxPatcher.Patches.Dynamic
{
    public class PDAScanner_NotifyProgress_Patch : NitroxPatch, IDynamicPatch
    {
        public static readonly MethodInfo TARGET_METHOD = typeof(PDAScanner).GetMethod("NotifyProgress", BindingFlags.NonPublic | BindingFlags.Static);

        public static void Prefix(PDAScanner.Entry entry)
        {
            if (entry != null)
            {
                NitroxServiceLocator.LocateService<PDAManagerEntry>().Progress(entry);
            }
        }

        public override void Patch(HarmonyInstance harmony)
        {
            PatchPrefix(harmony, TARGET_METHOD);
        }
    }
}
