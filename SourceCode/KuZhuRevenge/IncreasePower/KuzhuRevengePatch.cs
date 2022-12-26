using GameData.Domains.CombatSkill;
using GameData.Domains;
using HarmonyLib;
using GameData.Domains.Character;
using GameData.Domains;
using TaiwuModdingLib.Core.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using GameData.Utilities;
using Redzen.Random;
using GameData.Common;

namespace IncreasePower
{
    [PluginConfig("KuzhuRevenge", "褪色人", "1.0")]
    public class KuzhuRevengePatch : TaiwuRemakePlugin
    {
        Harmony harmony;
        public static bool printLog;
        public override void Dispose()
        {
            if( harmony != null)
            {
                harmony.UnpatchSelf();
            }
           
        }

        public override void Initialize()
        {
            harmony = Harmony.CreateAndPatchAll(typeof(KuzhuRevengePatch));

        }


        public override void OnModSettingUpdate()
        {
            DomainManager.Mod.GetSetting(ModIdStr, "printLog", ref printLog);
        }


        [HarmonyPostfix]
        [HarmonyPatch(typeof(CombatSkill) , "CalcPower")]
        public static void CalcPower_Patch(CombatSkill __instance , ref short __result)
        {
            int charId = Traverse.Create(__instance).Field("_id").GetValue<CombatSkillKey>().CharId;

            if (charId == DomainManager.Taiwu.GetTaiwuCharId())
            {
                int taiWuId = charId;
                short wifeIncreaseRatio = 0;
                short adoreIncreaseRatio = 0;
                //GameData.Domains.Character.Character TaiwuCharacter = DomainManager.Character.GetElement_Objects(taiWuId);

                // Taiwu Wife increase ratio calculation
                HashSet<int> TaiwuWifeCharacter = DomainManager.Character.GetRelatedCharIds(taiWuId, 1024);

                foreach (int taiwuWifeId in TaiwuWifeCharacter)
                {
                    // TaiWu wife have to be alive
                    if (DomainManager.Character.IsCharacterAlive(taiwuWifeId))
                    {
                        HashSet<int> huangMaoCharacter = DomainManager.Character.GetRelatedCharIds(taiwuWifeId, 16384);
                        // each haungmao
                        foreach (int huangMaoCharId in huangMaoCharacter)
                        {
                            //Huangmao character have to be alive and exclude taiwu
                            if ((DomainManager.Character.IsCharacterAlive(huangMaoCharId)) && (huangMaoCharId != taiWuId))
                            {
                                bool loveBack = DomainManager.Character.GetRelatedCharIds(huangMaoCharId, 16384).Contains(taiwuWifeId);
                                if (loveBack)
                                {
                                    //AdaptableLog.Info("太吾妻子的id： " + taiwuWifeId + "  妻子黄毛id： " + huangMaoCharId + " Mutual Love"); 
                                    wifeIncreaseRatio += 10;
                                }
                            }
                        }

                    }
                }

                //AdaptableLog.Info("太吾妻子提供的加成： " + wifeIncreaseRatio);

                // Taiwu Adored character ratio calculation
                HashSet<int> TaiwuAdoreCharacter = DomainManager.Character.GetRelatedCharIds(taiWuId, 16384);

                // For every TaiWu adored character
                foreach(int currTaiwuAdoreCharId in TaiwuAdoreCharacter)
                {
                    // TaiWu adore character have to be alive
                    if (DomainManager.Character.IsCharacterAlive(currTaiwuAdoreCharId))
                    {
                        HashSet<int> huangMaoCharacter = DomainManager.Character.GetRelatedCharIds(currTaiwuAdoreCharId, 16384);
                        // each haungmao
                        foreach (int huangMaoCharId in huangMaoCharacter)
                        {
                           //Huangmao character have to be alive and exclude taiwu
                           if ( (DomainManager.Character.IsCharacterAlive(huangMaoCharId)) &&  (huangMaoCharId != taiWuId))
                           {
                                bool loveBack = DomainManager.Character.GetRelatedCharIds(huangMaoCharId, 16384).Contains(currTaiwuAdoreCharId);
                                if (loveBack)
                                {
                                    //AdaptableLog.Info("太吾爱慕的id： " + currTaiwuAdoreCharId + "  爱慕黄毛id： " + huangMaoCharId + " Mutual Love");
                                    adoreIncreaseRatio += 5;
                                }
                           }
                        }

                    }      
                }
                //AdaptableLog.Info("太吾爱慕提供的加成： " + adoreIncreaseRatio);
                //AdaptableLog.Info("Total Increase: " + (adoreIncreaseRatio + wifeIncreaseRatio));

                __result += wifeIncreaseRatio;
                __result += adoreIncreaseRatio;
            }
        }


