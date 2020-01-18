﻿using System;
using System.Reflection;
using Harmony;
using NitroxClient.MonoBehaviours;
using UnityEngine;
using NitroxClient.GameLogic.Helper;
using NitroxModel.Core;
using NitroxClient.GameLogic;
using NitroxModel.DataStructures;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

// TODO: Temporarily persistent to run before everything.  When we migrate the patch hook to an early point then make this non-persistent
namespace NitroxPatcher.Patches.Persistent
{
    class CellManager_TryLoadCacheBatchCells_Patch : NitroxPatch, IPersistentPatch
    {
        public static readonly Type TARGET_CLASS = typeof(CellManager);
        public static readonly MethodInfo TARGET_METHOD = TARGET_CLASS.GetMethod("TryLoadCacheBatchCells", BindingFlags.Public | BindingFlags.Instance);
        
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> instrList = instructions.ToList();
            for (int i = 0; i < instrList.Count; i++)
            {
                CodeInstruction instruction = instrList[i];
                if (instrList.Count > i + 2 && instrList[i+2].opcode == OpCodes.Callvirt && instrList[i+2].operand == (object)typeof(LargeWorldStreamer).GetProperty("pathPrefix", BindingFlags.Public | BindingFlags.Instance).GetGetMethod())
                {
                    yield return new CodeInstruction(OpCodes.Ldstr, "");
                    i += 2;
                }
                else if (instrList.Count > i + 2 && instrList[i + 2].opcode == OpCodes.Callvirt && instrList[i + 2].operand == (object)typeof(LargeWorldStreamer).GetProperty("fallbackPrefix", BindingFlags.Public | BindingFlags.Instance).GetGetMethod())
                {
                    yield return new CodeInstruction(OpCodes.Ldstr, "");
                    i += 2;
                }
                else
                {
                    yield return instruction;
                }
            }
        }

        public override void Patch(HarmonyInstance harmony)
        {
            PatchTranspiler(harmony, TARGET_METHOD);
        }
    }
}
