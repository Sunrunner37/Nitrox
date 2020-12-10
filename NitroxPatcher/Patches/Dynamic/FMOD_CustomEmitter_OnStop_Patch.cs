﻿using System.Reflection;
using FMOD.Studio;
using Harmony;
using NitroxClient.GameLogic.FMOD;
using NitroxClient.MonoBehaviours;
using NitroxModel.Core;

namespace NitroxPatcher.Patches.Dynamic
{
    public class FMOD_CustomEmitter_OnStop_Patch : NitroxPatch, IDynamicPatch
    {
        private static FMODSystem fmodSystem;

        private static readonly MethodInfo targetMethod = typeof(FMOD_CustomEmitter).GetMethod("OnStop", BindingFlags.NonPublic | BindingFlags.Instance);

        public static void Postfix(FMOD_CustomEmitter __instance)
        {
            if (fmodSystem.IsWhitelisted(__instance.asset.path))
            {
                __instance.GetEventInstance().getDescription(out EventDescription description);
                description.is3D(out bool is3D);

                if (is3D)
                {
                    __instance.TryGetComponent(out NitroxEntity nitroxEntity);
                    if (!nitroxEntity)
                    {
                        nitroxEntity = __instance.GetComponentInParent<NitroxEntity>();
                    }
                    if (nitroxEntity)
                    {
                        fmodSystem.PlayCustomEmitter(nitroxEntity.Id, __instance.asset.path, false);
                    }
                }
            }
        }

        public override void Patch(HarmonyInstance harmony)
        {
            fmodSystem = NitroxServiceLocator.LocateService<FMODSystem>();
            PatchPostfix(harmony, targetMethod);
        }
    }
}
