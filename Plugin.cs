using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System.Collections.Generic;
using System;
using System.Linq;
using BepInEx.Configuration;

namespace Patty_PyreHeartEffectEnabler_MOD
{
    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
    class Plugin : BaseUnityPlugin
    {
        internal static ManualLogSource LogSource { get; private set; }
        internal static Harmony PluginHarmony { get; private set; }
        internal static List<ConfigEntry<bool>> ConfigEntries { get; private set; } = new List<ConfigEntry<bool>>();
        internal static List<RelicState> PyreHeartArtifacts = new List<RelicState>();
        internal static Dictionary<string, string> GUIDToName { get; private set; } = new Dictionary<string, string>()
        {
            { "17c0693e-5b08-4252-b300-b4b377b9540d", "Proto Heartcage" },
            { "17ed1b69-ffb7-436c-b87c-98c07aba0242", "Heart of the Pact" },
            { "39dd8cf1-d611-4804-8ab9-a0f992afe691", "Lifemothers Pyre" },
            { "68a9b977-3407-4128-bf35-245fd92f8e2b", "Malickas Shifting Pyre" },
            { "7f5fff27-455e-4fc7-825d-df92f07e084d", "Wynghs Spirit" },
            { "cc792aac-4b82-40af-bf80-3a83bc511383", "Fhyras Greed" },
            { "36075977-411e-43e4-a075-0c4e3b81349b", "Aquaths Reservation" },
            { "7a60e645-e4f0-403c-9b4a-e794675f3a60", "Bogwurms Growth" },
            { "c36f7f21-9293-4c77-8f89-a7919180186e", "Echo of the Time Father" },
            { "db66de87-1fbe-4f29-9e62-3926d639b413", "Herzals Horde" },
            { "cc6a381e-0723-490a-af72-78cc82978094", "Pyre of Savagery" },
            { "c0c09adc-b23e-422e-bb75-689ee82cfa36", "Pyre of Dominion" },
            { "87d50a3b-dade-455f-9012-04f46bac02a5", "Pyre of Entropy" },
            { "7d9d33b9-2a9e-4e98-a205-600023d5386f", "Unassigned 1" },
            { "197f8e8d-cefe-4fa2-af94-d84f9631509c", "Unassigned 2" },
            { "96cca2fd-d096-4340-9054-d498826a0781", "Unassigned 3" },
        };

        public void Awake()
        {
            LogSource = Logger;
            try
            {
                PluginHarmony = Harmony.CreateAndPatchAll(typeof(PatchList), PluginInfo.GUID);
            }
            catch (Exception ex)
            {
                LogSource.LogError((ex.InnerException ?? ex).Message);
            }

            foreach (ConfigEntry<bool> entry in CreateAllEntry())
            {
                ConfigEntries.Add(entry);
            }
        }

        public static List<RelicState> GetPyreHeartArtifacts()
        {
            return PyreHeartArtifacts;
        }

        IEnumerable<ConfigEntry<bool>> CreateAllEntry()
        {
            yield return Config.Bind<bool>(new ConfigDefinition("Pyre Heart Effects", "Proto Heartcage"), true,
                                           new ConfigDescription("No special capabilities."));

            yield return Config.Bind<bool>(new ConfigDefinition("Pyre Heart Effects", "Heart of the Pact"), true,
                                           new ConfigDescription("After playing a card from your Primary and Allied clan, draw +1 next turn."));

            yield return Config.Bind<bool>(new ConfigDefinition("Pyre Heart Effects", "Lifemothers Pyre"), true,
                                           new ConfigDescription("Adds the option to copy cards at the Merchants of Steel, Magic, and Trinkets."));

            yield return Config.Bind<bool>(new ConfigDefinition("Pyre Heart Effects", "Malickas Shifting Pyre"), true,
                                           new ConfigDescription("All drafted cards appear with an upgrade."));

            yield return Config.Bind<bool>(new ConfigDefinition("Pyre Heart Effects", "Wynghs Spirit"), true,
                                           new ConfigDescription("Once per battle, select the Heart to restore the front friendly units on all floors to full health."));

            yield return Config.Bind<bool>(new ConfigDefinition("Pyre Heart Effects", "Fhyras Greed"), true,
                                           new ConfigDescription("When your Pyre takes damage, gain 10 Gold."));

            yield return Config.Bind<bool>(new ConfigDefinition("Pyre Heart Effects", "Aquaths Reservation"), true,
                                           new ConfigDescription("Once per battle, select the Heart to gain 3 Ember."));

            yield return Config.Bind<bool>(new ConfigDefinition("Pyre Heart Effects", "Bogwurm Growths"), true,
                                           new ConfigDescription("Permanently add 1 Capacity to a random floor after each battle."));

            yield return Config.Bind<bool>(new ConfigDefinition("Pyre Heart Effects", "Echo of the Time Father"), true,
                                           new ConfigDescription("Once per battle, select the Heart to apply Frozen to all non-Blight and non-Scourge cards in hand."));

            yield return Config.Bind<bool>(new ConfigDefinition("Pyre Heart Effects", "Herzals Horde"), true,
                                           new ConfigDescription("Gain  4 Deployment Ember. Deployable upgrade is 50% cheaper."));

            yield return Config.Bind<bool>(new ConfigDefinition("Pyre Heart Effects", "Pyre of Savagery"), true,
                                           new ConfigDescription("Friendly units gain Slay: +1 Attack permanently."));

            yield return Config.Bind<bool>(new ConfigDefinition("Pyre Heart Effects", "Pyre of Dominion"), true,
                                           new ConfigDescription("Remove Train Stewards and starting cards. Immediately draft four packs, and double each card picked."));

            yield return Config.Bind<bool>(new ConfigDefinition("Pyre Heart Effects", "Pyre of Entropy"), true,
                                           new ConfigDescription("Add a Vengeful Shard to your deck after each battle. Draw 1 extra card for every 2 Blights in your deck."));

            yield return Config.Bind<bool>(new ConfigDefinition("Pyre Heart Effects", "Unassigned 1"), true,
                                           new ConfigDescription("(WIP) Gain a duplicate of each drafted card."));

            yield return Config.Bind<bool>(new ConfigDefinition("Pyre Heart Effects", "Unassigned 2"), true,
                                           new ConfigDescription("Fhyra_ArtifactDataGainSpellPower."));

            yield return Config.Bind<bool>(new ConfigDefinition("Pyre Heart Effects", "Unassigned 3"), true,
                                           new ConfigDescription("-1 Capacity on all floors. Add 1 Capacity to a random floor after each battle."));
        }
    }
}
