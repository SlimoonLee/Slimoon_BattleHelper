using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityModManagerNet;
using Random = UnityEngine.Random;

namespace Slimoon_BattleHelper

{
    [HarmonyPatch(typeof(BattleSystem), "SetChooseAttackPart")]
	public static class BattleSystem_SetChooseAttackPart_Patch
    {

		public static void Prefix(ref int typ)
        {
            if (Main.settings.AutoChange)
            {
				var SixCost = BattleSystem.instance.choosePartGetCost;
				if (Main.settings.ForcedBuWei && !Main.settings.WeakBuWei)
				{
					typ = Main.settings.ForcedBuWeiType;
					if (typ == 7) typ = -1;
					return;
				}
				var Chance = BattleSystem.instance.attackPartChooseObbs;
				var CostBar = BattleSystem.instance.actorActionCost.Values;
				int LastCost = -1;
				if (CostBar.Count > 0) LastCost = CostBar.Last();
				var PossiblePart = new List<int>();
				if (Main.settings.ForYunv && LastCost >= 0)
				{

					for(int i = 0; i < SixCost.Length; i++)
                    {
						if (SixCost[i] == LastCost)
							PossiblePart.Add(i);
                    }
				}
				else if (Main.settings.ForcedCost)
                {
					for (int i = 0; i < SixCost.Length; i++)
					{
						if (SixCost[i] == Main.settings.ForcedCostType)
							PossiblePart.Add(i);
					}
				}
				if (PossiblePart.Count == 0)
				{
					for (int i = 0; i < SixCost.Length; i++)
					{
						PossiblePart.Add(i);
					}
				}

				if(Main.settings.ChanceGoFirst)
                {
                    if (Main.settings.ChanceEnhancedByHand)
                    {
						for(int i = 0; i < Chance.Length; i++)
                        {
							Chance[i] = ((int)(100 - (100 - Chance[i]) * Mathf.Max((75 - Chance[i]) / 75f, 0f)));
							if (Chance[i] > 100) Chance[i] = 100;
                        }
						if (Main.settings.GivpNoBest)
						{
							if (!Chance.Contains(100))
							{
								typ = -1;
								return;
							}

						}
					}

                    if (Main.settings.DamGoFirst)
                    {
                        for(int i = 0; i < Chance.Length; i++)
                        {
							Chance[i] *= i == 0 ? 60 : i == 1 ? 60 : i == 2 ? 100 : 40;
                        }
                    }

					int MaxChance = 0;
					foreach (int i in PossiblePart)
					{
						if (Chance[i] > MaxChance) MaxChance = Chance[i];
					}
					PossiblePart.RemoveAll(q => Chance[q] < MaxChance);
				}

				if (true)
				{
					if (Main.settings.ForcedBuWei && Main.settings.WeakBuWei && PossiblePart.Contains(Main.settings.ForcedBuWeiType))
					{
						typ = Main.settings.ForcedBuWeiType;
						return;
					}
					else
					{
						typ = PossiblePart[Random.Range(0, PossiblePart.Count - 1)];
						return;
					}
				}

				/*Dictionary<int, int> PossiblePartAndChance = new Dictionary<int, int>();
				foreach(var i in PossiblePart)
                {
					PossiblePartAndChance.Add(i, Chance[i]);
                }
				int MaxChance = 0;
				foreach(var i in PossiblePartAndChance)
                {
					if (i.Value > MaxChance) MaxChance = i.Value;
					//string Shi = SixCost[i.Key] < 0 ? "没有式" : Main.CostType[SixCost[i.Key]];
					//Debug.Log($"部位：{Main.BuWei[i.Key]}，修正概率（如有）：{i.Value}，对应式：{Shi}");
                }
				//Debug.Log("这是个测试，考虑到概率最终选定的部位为：");
				Dictionary<int,int> FinalPairs = (Dictionary<int, int>)(from i in PossiblePartAndChance where i.Value == MaxChance select i);
				if(Main.settings.ForcedBuWei && Main.settings.WeakBuWei && FinalPairs.ContainsKey(Main.settings.ForcedBuWeiType))
                {
					typ = Main.settings.ForcedBuWeiType;
					return;
				}
                else
                {
					typ = FinalPairs.ElementAt(Random.Range(0, FinalPairs.Count - 1)).Key;
					return;
				}*/


				/*foreach (var i in FinalPairs)
				{

					//string Shi = SixCost[i.Key] < 0 ? "没有式" : Main.CostType[SixCost[i.Key]];
					//Debug.Log($"部位：{Main.BuWei[i.Key]}，修正概率（如有）：{i.Value}，对应式：{Shi}");
				}*/
			}
		}
    }
}
