﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Harmony;
using System.Reflection;

namespace NitroxPatcher.Patches.Persistent
{
    public class PrefabPlaceholdersGroup_Spawn_Patch : NitroxPatch
    {
        public static bool Prefix()
        {
            return false; // Disable spawning of PrefabPlaceholders(In other words large portion of objects)
        }

        public override void Patch(HarmonyInstance harmony)
        {
            PatchPrefix(harmony, typeof(PrefabPlaceholdersGroup).GetMethod("Spawn", BindingFlags.Instance | BindingFlags.Public));
        }
    }
}