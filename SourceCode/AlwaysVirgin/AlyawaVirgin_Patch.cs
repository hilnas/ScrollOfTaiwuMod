using System;
using TaiwuModdingLib.Core.Plugin;
using GameData.Utilities;
using HarmonyLib;
using GameData.Domains.Character;

namespace AlwaysVirgin
{
    [PluginConfig("AlwaysVirgin", "褪色人" ,"1.0")]
    public class AlyawaVirgin_Patch : TaiwuRemakePlugin
    {
        Harmony harmony;
        public override void Dispose()
        {
            if(harmony != null)
            {
                harmony.UnpatchSelf();
            }
        }

        public override void Initialize()
        {
            harmony = Harmony.CreateAndPatchAll(typeof(AlyawaVirgin_Patch));
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Character) , "HasVirginity")]
        public static void HasVirginity_Patch(ref Boolean __result)
        {
            __result = true;
        }
    }
}
