using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityModManagerNet;

namespace Slimoon_BattleHelper

{


    public static class Main
    {
        public static bool enabled;
        public static Settings settings;
        public static UnityModManager.ModEntry.ModLogger Logger;
		public static readonly string[] CostType = new string[]
		{
			"掷",
			"弹",
			"御",
			"劈",
			"刺",
			"撩",
			"崩",
			"点",
			"拿",
			"音",
			"缠",
			"咒",
			"机",
			"药",
			"毒",
			"无",
			"扫",
			"万",
			"杀"
		};
		public static readonly string[] BuWei = new string[]
{
			"胸背",
			"腰腹",
			"头颈",
			"左臂",
			"右臂",
			"左腿",
			"右腿",
			"放弃变招"
};

		public static bool Load(UnityModManager.ModEntry modEntry)
        {
            var harmony = new Harmony(modEntry.Info.Id);
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            settings = Settings.Load<Settings>(modEntry);

            Logger = modEntry.Logger;

            modEntry.OnToggle = OnToggle;
            modEntry.OnGUI = OnGUI;
            modEntry.OnSaveGUI = OnSaveGUI;

            return true;
        }

        public static bool OnToggle(UnityModManager.ModEntry modEntry, bool value)
        {

            enabled = value;

            return true;
        }

