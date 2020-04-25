﻿using System.Reflection;
using Harmony;
using UnityEngine;

namespace NitroxPatcher.Patches.Dynamic
{
    public class EnergyMixin_SpawnDefault_Patch : NitroxPatch, IDynamicPatch
    {
        public static readonly MethodInfo TARGET_METHOD = typeof(EnergyMixin).GetMethod("SpawnDefault", BindingFlags.Public | BindingFlags.Instance);

        public static bool Prefix(EnergyMixin __instance)
        {
            //Try to figure out if the default battery is spawned from a vehicle or cyclops
            GameObject gameObject = __instance.gameObject;
            if(gameObject.GetComponent<Vehicle>() != null)
            {
                return false;
            } else if(gameObject.GetComponentInParent<Vehicle>() != null)
            {
                return false;
            } else if (gameObject.GetComponentInParent<SubRoot>() != null)
            {
                return false;
            }

            return true;
        }

        public override void Patch(HarmonyInstance harmony)
        {
            PatchPrefix(harmony, TARGET_METHOD);
        }
    }
}
