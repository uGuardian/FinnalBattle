using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using abcdcode_brawl_MOD;

namespace FinallyBeyondTheTime {
	public class FinallHarmony {
		public static void Load() {
			Harmony harmony = new Harmony("LoR.uGuardian.Finnal");
			FinnalConfig.HarmonyMode = 1;
			CheckSummonLiberation();
			harmony.PatchAll();
		}
		public static void CheckSummonLiberation() {
			List<String> assembly = new List<String>();
			foreach (var a in AppDomain.CurrentDomain.GetAssemblies()) {
				assembly.Add(a.GetName().Name);
			}
			if (assembly.Contains("BaseMod")) {
				UnityEngine.Debug.Log("Finall: BaseMod Found");
				FinnalConfig.HarmonyMode = 2;
			}
		}
	}

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
		[HarmonyFinalizer]
		[HarmonyPatch(typeof(BattleEmotionCoinUI), "Init")]
        public static Exception Init_Finalizer2(Exception __exception) {
			if (Singleton<StageController>.Instance.EnemyStageManager is EnemyTeamStageManager_UltimaAgain) {
    			return null;
			}
            return __exception;
        }
    }
	[HarmonyPatch(typeof(RencounterManager), "StartRencounter")]
    class EnableNoDelay {
        public static void Postfix(RencounterManager __instance) {
    		if (FinnalConfig.Instance.DiceSpeedUp && Singleton<StageController>.Instance.EnemyStageManager is EnemyTeamStageManager_UltimaAgain) {
    			__instance.SetNodelay(true);
    		}
    	}
    }
	[HarmonyPatch(typeof(RencounterManager), "EndRencounter")]
	class BrawlModPatch {
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
	class ShowTargetLinesPatch {
		// This patch is always applied because when it occurs it's always a bug.
		public static Exception Finalizer(Exception __exception) {
			if (!(__exception is ArgumentOutOfRangeException)) {
				Debug.LogException(__exception);
			}
			return null;
		}
	}
}