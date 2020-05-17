﻿using System.Reflection;
using Harmony;
using NitroxClient.GameLogic;
using NitroxClient.MonoBehaviours;
using NitroxClient.Unity.Helper;
using NitroxModel.Core;
using NitroxModel.DataStructures;
using NitroxModel.DataStructures.Util;
using NitroxModel.Helper;
using NitroxModel.Logger;
using NitroxModel_Subnautica.DataStructures.GameLogic;
using UnityEngine;

namespace NitroxPatcher.Patches.Dynamic
{
    public class Rocket_Start_Patch : NitroxPatch, IDynamicPatch
    {
		public static readonly MethodInfo TARGET_METHOD = typeof(Rocket).GetMethod("SubConstructionComplete", BindingFlags.Public | BindingFlags.Instance);

        public static void Prefix(Rocket __instance)
        {
            GameObject gameObject = __instance.gameObject;
            NitroxId id = NitroxEntity.GetId(gameObject);
            Optional<NeptuneRocketModel> model = NitroxServiceLocator.LocateService<Vehicles>().TryGetVehicle<NeptuneRocketModel>(id);

            if (!model.HasValue)
            {
                Log.Error($"{nameof(Rocket_Start_Patch)}: Could not find {nameof(NeptuneRocketModel)} by Nitrox id {id}.\nGO containing wrong id: {__instance.GetHierarchyPath()}");
            }

            __instance.currentRocketStage = model.Value.CurrentRocketStage;
        }

        public override void Patch(HarmonyInstance harmony)
        {
            PatchPrefix(harmony, TARGET_METHOD);
        }
    }
}