        [HarmonyPostfix]
        [HarmonyPatch(typeof(Character), "OfflineMakeLove")]
        public static void OfflineMakeLove_Patch(IRandomSource random, Character father, Character mother, bool isRape)
        {
            Character taiwu = DomainManager.Taiwu.GetTaiwu();
            int TaiwuId = taiwu.GetId();
            int fatherId = father.GetId();
            int motherId = mother.GetId();

            // Not Taiwu
            if (fatherId == TaiwuId || motherId == TaiwuId)
            {
                return;
            }

            HashSet<int> TaiwuWifeCharacter = DomainManager.Character.GetRelatedCharIds(TaiwuId, 1024);
            HashSet<int> TaiwuAdoreCharacter = DomainManager.Character.GetRelatedCharIds(TaiwuId, 16384);

            String sexType = isRape ? " 被欺侮/欺侮" : " 春宵"; 


            if(TaiwuWifeCharacter.Contains(fatherId) || TaiwuWifeCharacter.Contains(motherId))
            {
                DataContext currentThreadDataContext = DataContextManager.GetCurrentThreadDataContext();
                int currentNeiLi = taiwu.GetExtraNeili();

                if (printLog)
                {
                    if (TaiwuWifeCharacter.Contains(fatherId))
                    {
                        FileLog.Log(DateTime.Now.ToString() + "      太吾的配偶 " + father.GetSurname() + father.GetGivenName() + fatherId + " 与 " + mother.GetSurname() + mother.GetGivenName() + motherId + sexType + "， 永久增加太吾内力100点");
                    }
                    else if (TaiwuWifeCharacter.Contains(motherId))
                    {
                        FileLog.Log(DateTime.Now.ToString() + "      太吾的配偶 " + mother.GetSurname() + mother.GetGivenName() + motherId + " 与 " + father.GetSurname() + father.GetGivenName() + fatherId + sexType + "， 永久增加太吾内力100点");
                    }
                }

                taiwu.SetExtraNeili((currentNeiLi + 100), currentThreadDataContext);

                return;
            }

            if(TaiwuAdoreCharacter.Contains(fatherId) || TaiwuAdoreCharacter.Contains(motherId))
            {
                DataContext currentThreadDataContext = DataContextManager.GetCurrentThreadDataContext();
                int currentNeiLi = taiwu.GetExtraNeili();

                if (printLog)
                {
                    if (TaiwuAdoreCharacter.Contains(fatherId))
                    {
                        FileLog.Log(DateTime.Now.ToString() + "      太吾的男爱慕对象 " + father.GetSurname() + father.GetGivenName() + fatherId + " 与 " + mother.GetSurname() + mother.GetGivenName() + motherId + sexType + "， 永久增加太吾内力15点");
                    }
                    else if (TaiwuAdoreCharacter.Contains(motherId))
                    {
                        FileLog.Log(DateTime.Now.ToString() + "       太吾的女性爱慕对象 " + mother.GetSurname() + mother.GetGivenName() + motherId + " 与 " + father.GetSurname() + father.GetGivenName() + fatherId + sexType + "， 永久增加太吾内力15点");
                    }
                }


                taiwu.SetExtraNeili((currentNeiLi + 15), currentThreadDataContext);

            }


        }


    }
}
