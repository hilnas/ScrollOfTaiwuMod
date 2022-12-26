using TaiwuModdingLib.Core.Plugin;
using HarmonyLib;
using GameData.Domains.Character;
using GameData.Domains;
using GameData.Domains.Character.Relation;
using GameData.Domains.Character.Ai;
using System.Collections.Generic;
using System;
using GameData.Utilities;
using GameData.Domains.Character.ParallelModifications;
using Redzen.Random;
using System.Reflection;
using GameData.Common;
using GameData.GameDataBridge;

// Adore : 16384
// husbang or wife : 1024

// Harmony patch refleciton to access private variable
// typeof(要反射的类).GetField("变量名称",AccessTools.all).GetValue(要反射的类的实例);
// 要反射的类的实例.GetType().GetField("变量名称", AccessTools.all).GetValue(要反射的类的实例); 

// var foo = SomeClass.GetType().GetMethod("MethodName", BindingFlags.NonPublic | BindingFlags.Instance(or Static));
// foo.Invoke(something something);


// Second Step:
// 		public void PeriAdvanceMonth_RelationsUpdate(DataContext context, HashSet<int> charSet) postPatch here?
// Second step
// GetStartOrEndRelationTarget second parameter:  2 start adore , 3 start boy girl friend  patch here?
// called this for judgement

// Gender : 0 for female , 1 for male

namespace NtrYes
{
    [PluginConfig("ntryes", "褪色人", "1.0")]
    public class NtrYes_Patch : TaiwuRemakePlugin
    {
        // Taiwu's femlae character been raped
        public static int rape_rate = 8;
        public static int sex_rate = 20;
        public static int add_adoreReady_List = 30;
        public static int start_adore_apply = 30;
        public static int final_rate = 45;
        public static bool printLog = false;
        public static int pregnant_chance = 0;

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
            harmony = Harmony.CreateAndPatchAll(typeof(NtrYes_Patch));
        }

        public override void OnModSettingUpdate()
        {
            DomainManager.Mod.GetSetting(ModIdStr, "add_adoreReady_List" , ref add_adoreReady_List);
            DomainManager.Mod.GetSetting(ModIdStr, "start_adore_apply", ref start_adore_apply);
            DomainManager.Mod.GetSetting(ModIdStr, "final_rate", ref final_rate);
            DomainManager.Mod.GetSetting(ModIdStr, "sex_rate", ref sex_rate);
            DomainManager.Mod.GetSetting(ModIdStr, "rape_rate", ref rape_rate);
            DomainManager.Mod.GetSetting(ModIdStr, "pregnant_chance", ref pregnant_chance);
            DomainManager.Mod.GetSetting(ModIdStr, "printLog", ref printLog);

            if (printLog)
            {
                FileLog.Log("NTR MOD START ： add_adoreReady_List ：" + add_adoreReady_List
                    + " start_adore_apply: " + start_adore_apply + " final_rate: " + final_rate
                    + " sex_rate: " + sex_rate + " rape_rate: " + rape_rate + " printLog: " + printLog + " pregant_change: " + pregnant_chance);
            }
        }



        // Used for add to adore ready list , check for taiwu wife and adored charas , no gender judgement
        public static bool isCharTaiWuWifeOrAdore(int charId)
        {
            int taiWuId = DomainManager.Taiwu.GetTaiwuCharId();
            bool isSelfCharTaiWuLover = false;
         
            RelatedCharacter TaiwuToSelf;
            if (DomainManager.Character.TryGetRelation(taiWuId ,charId, out TaiwuToSelf))
            {
                if (RelationType.HasRelation(TaiwuToSelf.RelationType, 16384) || RelationType.HasRelation(TaiwuToSelf.RelationType, 1024))
                {
                    isSelfCharTaiWuLover = true;
                }
            }

            return isSelfCharTaiWuLover;
            
        }

        public static bool isCharTaiwuSpouse(int charId)
        {
            int taiWuId = DomainManager.Taiwu.GetTaiwuCharId();

            RelatedCharacter TaiwuToSelf;
            if (DomainManager.Character.TryGetRelation(taiWuId, charId, out TaiwuToSelf))
            {
                if (RelationType.HasRelation(TaiwuToSelf.RelationType, 1024))
                {
                    return true;
                }
            }

            return false;
        }


