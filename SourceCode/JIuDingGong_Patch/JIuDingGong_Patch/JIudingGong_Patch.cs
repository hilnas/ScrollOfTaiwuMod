using TaiwuModdingLib.Core.Plugin;
using HarmonyLib;
using GameData.Domains.SpecialEffect.CombatSkill.Kongsangpai.Neigong;
using System;
using GameData.Domains.SpecialEffect;


namespace JIuDingGong_Patch
{
    [PluginConfig("JIudingGong_Patch", "褪色人", "1.0")]
    public class JIudingGong_Patch : TaiwuRemakePlugin
    {
        Harmony harmony;
        public override void Dispose()
        {
            if (harmony != null)
            {
                harmony.UnpatchSelf();
            }
        }

        public override void Initialize()
        {
            harmony = Harmony.CreateAndPatchAll(typeof(JIudingGong_Patch));
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(JiuDingGong), "GetModifyValue") ]
        public static void GetModifyValue_Patch( JiuDingGong __instance , AffectedDataKey dataKey, ref int __result)
        {

            int addPower =  (int) Math.Max( (__instance.CharObj.GetHealth() / 12 * 30 / 100) , ((__instance.CharObj.GetMaxHealth() - __instance.CharObj.GetHealth()) / 12 * 30 / 100) );

            if(__result != 0)
            {
                __result = addPower;
            }
        }
       
    }

}

