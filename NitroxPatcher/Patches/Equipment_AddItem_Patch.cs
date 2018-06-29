﻿using System;
using System.Reflection;
using Harmony;
using NitroxClient.GameLogic;
using NitroxModel.Core;
using NitroxModel.Logger;

namespace NitroxPatcher.Patches
{
    public class Equipment_AddItem_Patch : NitroxPatch
    {
        public static readonly Type TARGET_CLASS = typeof(Equipment);
        public static readonly MethodInfo TARGET_METHOD = TARGET_CLASS.GetMethod("AddItem", BindingFlags.Public | BindingFlags.Instance);

        public static void Postfix(Equipment __instance, bool __result, string slot, InventoryItem newItem)
        {
            Log.Info("Equipment_AddItem_Patch #########################################################################################################");
            if (__result)
            {

                NitroxServiceLocator.LocateService<EquipmentSlots>().BroadcastEquip(newItem.item, __instance.owner, slot);
            }
        }

        public override void Patch(HarmonyInstance harmony)
        {
            PatchPostfix(harmony, TARGET_METHOD);
        }
    }
}