        // Check whether self character or target character is taiwu wife or lover (Female Only)
        public static bool isCharTaiWuLover(Character selfChar, Character targetChar)
        {
            int selfId = selfChar.GetId();
            int targerId = targetChar.GetId();

            int taiWuId = DomainManager.Taiwu.GetTaiwuCharId();

            bool isSelfCharTaiWuLover = false;
            bool isSTargetCharTaiWuLover = false;

            if (selfChar.GetGender() == 0)
            {
                RelatedCharacter TaiwuToSelf;
                if (DomainManager.Character.TryGetRelation(taiWuId ,selfId, out TaiwuToSelf))
                {
                    if (RelationType.HasRelation(TaiwuToSelf.RelationType, 16384) || RelationType.HasRelation(TaiwuToSelf.RelationType, 1024))
                    {
                        isSelfCharTaiWuLover = true;
                    }
                }
            }

            if (targetChar.GetGender() == 0)
            {
                RelatedCharacter TaiwuToTarget;
                if (DomainManager.Character.TryGetRelation(taiWuId,targerId,out TaiwuToTarget))
                {
                    if (RelationType.HasRelation(TaiwuToTarget.RelationType, 16384) || RelationType.HasRelation(TaiwuToTarget.RelationType, 1024))
                    {
                        isSTargetCharTaiWuLover = true;
                    }
                }
            }

            if (isSelfCharTaiWuLover || isSTargetCharTaiWuLover)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // Used for marriage , Taiwu lover and other character , Only check for whether is Taiwu adored chara
        public static bool isCharTaiWuLover(int selfId, int targerId)
        {
    
            int taiWuId = DomainManager.Taiwu.GetTaiwuCharId();

            bool isSelfCharTaiWuLover = false;
            bool isSTargetCharTaiWuLover = false;

            RelatedCharacter TaiwuToSelf;
            if (DomainManager.Character.TryGetRelation(taiWuId,selfId, out TaiwuToSelf))
            {
                if (RelationType.HasRelation(TaiwuToSelf.RelationType, 16384))
                {
                    isSelfCharTaiWuLover = true;
                }
            }

            RelatedCharacter TaiwuToTarget;
            if (DomainManager.Character.TryGetRelation(taiWuId ,targerId, out TaiwuToTarget))
            {
                if (RelationType.HasRelation(TaiwuToTarget.RelationType, 16384))
                {
                    isSTargetCharTaiWuLover = true;
                }
            }

            if (isSelfCharTaiWuLover || isSTargetCharTaiWuLover)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

  
        // Will love back if any character is taiwu related female
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Character), "ApplyAddRelation_Adore")]
        public static bool ApplyAddRelation_Adore_Patch(Character selfChar, Character targetChar, ref bool targetLovesBack)
        {

            if (NtrYes_Patch.isCharTaiWuLover(selfChar, targetChar))
            {
                targetLovesBack = true;
            }

            return true;
        }

        // Will love back if any character it taiwu related female
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Character), "ApplyBecomeBoyOrGirlFriend")]
        public static bool ApplyBecomeBoyOrGirlFriend_Patch(Character selfChar, Character targetChar, ref bool succeed)
        {

            if (NtrYes_Patch.isCharTaiWuLover(selfChar, targetChar))
            {
                succeed = true;
            }

            return true;
        }

        // Married characters are more likely to adore others
        [HarmonyPostfix]
        [HarmonyPatch(typeof(AiHelper.Relation), "GetStartRelationSuccessRate_BoyOrGirlFriend")]
        public static void GetStartRelationSuccessRate_BoyOrGirlFriend_Patch(Character selfChar, Character targetChar, ref int __result)
        {
            if (__result != 0)
            {
                int selfAliveSpouse = DomainManager.Character.GetAliveSpouse(selfChar.GetId());
                int targetAliveSpouse = DomainManager.Character.GetAliveSpouse(targetChar.GetId());

                if (selfAliveSpouse >= 0 && selfAliveSpouse != targetChar.GetId())
                {
                    __result += 60;
                }

                if (targetAliveSpouse >= 0 && targetAliveSpouse != selfChar.GetId())
                {
                    __result += 60;
                }

            }
        }

        // Increate rape rate for taiwu related female lover
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Character), "OfflineExecuteFixedAction_MakeLove_Mutual")]
        public static bool OfflineExecuteFixedAction_MakeLove_Mutual_Patch(IRandomSource random, int targetCharId, ref bool allowRape, Character __instance,ref PeriAdvanceMonthFixedActionModification mod)
        {
            int TaiwuId = DomainManager.Taiwu.GetTaiwuCharId();
            
            // Taiwu related， do nothing
            if(TaiwuId == __instance.GetId() || TaiwuId == targetCharId ){
                return true;
            }

            Character target = DomainManager.Character.GetElement_Objects(targetCharId);
            if(__instance == null || target == null)
            {
                return true;
            }

            // Any character is taiwu related lover
            if (NtrYes_Patch.isCharTaiWuLover(__instance , target))
            {
           
                // Sex and rape check begin
     
                RelatedCharacter targetToSelf = DomainManager.Character.GetRelation(targetCharId, __instance.GetId());

                Character father, mother;

                if (__instance.GetGender() == 1)
                {
                    father = __instance;
                    mother = target;
                }
                else
                {
                    father = target;
                    mother = __instance;
                }

                // Already married or adore 
                if ( RelationType.HasRelation(targetToSelf.RelationType, 1024) || RelationType.HasRelation(targetToSelf.RelationType, 16384) )
                {
                    // Sex check
                    bool sexCheck = random.CheckPercentProb(sex_rate);

                    if (sexCheck)
                    {
  
                        bool isSexLegal = RelationType.HasRelation(targetToSelf.RelationType, 1024) ? true : false;

                        if (mod.MakeLoveTargetList == null)
                        {
                            mod.MakeLoveTargetList = new List<ValueTuple<Character, PeriAdvanceMonthFixedActionModification.MakeLoveState, bool>>();
                        }
                       
                        var makeLoveMethod = Traverse.Create(__instance).Method("OfflineMakeLove", new Type[]
                        {
                           typeof(IRandomSource),
                           typeof(Character),
                           typeof(Character),
                           typeof(bool)
                        });

                        bool isPregnant = makeLoveMethod.GetValue<bool>(random, father, mother, false);

                        if (isSexLegal)
                        {
                            mod.MakeLoveTargetList.Add(new ValueTuple<Character, PeriAdvanceMonthFixedActionModification.MakeLoveState, bool>
                            (target, PeriAdvanceMonthFixedActionModification.MakeLoveState.Legal, isPregnant));
                        }
                        else
                        {
                            mod.MakeLoveTargetList.Add(new ValueTuple<Character, PeriAdvanceMonthFixedActionModification.MakeLoveState, bool>
                            (target, PeriAdvanceMonthFixedActionModification.MakeLoveState.Illegal, isPregnant));
                        }

                        if (printLog)
                        {
                            FileLog.Log("春宵判定成功 ， 怀孕： " + isPregnant + "角色id： " + __instance.GetId() + "   " + targetCharId);
                        }
                        
                        return false;
                    }
                   
                    // Sex check false ， Rape check, 只会被欺辱
                    if (isCharTaiWuWifeOrAdore(targetCharId))
                    {
                        // Original char is male , target character is tawu related female. Adore relationship only
                        if(__instance.GetGender() == 1 && target.GetGender() == 0 && RelationType.HasRelation(targetToSelf.RelationType, 16384))
                        {
                            bool rapeCheck = random.CheckPercentProb(rape_rate);
       
                            if (mod.MakeLoveTargetList == null)
                            {
                                mod.MakeLoveTargetList = new List<ValueTuple<Character, PeriAdvanceMonthFixedActionModification.MakeLoveState, bool>>();
                            }

                            if (rapeCheck)
                            {
                                // Raping.
                                var makeLoveMethod = Traverse.Create(__instance).Method("OfflineMakeLove", new Type[]
                                 {
                                     typeof(IRandomSource),
                                     typeof(Character),
                                     typeof(Character),
                                     typeof(bool)
                                 });

                                bool isPregnant = makeLoveMethod.GetValue<bool>(random, father, mother, true);

                                if (printLog)
                                {
                                    FileLog.Log("欺辱判定成功 ， 怀孕： " + isPregnant + "角色id： " + __instance.GetId() + "   " + targetCharId);
                                }

                                mod.MakeLoveTargetList.Add(new ValueTuple<Character, PeriAdvanceMonthFixedActionModification.MakeLoveState,
                                    bool>(target, PeriAdvanceMonthFixedActionModification.MakeLoveState.RapeSucceed, isPregnant));

                                return false;
                            }
                            
                        }

                    }

                }
            }
           
            return true;
        }

        //TaiWu 's Lovers are more likely to marry other character.
        [HarmonyPostfix]
        [HarmonyPatch(typeof(AiHelper.Relation), "CanStartRelation_HusbandOrWife")]

        public static void CanStartRelation_HusbandOrWife_Patch(int selfCharId , RelatedCharacter selfToTarget, RelatedCharacter targetToSelf, int targetCharId , ref bool __result)
        {
            // Original judgement
            bool canNotMarry = (selfToTarget.RelationType & 16384) == 0 || (targetToSelf.RelationType & 16384) == 0 || 
                     RelationType.ContainBloodExclusionRelations(selfToTarget.RelationType) ||
                     DomainManager.Character.GetAliveSpouse(selfCharId) >= 0 ||
                     DomainManager.Character.GetAliveSpouse(targetCharId) >= 0;
            
            if (canNotMarry)
            {
                return;
            }

            int taiWuId = DomainManager.Taiwu.GetTaiwuCharId();

            // If any char is TaiWu, do nothing
            if(taiWuId == selfCharId || taiWuId == targetCharId)
            {
                return;
            }

            if (NtrYes_Patch.isCharTaiWuLover(selfCharId, targetCharId))
            {
                __result = true;
            }
        }
       

        /* This funciton search for whether a character is suitable for adore_ready character , if yes. add to adore ready list.
        *
        *  Judgement : Check whether in the same block
        *  There must already be a relation established
        *  Age larger than 3
        *  
        * Then call CanStartRelation_Adored for checking:  already adored? return false, then compute result based on favoribility
        * 
        *  First check for adore judgement
        *  
        *  Patch : if target character is taiwuLover , 60% add it to adore ready list
        */
        [HarmonyPrefix]
        [HarmonyPatch(typeof(CharacterDomain) , "GetPotentialRelatedCharactersInSet")]

        public static bool GetPotentialRelatedCharactersInSet_Patch(PotentialRelatedCharacters canStartRelationChars , PotentialRelatedCharacters canEndRelationChars,
            List<Character> needToCreateRelations, Character character, HashSet<int> blockCharSet , CharacterDomain __instance)
        {

            canStartRelationChars.OfflineClear();
            canEndRelationChars.OfflineClear();
            int charId = character.GetId();
            short charAge = character.GetCurrAge();
            sbyte charAgeGroup = AgeGroup.GetAgeGroup(charAge);
            bool charIsMarried = __instance.GetAliveSpouse(charId) >= 0 || !character.GetOrganizationInfo().Principal;
            bool charCanMarry = character.OrgAndMonkTypeAllowMarriage();
            sbyte selfBehaviorType = character.GetBehaviorType();

            foreach (int blockCharId in blockCharSet)
            {
                // Target character is not equal to self
                if ( !(charId == blockCharId) )
                {
                    RelatedCharacter selfToTarget;

                    // Try get relation if there is none .
                    bool flag2 = !__instance.TryGetRelation(charId, blockCharId, out selfToTarget);
                    if (flag2)
                    {
                        needToCreateRelations.Add(DomainManager.Character.GetElement_Objects(blockCharId));
                    }
                    else
                    {
                        RelatedCharacter targetToSelf;
                        bool flag3 = !__instance.TryGetRelation(blockCharId, charId, out targetToSelf);

                        // Mutual relationship
                        if (!flag3)
                        {
                            Character blockChar = DomainManager.Character.GetElement_Objects(blockCharId);

                            sbyte targetBehaviorType = blockChar.GetBehaviorType();
                            short targetAge = blockChar.GetCurrAge();
                            sbyte blockCharAgeGroup = AgeGroup.GetAgeGroup(targetAge);
                            bool flag4 = blockCharAgeGroup == 0;
                            
                            // Age judgement , must larget than 3
                            if (!flag4)
                            {
                                // Enemy
                                bool flag5 = AiHelper.Relation.CanStartRelation_Enemy(selfToTarget, selfBehaviorType);
                                if (flag5)
                                {
                                    canStartRelationChars.Enemies.Add(blockCharId);
                                }
                                else
                                {
                                    bool flag6 = AiHelper.Relation.CanEndRelation_Enemy(selfToTarget, selfBehaviorType);
                                    if (flag6)
                                    {
                                        canEndRelationChars.Enemies.Add(blockCharId);
                                    }
                                }

                                // Friend. PATCH?
                                bool flag7 = AiHelper.Relation.CanStartRelation_Friend(selfToTarget, selfBehaviorType, targetToSelf, targetBehaviorType);
                                if (flag7)
                                {
                                    canStartRelationChars.Friends.Add(blockCharId);
                                }
                                else
                                {
                                    bool flag8 = AiHelper.Relation.CanEndRelation_Friend(selfToTarget, selfBehaviorType);
                                    if (flag8)
                                    {
                                        canEndRelationChars.Friends.Add(blockCharId);
                                    }
                                }

                                // Sworn Brother/ Sister
                                bool flag9 = AiHelper.Relation.CanStartRelation_SwornBrotherOrSister(selfToTarget, selfBehaviorType, targetToSelf, targetBehaviorType);
                                if (flag9)
                                {
                                    canStartRelationChars.SwornBrothersAndSisters.Add(blockCharId);
                                }
                                else
                                {
                                    bool flag10 = AiHelper.Relation.CanEndRelation_SwornBrotherOrSister(selfToTarget, selfBehaviorType);
                                    if (flag10)
                                    {
                                        canEndRelationChars.SwornBrothersAndSisters.Add(blockCharId);
                                    }
                                }

                                // Adoptive parent
                                bool flag11 = AiHelper.Relation.CanStartRelation_AdoptiveParent(charId, selfToTarget, charAge, blockCharId, targetToSelf, targetAge);
                                if (flag11)
                                {
                                    canStartRelationChars.AdoptiveParents.Add(blockCharId);
                                }
                                else
                                {
                                    bool flag12 = AiHelper.Relation.CanStartRelation_AdoptiveChild(charId, selfToTarget, charAge, blockCharId, targetToSelf, targetAge);
                                    if (flag12)
                                    {
                                        canStartRelationChars.AdoptiveChildren.Add(blockCharId);
                                    }
                                }

                                // Adore .PATCH
                                bool flag13 = AiHelper.Relation.CanStartRelation_Adored(selfToTarget, selfBehaviorType);
                                if (flag13)
                                {   
                                    canStartRelationChars.Adored.Add(blockCharId);
                                }

                                // Patch
                                if( !flag13)
                                {
                                    // Not already adored
                                    if ( !((selfToTarget.RelationType & 16384) > 0))
                                    {
                                        int taiwuId = DomainManager.Taiwu.GetTaiwuCharId();

                                        // Taiwu if not affected
                                        if (taiwuId != charId && taiwuId != blockCharId)
                                        {
                                            // Target character is female taiwu lover, current chara is male
                                            if (isCharTaiWuWifeOrAdore(blockCharId) && character.GetGender() == 1 && blockChar.GetGender() == 0)
                                            {
                                                Random r = new Random();
                                                int num = r.Next(0, 100);

                                                if (isCharTaiwuSpouse(blockCharId))
                                                {
                                                    if (num >= 0 && num < add_adoreReady_List+10)
                                                    {
                                                        if (printLog)
                                                        {
                                                            FileLog.Log("1111太吾妻子被动加入爱慕列表判定成功 角色id： " + charId + "目标女（太吾相关）Id + " + blockChar.GetSurname() + blockChar.GetGivenName() + blockCharId);
                                                        }
                                                        
                                                        canStartRelationChars.Adored.Add(blockCharId);
                                                        flag13 = true;
                                                    }
                                                }
                                                else
                                                {
                                                    if (num >= 0 && num < add_adoreReady_List)
                                                    {
                                                        if (printLog)
                                                        {
                                                            FileLog.Log("1111太吾相关爱慕被动角色加入爱慕列表判定成功 角色id： " + charId + "目标女（太吾相关）Id + " + blockChar.GetSurname() + blockChar.GetGivenName() + blockCharId);
                                                        }
                                                       
                                                        canStartRelationChars.Adored.Add(blockCharId);
                                                        flag13 = true;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                                // Original Code
                                if (!flag13)
                                {

                                    bool flag14 = AiHelper.Relation.CanStartRelation_BoyOrGirlFriend(selfToTarget, selfBehaviorType, targetToSelf, targetBehaviorType);
            
                                    if (flag14)
                                    {
                                        canStartRelationChars.BoyAndGirlFriends.Add(blockCharId);
                                    }
                                    else
                                    {
                                        bool flag15 = AiHelper.Relation.CanEndRelation_BoyOrGirlFriend(selfToTarget, selfBehaviorType, targetToSelf, targetBehaviorType);
                                        if (flag15)
                                        {
                                            canEndRelationChars.BoyAndGirlFriends.Add(blockCharId);
                                        }
                                        else
                                        {
                                            bool blockCharIsMarried = __instance.GetAliveSpouse(blockCharId) >= 0 || !blockChar.GetOrganizationInfo().Principal;
                                            bool blockCharCanMarry = blockChar.OrgAndMonkTypeAllowMarriage();
                                            bool flag16 = charAgeGroup == 2 && blockCharAgeGroup == 2 && charCanMarry && blockCharCanMarry && !charIsMarried && !blockCharIsMarried && AiHelper.Relation.CanStartRelation_HusbandOrWife(charId, selfToTarget, selfBehaviorType, blockCharId, targetToSelf, targetBehaviorType);
                                            if (flag16)
                                            {
                                                canStartRelationChars.HusbandsAndWives.Add(blockCharId);
                                            }
                                        }
                                    }


                                }


                            }
                        }
                    }
                }
            }

            return false;
        }


        // Adore .
        // Second judgement
        // Adore target already selected , check whether start adore application
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Character) , "GetStartOrEndRelationTarget")]
        public static void GetStartOrEndRelationTarget_Patch(Character __instance , IRandomSource random, short aiRelationsTemplateId, List<int> selectableChars,  ref Character __result)
        {
            //Only adore or boy girl friend
            if(aiRelationsTemplateId != (short)2 && aiRelationsTemplateId != (short)3)
            {           
                return;
            }

            //Already selected , or no available adore ready character
            if ( (__result != null) ||  (selectableChars.Count == 0) )
            {
                return;
            }

            int currentCharID = __instance.GetId();
            int TaiWuId = DomainManager.Taiwu.GetTaiwuCharId();

            //Current char is taiwu,  Unlikely to happen
            if( (currentCharID == TaiWuId))
            {
                return;
            }


            // 太吾相关爱慕角色为发起者
            if (isCharTaiWuWifeOrAdore(currentCharID) && __instance.GetGender() == 0)
            {
                if (isCharTaiwuSpouse(currentCharID) && random.CheckPercentProb(start_adore_apply))
                {

                    int targetCharId = selectableChars.GetRandom(random);
                    __result = DomainManager.Character.GetElement_Objects(targetCharId);

                    if (printLog)
                    {
                        FileLog.Log("22222222太吾妻子主动主动主动爱慕判定失败。。。， 改为成功， 发起者ID： " + __instance.GetSurname() + __instance.GetGivenName() + currentCharID + "接受者： " + targetCharId);
                    }

                }
                else if (random.CheckPercentProb(start_adore_apply-10))
                {

                    int targetCharId = selectableChars.GetRandom(random);
                    __result = DomainManager.Character.GetElement_Objects(targetCharId);

                    if (printLog)
                    {
                        FileLog.Log("22222222太吾相关女性主动主动主动爱慕判定失败。。。， 改为成功， 发起者ID： " + __instance.GetSurname() + __instance.GetGivenName() + currentCharID + "接受者： " + targetCharId);
                    }
                    
                }

                return;
            }


            // 太吾相关爱慕角色接受者
            HashSet<int> TaiwuAdoreCharacter = DomainManager.Character.GetRelatedCharIds(TaiWuId, 16384);
            HashSet<int> TaiwuSpouseCharacter = DomainManager.Character.GetRelatedCharIds(TaiWuId, 1024);

            for(int i = 1; i <= 3; i++)
            {
                int targetCharId = selectableChars.GetRandom(random);

                // Taiwu wife is selected by a male character
                if (TaiwuSpouseCharacter.Contains(targetCharId) && __instance.GetGender() == 1 && DomainManager.Character.GetElement_Objects(targetCharId).GetGender() == 0)
                {
                    if (random.CheckPercentProb(start_adore_apply+10))
                    {
                        __result = DomainManager.Character.GetElement_Objects(targetCharId);

                        if (printLog)
                        {
                            FileLog.Log("22222222太吾妻子被动被动被动爱慕判定失败。。。， 改为成功， 发起者ID： " + currentCharID + "接受者(太吾相关)： " + __result.GetSurname() + __result.GetGivenName() + targetCharId);
                        }
                        
                        return;
                    }
                }

                // Taiwu adored female character is selected by a male character
                if (TaiwuAdoreCharacter.Contains(targetCharId) && __instance.GetGender() == 1 && DomainManager.Character.GetElement_Objects(targetCharId).GetGender()==0  )
                {
                    if (random.CheckPercentProb(start_adore_apply))
                    {
                        __result = DomainManager.Character.GetElement_Objects(targetCharId);

                        if (printLog)
                        {
                            FileLog.Log("22222222太吾相关爱慕角色被动被动被动爱慕判定失败。。。， 改为成功， 发起者ID： " + currentCharID + "接受者(太吾相关)： " + __result.GetSurname() + __result.GetGivenName() + targetCharId);
                        }
                  
                        return;
                    }
                   
                }
  
            }

            return;
        }



        /* Increase sex related rate for TaiWu realated female character ,, 
        *
        * Last check for adore judgement
        */
        [HarmonyPostfix]
        [HarmonyPatch(typeof(AiHelper.Relation), "GetStartRelationSuccessRate_SexRelationBaseRate")]
        public static void GetStartRelationSuccessRate_Adored_Patch(Character selfChar, Character targetChar, ref int __result)
        {

            if (NtrYes_Patch.isCharTaiWuLover(selfChar, targetChar))
            {
                if (__result == int.MinValue)
                {
                    __result = final_rate/2;
                }
                else
                {
                    __result += final_rate;
                }

                if (printLog)
                {
                    FileLog.Log("33333333太吾相关爱慕角色最终判定开始。。。成功率： " + __result);
                }
         
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(PregnantState),"CheckPregnant")]
        public static void CheckPregnant_Patch(IRandomSource random, Character father, Character mother, bool isRape, ref bool __result)
        {
            int taiwuId = DomainManager.Taiwu.GetTaiwuCharId();

            // Not taiwu character. Not true result
            if(taiwuId == father.GetId() || taiwuId == mother.GetId() || __result == true)
            {
                return;
            }

            // Not same gender. Mother not already pregnant.
            int num;
            if(father.GetGender() == mother.GetGender() ||
                mother.GetFeatureIds().Contains(197) || 
                DomainManager.Character.TryGetElement_PregnancyLockEndDates(mother.GetId(), out num))
            {
                return;
            }

            if (isCharTaiWuWifeOrAdore(mother.GetId()) && mother.GetGender() == 0 && random.CheckPercentProb(pregnant_chance) )
            {
                __result = true;
            }

        }

    }
}
