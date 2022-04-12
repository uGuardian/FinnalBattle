using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using UnityEngine;
using abcdcode_brawl_MOD;

namespace FinallyBeyondTheTime.HarmonyOptional {
	public class FinallHarmony {
		readonly Harmony harmony = new Harmony("LoR.uGuardian.Finnal");
		public void Load() {
			try {
				harmony.PatchAll();
				FinnalConfig.HarmonyMode = 1;
				if (FinnalConfig.Instance.DiceSpeedUp) {
					harmony.Patch(typeof(RencounterManager).GetMethod(nameof(RencounterManager.StartRencounter), AccessTools.all),
						postfix: new HarmonyMethod(typeof(EnableNoDelay).GetMethod(nameof(EnableNoDelay.Postfix))));
				}
			} catch (Exception ex) {
				Debug.LogException(ex);
			}

			try {
				CheckOtherMods();
			} catch (Exception ex) {
				Debug.LogException(ex);
			}
		}
		public void CheckOtherMods() {
			List<string> assembly = new List<string>();
			bool brawlFound = false;
			foreach (var a in AppDomain.CurrentDomain.GetAssemblies()) {
				switch (a.GetName().Name) {
					case "BaseMod" when FinnalConfig.HarmonyMode < 2 && a.GetType("SummonLiberation.Harmony_Patch") != null:
						UnityEngine.Debug.Log("Finnal: BaseMod SummonLiberation Found");
						FinnalConfig.HarmonyMode = 2;
						break;
					case "abcdcode_brawl_MOD":
						harmony.Patch(typeof(RencounterManager).GetMethod(nameof(RencounterManager.EndRencounter), AccessTools.all),
							postfix: new HarmonyMethod(typeof(BrawlModPatch).GetMethod(nameof(BrawlModPatch.Postfix))));
						brawlFound = true;
						break;
				}
				if (brawlFound && FinnalConfig.HarmonyMode == 2) {
					break;
				}
			}
			/*
			if (assembly.Contains("BaseMod")) {
				UnityEngine.Debug.Log("Finnal: BaseMod Found");
				FinnalConfig.HarmonyMode = 2;
			}
			if (AppDomain.CurrentDomain.GetAssemblies().Any(a => a.GetName().Name.Contains("BaseMod")
				&& a.GetType("SummonLiberation.Harmony_Patch") != null)) {
					UnityEngine.Debug.Log("Finnal: BaseMod SummonLiberation Found");
					FinnalConfig.HarmonyMode = 2;
			}
			*/
		}
	}

	/* Removed this patch because it doesn't quite work properly and it's unneccecary overhead.
	[HarmonyPatch(typeof(BattleEmotionCoinSlotUI))]
	class EmotionCoinExceptionSuppressor {
		[HarmonyFinalizer]
		[HarmonyPatch("Init")]
		public static Exception Init_Finalizer(Exception __exception) {
			if (Singleton<StageController>.Instance.EnemyStageManager is EnemyTeamStageManager_UltimaAgain) {
				return null;
			}
			return __exception;
		}
		[HarmonyFinalizer]
		[HarmonyPatch("StartMoving")]
		public static Exception StartMoving_Finalizer(Exception __exception) {
			if (Singleton<StageController>.Instance.EnemyStageManager is EnemyTeamStageManager_UltimaAgain) {
				return null;
			}
			return __exception;
		}
		// [HarmonyFinalizer]
		// [HarmonyPatch(typeof(BattleEmotionCoinUI), "Init")]
		// public static Exception Init_Finalizer2(Exception __exception) {
		// 	if (Singleton<StageController>.Instance.EnemyStageManager is EnemyTeamStageManager_UltimaAgain) {
		// 		return null;
		// 	}
		//	 return __exception;
		// }
	}
	*/
	// [HarmonyPatch(typeof(RencounterManager), "StartRencounter")]
	static class EnableNoDelay {
		public static void Postfix(RencounterManager __instance) {
			if (Singleton<StageController>.Instance.EnemyStageManager is EnemyTeamStageManager_UltimaAgain) {
				__instance.SetNodelay(true);
			}
		}
	}
	// [HarmonyPatch(typeof(RencounterManager), "EndRencounter")]
	static class BrawlModPatch {
		public static void Postfix(RencounterManager __instance) {
			if (Harmony.HasAnyPatches("LOR.abcdcode.Brawl")) {
				BrawlMod.Fix(__instance);
			}
		}
	}
	internal static class BrawlMod {
		internal static void Fix(RencounterManager instance) {
			instance.EndRencounterForcely();
			BrawlInitializer.Rencounters.Remove(instance);
			UnityEngine.Object.DestroyImmediate(instance.gameObject);
		}
	}
	[HarmonyPatch(typeof(BattleUnitTargetArrowManagerUI), "ShowTargetLines")]
	static class ShowTargetLinesPatch {
		// This patch is always applied because when it occurs it's always a bug.
		public static Exception Finalizer(Exception __exception) {
			if ((__exception != null) && !(__exception is ArgumentOutOfRangeException)) {
				Debug.LogException(__exception);
			}
			return null;
		}
	}
}