        static void OnGUI(UnityModManager.ModEntry modEntry)
        {
			settings.G_Cheat = GUILayout.Toggle(settings.G_Cheat, "功法作弊", new GUILayoutOption[0]);
            if (settings.G_Cheat)
            {
				settings.OnlyForTaiWu = GUILayout.Toggle(settings.OnlyForTaiWu, "只对我方生效", new GUILayoutOption[0]);
				settings.AllWeaponPermitted = GUILayout.Toggle(settings.AllWeaponPermitted, "去除主动功法要求当前武器式", new GUILayoutOption[0]);
				settings.GodModeForGongFaUse = GUILayout.Toggle(settings.GodModeForGongFaUse, "无条件主动使用功法", new GUILayoutOption[0]);
				settings.Cheat = GUILayout.Toggle(settings.Cheat, "开局作弊（开局万用式+满提起架势+3倍治疗次数）", new GUILayoutOption[0]);
			}
			GUILayout.BeginHorizontal();
			settings.TwoPMove = GUILayout.Toggle(settings.TwoPMove, "（仅我方）两点反复横跳", new GUILayoutOption[0]);
            if (settings.TwoPMove)
            {
				GUILayout.Label($"第一个点：{Main.settings.SaveP1}");
				Main.settings.SaveP1 = (int)GUILayout.HorizontalSlider(Main.settings.SaveP1, 20f, 90f, new GUILayoutOption[]
			{
				GUILayout.Width(300),
			});
				GUILayout.Label($"第二个点：{Main.settings.SaveP2}");
				Main.settings.SaveP2 = (int)GUILayout.HorizontalSlider(Main.settings.SaveP2, 20f, 90f, new GUILayoutOption[]
			{
				GUILayout.Width(300),
			});
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.BeginVertical();
			settings.AutoChange = GUILayout.Toggle(settings.AutoChange, "开启自动点击变招（原理是自动点击，无需开启游戏内的自动变招）", new GUILayoutOption[0]);
            if (settings.AutoChange)
            {
				/*GUILayout.BeginHorizontal();
				GUILayout.Label("当下述三种变招方式决定的部位无法同时满足时，优先考虑：", new GUILayoutOption[0]);
				settings.PartGoFirst = GUILayout.SelectionGrid(settings.PartGoFirst, new string[]
				{
					"获取式",
					"成功率"
				}, 2, new GUILayoutOption[0]);
				GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();*/
				GUILayout.Label("变招控制：部位调整（指定部位会跳过后续两项变招控制）", new GUILayoutOption[0]);
				GUILayout.BeginHorizontal();
				settings.ForcedBuWei = GUILayout.Toggle(settings.ForcedBuWei, "变招指定部位：", new GUILayoutOption[0]);
				if (settings.ForcedBuWei)
                {
					GUILayout.BeginVertical();
					settings.ForcedBuWeiType = GUILayout.SelectionGrid(settings.ForcedBuWeiType, BuWei, 8, new GUILayoutOption[0]);
					settings.WeakBuWei = GUILayout.Toggle(settings.WeakBuWei, "低优先级模式（不会跳过后续两项，如后续两项判定后仍有多个可选部位，且包含此处的指定部位，则选择之）", new GUILayoutOption[0]);
					GUILayout.EndVertical();
				}
				GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();

				GUILayout.Label("变招控制：获取式", new GUILayoutOption[0]);
				settings.ForYunv = GUILayout.Toggle(settings.ForYunv, "优先变招相同式（正玉女神剑）", new GUILayoutOption[0]);
                if (!settings.ForYunv)
                {
					GUILayout.BeginHorizontal();
					settings.ForcedCost = GUILayout.Toggle(settings.ForcedCost, "变招获取指定式（如当前武器无指定式则此项自动跳过）：", new GUILayoutOption[0]);
					if (settings.ForcedCost)
						settings.ForcedCostType = GUILayout.SelectionGrid(settings.ForcedCostType, CostType, 10, new GUILayoutOption[0]);
					GUILayout.FlexibleSpace();
					GUILayout.EndHorizontal();
				}
				
				GUILayout.Label("变招控制：成功率", new GUILayoutOption[0]);
				GUILayout.BeginHorizontal();
				settings.ChanceGoFirst = GUILayout.Toggle(settings.ChanceGoFirst, "优先高成功率部位", new GUILayoutOption[0]);
                if (settings.ChanceGoFirst)
                {
					settings.ChanceEnhancedByHand = GUILayout.Toggle(settings.ChanceEnhancedByHand, "成功率计算时考虑逆三十六加成", new GUILayoutOption[0]);
                    if (settings.ChanceEnhancedByHand)
                    {
						settings.GivpNoBest = GUILayout.Toggle(settings.GivpNoBest, "无必中位置时放弃变招", new GUILayoutOption[0]);
					}
					settings.DamGoFirst = GUILayout.Toggle(settings.DamGoFirst, "计算成功率×部位伤害系数", new GUILayoutOption[0]);

				}
				GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndVertical();
            //此处绘制UI
        }

        static void OnSaveGUI(UnityModManager.ModEntry modEntry)
        {
            settings.Save(modEntry);
        }

    }

    //此处写harmonypatch
    /// <summary>
    ///   // , new Type[] { typeof(bool), typeof(int) }
    /// </summary>
    [HarmonyPatch(typeof(BattleSystem), "GongFaCanUse")]
    public static class BattleSystem_GongFaCanUse_Patch
    {
        public static bool Prefix(ref bool __result, bool isActor, int _gongFaId, Dictionary<int, int> _costs)
        {
            if (Main.settings.OnlyForTaiWu && !isActor)
            {
				return true;
            }
            if (Main.settings.GodModeForGongFaUse)
            {
				__result = true;
				return false;
            }
			bool flag = _gongFaId == 0 || BattleSystem.instance.gongFaPause;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				int num = BattleSystem.instance.ActorId(isActor, false);
				int num2;
				int num3;
				bool flag3;
				if (isActor)
				{
					num2 = BattleSystem.instance.actorUseGongFaId;
					num3 = BattleSystem.instance.actorNeedUseGongFa;
					bool flag2 = BattleSystem.instance.actorIsDefing;
					flag3 = (BattleSystem.instance.actorDoingOtherTyp > 0 || BattleSystem.instance.actorDoOtherTyp > 0);
				}
				else
				{
					num2 = BattleSystem.instance.enemyUseGongFaId;
					num3 = BattleSystem.instance.enemyNeedUseGongFa;
					bool flag2 = BattleSystem.instance.enemyIsDefing;
					flag3 = (BattleSystem.instance.enemyDoingOtherTyp > 0 || BattleSystem.instance.enemyDoOtherTyp > 0);
				}
				bool flag4 = flag3 || (num2 > 0 && (DateFile.GetCombatSkillEquipType(_gongFaId) != CombatSkillEquipType.Attack || DateFile.GetCombatSkillDataInt(num2, 245, BattleSystem.instance.ActorId(isActor, false), true) == 0) && (DateFile.GetCombatSkillEquipType(_gongFaId) != CombatSkillEquipType.Agile || DateFile.GetCombatSkillDataInt(num2, 246, BattleSystem.instance.ActorId(isActor, false), true) == 0) && (DateFile.GetCombatSkillEquipType(_gongFaId) != CombatSkillEquipType.Defence || DateFile.GetCombatSkillDataInt(num2, 243, BattleSystem.instance.ActorId(isActor, false), true) == 0)) || num3 > 0;
				if (flag4)
				{
					result = false;
				}
				else
				{
					bool flag5 = !BattleSystem.instance.StrengthMax(isActor, _gongFaId);
					if (flag5)
					{
						result = false;
					}
					else
					{
						int weaponId = BattleSystem.instance.GetWeaponId(isActor, -1, false);
						int[] gongfaRange = BattleSystem.instance.GetGongfaRange(isActor, weaponId, _gongFaId);
						bool flag6 = (BattleSystem.instance.battleRange < gongfaRange[0] || BattleSystem.instance.battleRange > gongfaRange[1]) && DateFile.GetCombatSkillDataInt(_gongFaId, 235, num, true) == 0;
						if (flag6)
						{
							result = false;
						}
						else
						{
							bool flag7 = (int.Parse(DateFile.instance.gongFaDate[_gongFaId][1]) == 1 && BattleVaule.instance.HaveMoveCost(isActor, true, false, true) > 0) || (isActor ? BattleVaule.instance.actorMoveCosts.Count : BattleVaule.instance.enemyMoveCosts.Count) < DateFile.GetCombatSkillConsumedMobility(_gongFaId, num) || (isActor ? BattleSystem.instance.actorBugSize : BattleSystem.instance.enemyBugSize) < int.Parse(DateFile.instance.gongFaDate[_gongFaId][39]) || (int.Parse(DateFile.instance.GetItemDate(weaponId, 902, true, -1)) != 0 && int.Parse(DateFile.instance.GetItemDate(weaponId, 901, true, -1)) < int.Parse(DateFile.instance.gongFaDate[_gongFaId][38]));
							if (flag7)
							{
								result = false;
							}
							else
							{
								List<int> list = new List<int>(DeBug_GetWeaponCost(weaponId));
								Dictionary<int, int> gongFaNeedCost = BattleSystem.instance.GetGongFaNeedCost(isActor, _gongFaId);
								List<int> list2 = new List<int>(gongFaNeedCost.Keys);
								bool flag8 = list2.Count > 0;
								if (flag8)
								{
									int i = 0;
									while (i < list2.Count)
									{
										int num4 = list2[i];
										int num5 = gongFaNeedCost[num4];
										bool flag9 = list.Contains(num4);
                                        if (Main.settings.AllWeaponPermitted)
                                        {
											flag9 = true;
                                        }
										if (flag9)
										{
											bool flag10 = BattleSystem.instance.GetActorCost(isActor).ContainsKey(17);
											if (!flag10)
											{
												bool flag11 = _costs.ContainsKey(num4) && _costs[num4] >= num5;
												if (!flag11)
												{
													goto IL_316;
												}
											}
											i++;
											continue;
										}
									IL_316:
										__result = false;
										return false;
									}
								}
								int combatSkillDataInt = DateFile.GetCombatSkillDataInt(_gongFaId, 206, num, true);
								bool flag12 = combatSkillDataInt == 0;
								if (flag12)
								{
									result = false;
								}
								else
								{
									int equipType = int.Parse(DateFile.instance.gongFaDate[_gongFaId][6]) - 1;
									bool flag13 = !BattleSystem.IsCombatSkillCooledDown(isActor, equipType, _gongFaId);
									result = !flag13;
								}
							}
						}
					}
				}
			}
			__result = result;
			return false;
		}

        public static  List<int> DeBug_GetWeaponCost(int weaponId)//就很怪，原方法不能用，原版照抄就能用
        {
            string[] array = DateFile.instance.GetItemDate(weaponId, 7, true, -1).Split(new char[]
            {
            '|'
            });
            List<int> list = new List<int>();
            for (int i = 0; i < array.Length; i++)
            {
                bool flag = (array[i] ?? "").Length == 0;
                if (!flag)
                {
                    list.Add(int.Parse(array[i]));
                }
            }
            return list;
        }
    }

