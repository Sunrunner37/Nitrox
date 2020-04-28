﻿using System.Reflection;
using Harmony;
using NitroxClient.GameLogic;
using NitroxClient.MonoBehaviours;
using NitroxModel.Core;
using NitroxModel.DataStructures;
using NitroxModel.DataStructures.GameLogic.Entities.Metadata;
using NitroxModel.Logger;

namespace NitroxPatcher.Patches.Dynamic
{
    class PrecursorDoorway_ToggleDoor_Patch : NitroxPatch, IDynamicPatch
    {
        public static readonly MethodInfo TARGET_METHOD = typeof(PrecursorDoorway).GetMethod(nameof(PrecursorDoorway.ToggleDoor), BindingFlags.Public | BindingFlags.Instance);

        public static void Prefix(PrecursorDoorway __instance, bool open)
        {
            NitroxEntity entity = __instance.GetComponent<NitroxEntity>();
            NitroxServiceLocator.LocateService<PrecursorManager>().TogglePrecursorDoor(entity.Id, open);
        }

        public static void Postfix(PrecursorDoorway __instance)
        {
            NitroxId id = NitroxEntity.GetId(__instance.gameObject);
            PrecursorDoorwayMetadata precursorDoorwayMetadata = new PrecursorDoorwayMetadata(__instance.isOpen);

            Entities entities = NitroxServiceLocator.LocateService<Entities>();
            entities.BroadcastMetadataUpdate(id, precursorDoorwayMetadata);
            Log.Debug("Precursor toggle door metadata sent id " + id + " value " + __instance.isOpen);
        }

        public override void Patch(HarmonyInstance harmony)
        {
            PatchPostfix(harmony, TARGET_METHOD);
        }
    }
}