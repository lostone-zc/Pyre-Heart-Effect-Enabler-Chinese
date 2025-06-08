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
        internal const string PROTO_HEARTCAGE = "Proto Heartcage";
        internal const string HEART_OF_THE_PACT = "Heart of the Pact";
        internal const string LIFEMOTHERS_PYRE = "Lifemothers Pyre";
        internal const string MALICKAS_SHIFTING_PYRE = "Malickas Shifting Pyre";
        internal const string WYNGHS_SPIRIT = "Wynghs Spirit";
        internal const string FHYRAS_GREED = "Fhyras Greed";
        internal const string AQUATHS_RESERVATION = "Aquaths Reservation";
        internal const string BOGWURMS_GROWTH = "Bogwurms Growth";
        internal const string ECHO_OF_THE_TIME_FATHER = "Echo of the Time Father";
        internal const string HERZALS_HORDE = "Herzals Horde";
        internal const string PYRE_OF_SAVAGERY = "Pyre of Savagery";
        internal const string PYRE_OF_DOMINION = "Pyre of Dominion";
        internal const string PYRE_OF_ENTROPY = "Pyre of Entropy";
        internal const string UNASSIGNED_1 = "Unassigned 1";
        internal const string UNASSIGNED_2 = "Unassigned 2";
        internal const string UNASSIGNED_3 = "Unassigned 3";
        internal static ManualLogSource LogSource { get; private set; }
        internal static Harmony PluginHarmony { get; private set; }
        internal static List<ConfigEntry<bool>> ConfigEntries { get; private set; } = new List<ConfigEntry<bool>>();
        internal static List<RelicState> PyreHeartArtifacts = new List<RelicState>();
        internal static Dictionary<string, string> GUIDToName { get; private set; } = new Dictionary<string, string>()
        {
            { "17c0693e-5b08-4252-b300-b4b377b9540d", PROTO_HEARTCAGE },
            { "17ed1b69-ffb7-436c-b87c-98c07aba0242", HEART_OF_THE_PACT },
            { "39dd8cf1-d611-4804-8ab9-a0f992afe691", LIFEMOTHERS_PYRE },
            { "68a9b977-3407-4128-bf35-245fd92f8e2b", MALICKAS_SHIFTING_PYRE },
            { "7f5fff27-455e-4fc7-825d-df92f07e084d", WYNGHS_SPIRIT },
            { "cc792aac-4b82-40af-bf80-3a83bc511383", FHYRAS_GREED },
            { "36075977-411e-43e4-a075-0c4e3b81349b", AQUATHS_RESERVATION },
            { "7a60e645-e4f0-403c-9b4a-e794675f3a60", BOGWURMS_GROWTH },
            { "c36f7f21-9293-4c77-8f89-a7919180186e", ECHO_OF_THE_TIME_FATHER },
            { "db66de87-1fbe-4f29-9e62-3926d639b413", HERZALS_HORDE },
            { "cc6a381e-0723-490a-af72-78cc82978094", PYRE_OF_SAVAGERY },
            { "c0c09adc-b23e-422e-bb75-689ee82cfa36", PYRE_OF_DOMINION },
            { "87d50a3b-dade-455f-9012-04f46bac02a5", PYRE_OF_ENTROPY },
            { "7d9d33b9-2a9e-4e98-a205-600023d5386f", UNASSIGNED_1 },
            { "197f8e8d-cefe-4fa2-af94-d84f9631509c", UNASSIGNED_2 },
            { "96cca2fd-d096-4340-9054-d498826a0781", UNASSIGNED_3 },
        };

        public void Awake()
        {
            LogSource = Logger;
            try
            {
                PluginHarmony = Harmony.CreateAndPatchAll(typeof(PatchList), PluginInfo.GUID);
            }
            catch (HarmonyException ex)
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
            yield return Config.Bind<bool>(new ConfigDefinition("Pyre Heart Effects", PROTO_HEARTCAGE), true,
                                           new ConfigDescription("No special capabilities.\n无特殊能力"));

            yield return Config.Bind<bool>(new ConfigDefinition("Pyre Heart Effects", HEART_OF_THE_PACT), true,
                                           new ConfigDescription("After playing a card from your Primary and Allied clan, draw +1 next turn.\n打出一张你主氏族和盟友氏族的卡牌后，下个回合抽 +1 张牌。"));

            yield return Config.Bind<bool>(new ConfigDefinition("Pyre Heart Effects", LIFEMOTHERS_PYRE), true,
                                           new ConfigDescription("Adds the option to copy cards at the Merchants of Steel, Magic, and Trinkets.\n新增在钢铁、魔法和杂货商人处复制卡牌的选项。"));

            yield return Config.Bind<bool>(new ConfigDefinition("Pyre Heart Effects", MALICKAS_SHIFTING_PYRE), true,
                                           new ConfigDescription("All drafted cards appear with an upgrade.\n所有自选卡牌都带有升级。"));

            yield return Config.Bind<bool>(new ConfigDefinition("Pyre Heart Effects", WYNGHS_SPIRIT), true,
                                           new ConfigDescription("Once per battle, select the Heart to restore the front friendly units on all floors to full health.\n选择薪火之心可为所有层的前排友方单位恢复所有生命值，每场战斗一次。"));

            yield return Config.Bind<bool>(new ConfigDefinition("Pyre Heart Effects", FHYRAS_GREED), true,
                                           new ConfigDescription("When your Pyre takes damage, gain 10 Gold.\n当薪火收到伤害时，获得 10 金币。"));

            yield return Config.Bind<bool>(new ConfigDefinition("Pyre Heart Effects", AQUATHS_RESERVATION), true,
                                           new ConfigDescription("Once per battle, select the Heart to gain 3 Ember.\n选择薪火之心可获得 3 余烬，每场战斗一次。"));

            yield return Config.Bind<bool>(new ConfigDefinition("Pyre Heart Effects", BOGWURMS_GROWTH), true,
                                           new ConfigDescription("Permanently add 1 Capacity to a random floor after each battle.\n每场战斗后，为随机楼层永久增加 1 容量。"));

            yield return Config.Bind<bool>(new ConfigDefinition("Pyre Heart Effects", ECHO_OF_THE_TIME_FATHER), true,
                                           new ConfigDescription("Once per battle, select the Heart to apply Frozen to all non-Blight and non-Scourge cards in hand.\n选择薪火之心可使所有非祸患和非天灾手牌获得冻结，每场战斗一次。"));

            yield return Config.Bind<bool>(new ConfigDefinition("Pyre Heart Effects", HERZALS_HORDE), true,
                                           new ConfigDescription("Gain  4 Deployment Ember. Deployable upgrade is 50% cheaper.\n获得 4 部署阶段余烬。升级可部署的价格降低 50%。"));

            yield return Config.Bind<bool>(new ConfigDefinition("Pyre Heart Effects", PYRE_OF_SAVAGERY), true,
                                           new ConfigDescription("Friendly units gain Slay: +1 Attack permanently.\n友方单位获得“杀戮：永久 +1 攻击力。”"));

            yield return Config.Bind<bool>(new ConfigDefinition("Pyre Heart Effects", PYRE_OF_DOMINION), true,
                                           new ConfigDescription("Remove Train Stewards and starting cards. Immediately draft four packs, and double each card picked.\n移除乘务员和初始卡牌。立刻获取四个自选卡包，并双倍获取挑选的卡牌。"));

            yield return Config.Bind<bool>(new ConfigDefinition("Pyre Heart Effects", PYRE_OF_ENTROPY), true,
                                           new ConfigDescription("Add a Vengeful Shard to your deck after each battle. Draw 1 extra card for every 2 Blights in your deck.\n每场战斗后在你的卡组中添加 1 张复仇碎片。你的卡组中每有 2 张祸患，额外抽 1 张牌。"));

            yield return Config.Bind<bool>(new ConfigDefinition("Pyre Heart Effects", UNASSIGNED_1), true,
                                           new ConfigDescription("(WIP) Gain a duplicate of each drafted card.\n（制作中）每次自选卡牌可获得一张复制品。"));

            yield return Config.Bind<bool>(new ConfigDefinition("Pyre Heart Effects", UNASSIGNED_2), true,
                                           new ConfigDescription("Fhyra_ArtifactDataGainSpellPower."));

            yield return Config.Bind<bool>(new ConfigDefinition("Pyre Heart Effects", UNASSIGNED_3), true,
                                           new ConfigDescription("-1 Capacity on all floors. Add 1 Capacity to a random floor after each battle.\n所有层 -1 容量。每场战斗后，随机层增加 1 容量。"));
        }
    }
}
