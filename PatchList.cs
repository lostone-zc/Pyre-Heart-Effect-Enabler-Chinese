using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using static ShinyShoe.ProfilingUtils;

namespace Patty_PyreHeartEffectEnabler_MOD
{
    class PatchList
    {
        [HarmonyTranspiler, HarmonyPatch(typeof(SaveManager), "SetupRun")]
        static IEnumerable<CodeInstruction> SetupRun(IEnumerable<CodeInstruction> instructions)
        {
            var modifiedInstructions = new List<CodeInstruction>(instructions);
            // Find the last line that calls AddRange
            int insertIdx = modifiedInstructions.FindLastIndex(codeInstruction =>
                                              codeInstruction.opcode == OpCodes.Callvirt &&
                                              codeInstruction.operand.ToString().Contains("AddRange"));

            // Increase the index by one so we are added after that line
            insertIdx += 1; 

            // Get the generic method to remove duplicates from ListExtensions type
            var removeDuplicatesMethod = typeof(ListExtensions)
                                                .GetMethods(AccessTools.allDeclared)
                                                .First(method => method.Name == "RemoveDuplicates" &&
                                                    method.IsGenericMethod &&
                                                    method.GetParameters().Length == 1)
                                                .MakeGenericMethod(typeof(RelicState));

            var newCodes = new List<CodeInstruction>
            {
                // Loads the list
                new CodeInstruction(OpCodes.Ldloc_2),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Plugin), nameof(Plugin.GetPyreHeartArtifacts))),
                // Call the method. The result would looks like list.AddRange(Plugin.GetPyreHeartArtifacts());
                new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(List<RelicState>), "AddRange")),
                new CodeInstruction(OpCodes.Ldloc_2),
                // Call the method. The result would looks like list.RemoveDuplicates();
                new CodeInstruction(OpCodes.Callvirt, removeDuplicatesMethod),
            };
            /*
             *	Final result would be like
             *	List<RelicState> list = new List<RelicState>();
             *	list.AddRange(this.GetCovenants());
             *	list.AddRange(this.GetMutators());
             *	list.AddRange(this.GetActiveSavePyreArtifactStates());
             *	list.AddRange(Plugin.GetPyreHeartArtifacts());
             *	list.RemoveDuplicates();
             */
            modifiedInstructions.InsertRange(insertIdx, newCodes);

            return modifiedInstructions.AsEnumerable();
        }

        [HarmonyPostfix, HarmonyPatch(typeof(SaveManager), nameof(SaveManager.GetAllRelics), new Type[] { typeof(List<RelicState>) })]
        static void GetAllRelics(List<RelicState> allRelics)
        {
            allRelics.AddRange(Plugin.GetPyreHeartArtifacts());
            allRelics.RemoveDuplicates();
        }

        [HarmonyPostfix, HarmonyPatch(typeof(SaveManager), nameof(SaveManager.SetActiveSavePyreCharacter))]
        static void SetActiveSavePyreCharacter(SaveManager __instance, ref MutableRules ___mutableRules)
        {
            Plugin.PyreHeartArtifacts.Clear();
            foreach (var artifact in __instance.GetAllGameData().GetAllPyreArtifactDatas())
            {
                string artifactName = Plugin.GUIDToName[artifact.GetID()];
                // Only create artifacts that has been enabled in the mod settings.
                if (Plugin.ConfigEntries.Exists(entry => entry.Definition.Key == artifactName && entry.Value == true))
                {
                    Plugin.PyreHeartArtifacts.Add(new PyreArtifactState(artifact, ___mutableRules));
                }
            }
        }
    }
}
