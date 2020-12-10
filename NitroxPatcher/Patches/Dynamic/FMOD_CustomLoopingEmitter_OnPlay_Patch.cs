﻿using System.Reflection;
using Harmony;
using NitroxClient.GameLogic.FMOD;
using NitroxClient.MonoBehaviours;
using NitroxModel.Core;

namespace NitroxPatcher.Patches.Dynamic
{
    public class FMOD_CustomLoopingEmitter_OnPlay_Patch : NitroxPatch, IDynamicPatch
    {
        private static FMODSystem fmodSystem;

        private static readonly MethodInfo targetMethod = typeof(FMOD_CustomLoopingEmitter).GetMethod("OnPlay", BindingFlags.NonPublic | BindingFlags.Instance);

        public static bool Prefix()
        {
            return !FMODSuppressor.SuppressFMODEvents;
        }

        public static void Postfix(FMOD_CustomLoopingEmitter __instance)
        {
            if (__instance.assetStart && fmodSystem.IsWhitelisted(__instance.assetStart.path))
            {
                __instance.TryGetComponent(out NitroxEntity nitroxEntity);
                if (!nitroxEntity)
                {
                    nitroxEntity = __instance.GetComponentInParent<NitroxEntity>();
                }
                if (nitroxEntity)
                {
                    fmodSystem.PlayCustomLoopingEmitter(nitroxEntity.Id, __instance.assetStart.path);
                }
            }
        }

        public override void Patch(HarmonyInstance harmony)
        {
            fmodSystem = NitroxServiceLocator.LocateService<FMODSystem>();
            PatchMultiple(harmony, targetMethod, true, true, false);
        }
    }
}