	[HarmonyPatch(typeof(BattleSystem), "Update")]
	public static class BattleSystemUpdate
	{
		// Token: 0x06000005 RID: 5 RVA: 0x00002194 File Offset: 0x00000394
		private static bool Prefix(BattleSystem __instance)
		{
			if (Main.settings.TwoPMove)
			{
				if (__instance.actorNeedRangeSlider.interactable && !__instance.autoBattle.isOn)
				{
					if (__instance.battleRange == Main.settings.SaveP1)
					{
						__instance.SetNeedRange(true, Main.settings.SaveP2, false);
					}
					else
					{
						__instance.SetNeedRange(true, Main.settings.SaveP1, false);
					}
				}
				__instance.autoBattle.transform.Rotate(new Vector3(0f, 0f, 100f) * Time.deltaTime);
			}
            if (Main.settings.AutoChange)
            {
				if (BattleSystem.instance.Part7Bar.fillAmount < 1f)
					BattleSystem.instance.Part7Bar.fillAmount = 1.1f;
			}
			return true;
		}
	}

	[HarmonyPatch(typeof(BattleSystem), "GetActor")]//战斗作弊用
	public static class ShowBattleWindow_Patch
	{
		public static void Postfix()
		{
			if (!Main.settings.Cheat) return;
			var List = new List<int>(BattleSystem.instance.actorDoRemovePoisonSize.Keys);
			for (int i = 0; i < List.Count; i++)
			{
				int key = List[i];
				BattleSystem.instance.actorDoRemovePoisonSize[key] *= 3;
			}
			var List2 = new List<int>(BattleSystem.instance.actorDoHealSize.Keys);
			for (int i = 0; i < List2.Count; i++)
			{
				int key = List2[i];
				BattleSystem.instance.actorDoHealSize[key] *= 3;
			}
			BattleSystem.instance.UpdateMagic(true, 30000, false);
			BattleSystem.instance.UpdateStrength(true, 30000, false);
			for (int i = 0; i < 5; i++)
				BattleSystem.instance.AddActionCostIcon(true, 17, false, true, -1);
			for (int i = 0; i < 4; i++)
			{
				//BattleSystem.AddSp(false, i, -5);
				BattleSystem.AddSp(true, i, 5);
			}
		}
	}
}
