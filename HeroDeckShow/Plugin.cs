using BepInEx;
using HarmonyLib;

namespace HeroDeckShow
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        private static bool discard = false;
        private void Awake()
        {
            // Plugin startup logic
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");

            Harmony.CreateAndPatchAll(typeof(Plugin));
        }

        [HarmonyPatch(typeof(DeckWindowUI), "SetCombatCard")]
        [HarmonyPrefix]
        public static void SetCombatDeck(DeckWindowUI __instance, int heroIndex, bool discard)
        {
            Plugin.discard = discard;
        }

        [HarmonyPatch(typeof(DeckWindowUI), "SetCombatCard")]
        [HarmonyPrefix]
        public static bool SetCombatCardPrefix(DeckWindowUI __instance, Hero hero,ref string cardId, int position, int total){
            // 正常显示弃牌堆
            if (discard) return true;
            
            var heroIndex = GetHeroIndex(hero);
            var heroDeck = MatchManager.Instance.GetHeroDeck(heroIndex);

            // 抽牌时查看牌堆顺序, 不是线程安全的
            // position 可能会超出牌堆数量
            if (position < heroDeck.Count)
            {
                cardId = heroDeck[position];
            }
            return true;
        }


        public static int GetHeroIndex(Hero target){
            var hero = AtOManager.Instance.GetHero(0);
            if (hero == target){
                return 0;
            }
            
            hero = AtOManager.Instance.GetHero(1);
            if (hero == target){
                return 1;
            }

            hero = AtOManager.Instance.GetHero(2);
            if (hero == target){
                return 2;
            }

            return 3;
        }
    }
}
