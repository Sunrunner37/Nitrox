﻿using System;
using System.Reflection;
using Harmony;
using NitroxClient.GameLogic;
using NitroxModel.Core;
using NitroxModel.DataStructures.GameLogic;
using NitroxModel.Logger;

namespace NitroxPatcher.Patches
{
    public class PDAEncyclopedia_Add_Patch : NitroxPatch
    {
        public static readonly Type TARGET_CLASS = typeof(PDAEncyclopedia);
        public static readonly MethodInfo TARGET_METHOD = TARGET_CLASS.GetMethod("Add", BindingFlags.NonPublic | BindingFlags.Static);

        public static void Prefix(string key, PDAEncyclopedia.Entry entry)
        {
            NitroxServiceLocator.LocateService<PDAEncyclopediaEntry>().Add(new EncyclopediaEntry((entry.timeCapsuleId != null ? key : entry.timeCapsuleId), entry.timeCapsuleId != null));
        }

        public override void Patch(HarmonyInstance harmony)
        {
            PatchPrefix(harmony, TARGET_METHOD);
        }
    }
}
