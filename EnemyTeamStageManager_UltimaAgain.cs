#if DEBUG
#endif

#if DEBUG_CryingChildren
#warning Compiled with CryingChildren debug option
#endif
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UI;
using UnityEngine;
using LOR_DiceSystem;
using FinallyBeyondTheTime.PassiveAbilities;

namespace FinallyBeyondTheTime
{
	public partial class EnemyTeamStageManager_UltimaAgain : EnemyTeamStageManager
	{
		public override void OnWaveStart()
		{
			this.phase = 0;
			#if DEBUG_PhaseSkip
			#warning Compiled with PhaseSkip debug option
			this.phase = 6;
			#endif
			currentFloor = Singleton<StageController>.Instance.GetCurrentStageFloorModel().Sephirah;
			Debug.Log($"Finnal: Initial floor is {currentFloor}");
			angelaappears = false;
			remains.Clear();
			foreach (LibraryFloorModel libraryFloorModel in LibraryModel.Instance.GetOpenedFloorList())
			{
				if (libraryFloorModel.GetUnitDataList().Count > 0)
				{
					StageLibraryFloorModel stageLibraryFloorModel = new StageLibraryFloorModel();
					stageLibraryFloorModel.Init(Singleton<StageController>.Instance.GetStageModel(), libraryFloorModel, isRebattle: false);
					if (stageLibraryFloorModel.Sephirah != currentFloor || currentFloor == SephirahType.Keter)
					{
						remains.Add(stageLibraryFloorModel);
					} else {
						Debug.Log($"Finnal: Floor list skipping over {currentFloor}");
					}
				}
			}
			if (currentFloor != SephirahType.Keter)
			{
				Debug.Log("Finnal: Inserting Keter at top of floor list");
				remains.Insert(0, remains[remains.Count - 1]);
			}
			// finnalFormation = new FormationModel(Singleton<StageController>.Instance.GetCurrentWaveModel().GetFormation().XmlInfo);
			// curPhase = Phase.FIRST; // This is to trick hardcoded logic.
			XiaoNullSettingCheck();
		}
		void XiaoNullSettingCheck() {
			if (FinnalConfig.Instance.XiaoNullStart < 0) {
				passives.Add(typeof(PassiveAbility_150238), null);
			} else if (FinnalConfig.Instance.XiaoNullStart == 0 && FinnalConfig.Instance.XiaoNullCooldown == 1) {
				return;
			} else {
				passives.Add(typeof(PassiveAbility_150238), typeof(PassiveAbility_150238_Finnal));
			}
		}

		public override void OnRoundEndTheLast()
		{
			if (FFH_Active) {
				FFH._finalTurnCount = (FFH._finalTurnCount + 1) % 7;
			}
			CheckFloorFinnal();
			// Phases from T3 SotC onwards have more complicated mechanics and less characters, so we stop cleaning every round just to be sure
			if (phase < 7)
			{
				CleanUp();
			} else {
				CleanUp(psuedo: true);
			}
			CheckPhaseFinnal();
			// If the harmony finalizer is not implemented, disable enemy targeting toggles to prevent a soft-lock
			if (FinnalConfig.HarmonyMode == 0) {
				SingletonBehavior<BattleManagerUI>.Instance.ui_emotionInfoBar.targetingToggle.SetToggle(0, false);
			}
			switch (phase) {
				case 10:
					BSH.CheckPhase();
					break;
			}
		}

		public override bool IsStageFinishable()
		{
			return finished;
		}

		public override void OnEndBattle() {
			SingletonBehavior<HexagonalMapManager>.Instance.ResetMapSetting();
			Singleton<StageController>.Instance._stageType = StageType.Invitation;
		}

		public override void OnRoundStart()
		{
			if (passiveAbility_240528_list.Count > 0) {
				if (!passiveAbility_240528_list.All(passive => passive.destroyed) && !passiveAbility_240528_list.All(passive => passive.Owner.IsDead())) {
					List<BattleUnitModel> aliveList = BattleObjectManager.instance.GetAliveList(Faction.Player);
					// Debug.Log("Finnal: ChildImmobilizeNerf = "+FinnalConfig.Instance.ChildImmobilizeNerf);
					if (aliveList.Count > 1 || !FinnalConfig.Instance.ChildImmobilizeNerf && aliveList.Count > 0) {
						RandomUtil.SelectOne(aliveList).bufListDetail.AddKeywordBufThisRoundByCard(KeywordBuf.Stun, 1, actor: null);
					} else {
						Debug.Log($"Finnal: ChildImmobilizeNerf = {FinnalConfig.Instance.ChildImmobilizeNerf}");
					}
				} else {
					passiveAbility_240528_list.Clear();
				}
			}
			// Related to BGM
			PhaseMapCheck();
			switch (phase) {
				case 10:
					BSH.PsuedoManager.StartPhaseRound();
					break;
			}
		}
		AudioClip bsTheme;
		private void PhaseMapCheck() {
			if (lastPhase != phase) {
				switch (phase) {
					case 10:
					case 12:
						break;
					default:
						// curPhase = Phase.FIRST; // This is to trick hardcoded logic.
						Singleton<StageController>.Instance._stageType = StageType.Invitation;
						break;
				}
				MDH.ClearDialog();
				ThemeEnforce(true);
				switch (phase) {
					case 14:
						break;
					case 13:
					case 12:
						Singleton<StageController>.Instance.GetStageModel().SetCurrentMapInfo(-1);
						SingletonBehavior<BattleSceneRoot>.Instance.ChangeToSephirahMap(this.currentFloor, true);
						SingletonBehavior<BattleSoundManager>.Instance.ChangeToFinalFinalBinahTheme(0);
						SingletonBehavior<BattleSoundManager>.Instance.SetEnemyTheme(new AudioClip[]{SingletonBehavior<BattleSoundManager>.Instance.CurrentPlayingTheme.clip});
						Singleton<StageController>.Instance._stageType = StageType.Creature;
						break;
					case 11:
						//TODO Add better redundancy logic
						// if (SingletonBehavior<BattleSoundManager>.Instance.CurrentPlayingTheme.clip == (bsTheme ?? false)) {
							SingletonBehavior<BattleSoundManager>.Instance.CurrentPlayingTheme.loop = true;
						// }
						Singleton<StageController>.Instance.GetStageModel().SetCurrentMapInfo(5);
						break;
					case 10:
						Singleton<StageController>.Instance.GetStageModel().SetCurrentMapInfo(4);
						SingletonBehavior<BattleSceneRoot>.Instance.ChangeToSpecialMap(Singleton<StageController>.Instance.GetStageModel().GetCurrentMapInfo(), true, true);
						SingletonBehavior<BattleSoundManager>.Instance.ChangeToBlackSilenceTheme();
						bsTheme = SingletonBehavior<BattleSoundManager>.Instance.CurrentPlayingTheme.clip;
						var bsThemeArray = new AudioClip[]{bsTheme};
						SingletonBehavior<BattleSoundManager>.Instance.SetEnemyTheme(bsThemeArray);
						SingletonBehavior<BattleSceneRoot>.Instance.currentMapObject.mapBgm = bsThemeArray;
						MDH.InitDialog(BSH.dlgIdList, Color.black);
						MDH.InitDialog(BSH.dlgIdList2, new Color(0.33f, 0f, 0f));
						MDH.InitDialog(BSH.dlgIdList3, Color.red);
						// curPhase = Phase.FOURTH; // This is to trick hardcoded logic.
						Singleton<StageController>.Instance._stageType = StageType.Creature;
						break;
					case 9:
						Singleton<StageController>.Instance.GetStageModel().SetCurrentMapInfo(-1);
						SingletonBehavior<BattleSceneRoot>.Instance.ChangeToSephirahMap(this.currentFloor, true);
						SingletonBehavior<BattleSoundManager>.Instance.SetEnemyTheme(new AudioClip[]{Resources.Load<AudioClip>("Sounds/Battle/Reverberation1st_Argalia")});
						break;
					case 8:
						if (mapCache != null) {
							mapCache.GetType().GetField("_bInit", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(mapCache, false);
							mapCache = null;
						}
						goto default;
					case 7:
						Singleton<StageController>.Instance.GetStageModel().SetCurrentMapInfo(2);
						SingletonBehavior<BattleSceneRoot>.Instance.ChangeToSpecialMap(Singleton<StageController>.Instance.GetStageModel().GetCurrentMapInfo(), true, true);
						MDH.InitDialog(MDH.yanDlgIdList, new Color32(134, 231, 255, 255));
						break;
					case 6:
						#if !DEBUG_PhaseSkip
						mapCache.GetType().GetField("_dlgIdList", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(mapCache, new List<string>());
						mapCache = null;
						#endif
						goto default;
					case 5:
						Singleton<StageController>.Instance.GetStageModel().SetCurrentMapInfo(1);
						SingletonBehavior<BattleSceneRoot>.Instance.ChangeToSpecialMap(Singleton<StageController>.Instance.GetStageModel().GetCurrentMapInfo(), true, true);
						mapCache = SingletonBehavior<BattleSceneRoot>.Instance.currentMapObject;
						PsuedoCry = new CryingChildrenHandler.PsuedoCryingManager(this);
						mapCache.GetType().GetField("_stageManager", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(mapCache, PsuedoCry);
						MDH.InitDialog(CCH.dlgIdList, new Color(0.8f, 0f, 0f));
						break;
					case 4:
						Singleton<StageController>.Instance.GetStageModel().SetCurrentMapInfo(0);
						break;
					default:
						ThemeEnforce(false);
						Singleton<StageController>.Instance.GetStageModel().SetCurrentMapInfo(-1);
						break;
				}
			} else {
				switch (phase) {
					case 7:
						if (specialUnits["Xiao"].IsDead()) {
							MDH.ClearDialog(MDH.yanDlgIdList);
							if (!specialUnits["Yan"].IsDead()) {
								Singleton<StageController>.Instance.GetStageModel().SetCurrentMapInfo(3);
								SingletonBehavior<BattleSceneRoot>.Instance.ChangeToSpecialMap(Singleton<StageController>.Instance.GetStageModel().GetCurrentMapInfo(), true, true);
								mapCache = SingletonBehavior<BattleSceneRoot>.Instance.currentMapObject;
							} else {
								Singleton<StageController>.Instance.GetStageModel().SetCurrentMapInfo(-1);
								if (mapCache != null) {
									mapCache.GetType().GetField("_bInit", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(mapCache, false);
									mapCache = null;
								}
								ThemeEnforce(false);
							}
						}
						if (specialUnits["Yan"].IsDead()) {
							MDH.ClearDialog(MDH.yanDlgIdList);
						}
						break;
					case 5:
						if (!PsuedoCryChanged && specialUnits["Crying1"].IsDead() && specialUnits["Crying2"].IsDead() && specialUnits["Crying3"].IsDead()) {
							PsuedoCry.ChangeToBasic();
							MDH.ClearDialog(CCH.dlgIdList);
							MDH.InitDialog(CCH.dlgIdList, Color.black);
							ThemeEnforce(true);
							PsuedoCryChanged = true;
							break;
						} else {
							goto default;
						}
					default:
						ThemeEnforce();
						break;
				}
			}
			lastPhase = phase;
		}
		public int lastPhase;
		public MapManager mapCache;
		public CryingChildrenHandler.PsuedoCryingManager PsuedoCry;
		public bool PsuedoCryChanged;
		public void ThemeEnforce() => ThemeEnforce(ThemeToggle);
		public void ThemeEnforce(bool special) {
			int emotionFloorTotalCoinNumber = Singleton<StageController>.Instance.GetCurrentStageFloorModel().team.emotionTotalCoinNumber;
			int emotionWaveTotalCoinNumber = Singleton<StageController>.Instance.GetCurrentWaveModel().team.emotionTotalCoinNumberWithBonus;
			if (special) {
				Singleton<StageController>.Instance.GetCurrentStageFloorModel().team.emotionTotalBonus = 0;
				Singleton<StageController>.Instance.GetCurrentWaveModel().team.emotionTotalBonus = emotionFloorTotalCoinNumber + 1;
			} else {
				Singleton<StageController>.Instance.GetCurrentStageFloorModel().team.emotionTotalBonus = emotionWaveTotalCoinNumber + 999;
			}
			ThemeToggle = special;
		}
		public bool ThemeToggle;
		
		#if DEBUG_ManualMap
		#warning Compiled with ManualMap debug option
		#pragma warning disable
		public class ManualMapTool {
			public void SetMap() => Singleton<StageController>.Instance.GetStageModel().SetCurrentMapInfo(mapNumber);
			public void CheckMapChange() => Singleton<StageController>.Instance.CheckMapChange();
			public string GetCurrentMapInfo { get => Singleton<StageController>.Instance.GetStageModel().GetCurrentMapInfo(); }
			public int mapNumber = -1;
		}
		ManualMapTool ManualMap = new ManualMapTool();
		#pragma warning restore
		#endif

		public override void OnRoundStart_After()
		{
			if (this.phase == 12)
			{
				List<BattleUnitModel> aliveList = BattleObjectManager.instance.GetAliveList(Faction.Player);
				using (List<BattleUnitModel>.Enumerator enumerator = BattleObjectManager.instance.GetAliveList(Faction.Enemy).GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						BattleUnitModel battleUnitModel2 = enumerator.Current;
						if (!aliveList.Exists((BattleUnitModel x) => x.IsTargetable(battleUnitModel2)))
						{
							foreach (BattleUnitModel battleUnitModel in aliveList)
							{
								battleUnitModel.bufListDetail.AddBuf(new PassiveAbility_1306012.BattleUnitBuf_nullfyNotTargetable());
								battleUnitModel.bufListDetail.AddKeywordBufThisRoundByEtc(KeywordBuf.Protection, 2, actor: null);
								battleUnitModel.bufListDetail.AddKeywordBufThisRoundByEtc(KeywordBuf.BreakProtection, 2, actor: null);
							}
						}
					}
				}
			}
		}

		public override void OnAllEnemyEquipCard() {
			if (phase == 5) {
				CCH.ChildDiceEffect();
			}
		}

		#if !DEBUG_CryingChildren
		public override bool HideEnemyTarget()
		{
			if (this.phase == 5) {
				foreach (BattleUnitModel battleUnitModel in BattleObjectManager.instance.GetAliveList(Faction.Enemy))
				{
					PassiveAbilityBase passiveAbilityBase = battleUnitModel.passiveDetail.PassiveList.Find((PassiveAbilityBase x) => x is PassiveAbility_240428);
					if (passiveAbilityBase != null && !passiveAbilityBase.destroyed)
					{
						return true;
					}
				}
			}
			return false;
		}
		#endif
		public override bool BlockEnemyAggroChange()
		{
			if (this.phase == 5) {
				foreach (BattleUnitModel battleUnitModel in BattleObjectManager.instance.GetAliveList(Faction.Enemy))
				{
					PassiveAbilityBase passiveAbilityBase = battleUnitModel.passiveDetail.PassiveList.Find((PassiveAbilityBase x) => x is PassiveAbility_240428);
					if (passiveAbilityBase != null && !passiveAbilityBase.destroyed)
					{
						return true;
					}
				}
			}
			return false;
		}
		#if !DEBUG_CryingChildren
		public override bool IsHideDiceAbilityInfo()
		{
			if (this.phase == 5) {
				foreach (BattleUnitModel battleUnitModel in BattleObjectManager.instance.GetAliveList(Faction.Enemy))
				{
					PassiveAbilityBase passiveAbilityBase = battleUnitModel.passiveDetail.PassiveList.Find((PassiveAbilityBase x) => x is PassiveAbility_240328);
					if (passiveAbilityBase != null && !passiveAbilityBase.destroyed)
					{
						return true;
					}
				}
			}
			return false;
		}
		#endif

		public readonly CryingChildrenHandler CCH = new CryingChildrenHandler();
		// public static implicit operator EnemyTeamStageManager_TheCrying(EnemyTeamStageManager_UltimaAgain m) => m.PsuedoCry;
		public readonly BlackSilenceHandler BSH = new BlackSilenceHandler();
		// public static implicit operator EnemyTeamStageManager_BlackSilence(EnemyTeamStageManager_UltimaAgain m) => m.BSH.PsuedoManager;
		public readonly MultiDialougeHandler MDH = new MultiDialougeHandler();
		public readonly EnemyTeamStageManager_FinalFinal FFH = new EnemyTeamStageManager_FinalFinal {
			_currentPhase = EnemyTeamStageManager_FinalFinal.FinalPhase.BinahEnterBattle
		};
		// public static implicit operator EnemyTeamStageManager_FinalFinal(EnemyTeamStageManager_UltimaAgain m) => m.FFH;
		public bool FFH_Active = false;

		public List<int> GetPhaseGuest(int phase)
		{
			List<int> result;
			switch (phase)
			{
			case 1:
				result = new List<int>
				{
					1,
					2,
					3,
					4,
					5,
					6,
					7,
					8,
					9,
					10,
					11,
					12,
					1001,
					1002,
					1003,
					1004,
				};
				break;
			case 2:
				result = new List<int>
				{
					10001,
					10002,
					10003,
					10004,
					10005,
					10006,
					11001,
					11002,
					11003,
					11004,
					11005,
					11006,
				};
				break;
			case 3:
				result = new List<int>
				{
					20001,
					20002,
					20003,
					20004,
					20005,
					20006,
					20007,
					20008,
					20009,
					20010,
					21001,
					21002,
					21003,
					21004,
					21005,
					21006,
					21007,
					21008,
					21009,
					21010,
					21011,
					21012,
					21013,
				};
				break;
			case 4:
				result = new List<int>
				{
					30001,
					30002,
					30003,
					30004,
					30005,
					30006,
					30007,
					30008,
					30009,
					30010,
					30011,
					30012,
					30013,
					30014,
					30015,
					30016,
					30018,
					30019,
					30020,
					30021,
					30022,
					30023,
					30024,
					30025,
					30026,
					30027,
					30028,
					31001,
					31002,
					31003,
					31004,
				};
				break;
			case 5:
				result = new List<int>
				{
					40001,
					40002,
					40003,
					40004,
					40005,
					40006,
					40007,
					40008,
					40011,
					40012,
					40013,
					40015,
					40016,
					40017,
					40018,
					40019,
					40020,
					40021,
					40022,
					40023,
					40024,
					40025,
					40026,
					42001,
					42002,
					42003,
					42004,
					42101,
					42005,
					42006,
					42007,
					42201,
					42008,
					41001,
					41002,
					43001,
					43002,
					43003,
					43004,
				};
				break;
			case 6:
				result = new List<int>
				{
					50001,
					50002,
					50003,
					50004,
					50005,
					50006,
					50007,
					50008,
					50009,
					50105,
					50010,
					50011,
					50012,
					50013,
					50015,
					50016,
					50017,
					51001,
					51002,
					50018,
					50019,
					50020,
					50021,
					50023,
					50024,
					50025,
					50026,
					50101,
					50102,
					50103,
					43005,
				};
				break;
			case 7:
				result = new List<int>
				{
					50022,
					50031,
					50032,
					50033,
					50034,
					50101,
					50102,
					50103,
					50035,
					50036,
					50037,
					50038,
					50051,
					50039,
					50040,
				};
				break;
			case 8:
				result = new List<int>
				{
					60001,
					60101,
					60002,
					60003,
					60004,
				};
				break;
			case 9:
				result = new List<int>
				{
					1301011,
					1301021,
					1302011,
					1302021,
					1303011,
					1303021,
					1304011,
					1304021,
					1304031,
					1305011,
					1305021,
					1305031,
					1307011,
					1307021,
					1307031,
					1307041,
					1307051,
					1308011,
					1308021,
					1306011,
					1310011,
				};
				if (!FinnalConfig.Instance.PlutoOff) {
					result.Add(1309011);
					result.Add(1309021);
				}
				break;
			case 10:
				result = new List<int>
				{
					60005,
					60006,
					60007,
					60107,
				};
				break;
			case 11:
				result = new List<int>
				{
					1408011,
					1410011,
					1409011,
					1409021,
					1405011,
					1405021,
					1405031,
					1405041,
					1407011,
					1406011,
					1403011,
					1404011,
					1401011,
					1402011,
				};
				break;
			case 12:
				result = new List<int>
				{
					80001,
					80002,
				};
				break;
			default:
				result = null;
				break;
			}
			return result;
		}

		public void CheckPhaseFinnal()
		{
			if (BattleObjectManager.instance.GetAliveList(Faction.Enemy).Count <= 0)
			{
				// We're between phases, so clean up if it hasn't already been done.
				if (phase >= 7) {
					CleanUp();
				}
				phase++;
				Debug.Log($"Finnal: Starting Phase Transition, new phase is {phase}");
				if (phase <= 12)
				{
					foreach (BattleUnitModel battleUnitModel in BattleObjectManager.instance.GetAliveList(Faction.Player))
					{
						battleUnitModel.RecoverHP(20);
						battleUnitModel.breakDetail.RecoverBreak(20);
						battleUnitModel.RecoverBreakLife(1);
						battleUnitModel.cardSlotDetail.RecoverPlayPoint(4);
						battleUnitModel.allyCardDetail.DrawCards(2);
					}
					int index = 0;
					bool loop = false;
					foreach (int num in GetPhaseGuest(this.phase))
					{
						switch (num) {
							case 42001:
								Singleton<StageController>.Instance.AddNewUnit(Faction.Enemy, num, index, 330);
								break;
							case 42101:
								Singleton<StageController>.Instance.AddNewUnit(Faction.Enemy, num, index, 270);
								break;
							case 42201:
								Singleton<StageController>.Instance.AddNewUnit(Faction.Enemy, num, index, 210);
								break;
							default:
						 		Singleton<StageController>.Instance.AddNewUnit(Faction.Enemy, num, index, -1);
								break;
						}
						BattleUnitModel battleUnitModel = BattleObjectManager.instance.GetUnitWithIndex(Faction.Enemy, index);
						PassiveReplacer(battleUnitModel);
						battleUnitModel.passiveDetail.OnCreated();
						battleUnitModel.passiveDetail.OnUnitCreated();
						battleUnitModel.passiveDetail.OnWaveStart();
						switch (num) {
							case 42001:
								specialUnits["Crying1"] = battleUnitModel;
								break;
							case 42101:
								specialUnits["Crying2"] = battleUnitModel;
								battleUnitModel.view.ChangeSkin("TheCryingChildren_Damaged_1");
								battleUnitModel.LoseHp(60);
								break;
							case 42201:
								specialUnits["Crying3"] = battleUnitModel;
								battleUnitModel.view.ChangeSkin("TheCryingChildren_Damaged_2");
								battleUnitModel.LoseHp(60);
								break;
							case 50035:
								specialUnits["pt"] = battleUnitModel;
								break;
							case 42005:
							case 42006:
							case 42007:
								CCH.CryingChildrenEdit(battleUnitModel, false);
								break;
							case 42008:
								CCH.CryingChildrenEdit(battleUnitModel, true);
								break;
							case 50038:
								specialUnits["Xiao"] = battleUnitModel;
								break;
							case 50051:
								specialUnits["Yan"] = battleUnitModel;
								break;
							case 80001:
							case 80002:
								FFH_Active = true;
								FFH._finalTurnCount = 0;
								break;
							default:
								break;
						}
						battleUnitModel.emotionDetail.SetEmotionLevel(Mathf.Min(this.phase + 1, 5));
						battleUnitModel.cardSlotDetail.RecoverPlayPoint(5);
						battleUnitModel.allyCardDetail.DrawCards(4);
						if (FinnalConfig.HarmonyMode != 2) {
							if (index == 4 && !loop) {
								Debug.Log("Finnal: Hit capacity, starting alternative fill method.");
								loop = true;
								BattleUnitModel battleUnitModel2 = BattleObjectManager.instance.GetUnitWithIndex(Faction.Enemy, 1);
								battleUnitModel2.index = 0;
								index = 1;
							} else if (!loop) {
								index++;
							} else {
								battleUnitModel.index = 0;
							}
							battleUnitModel.formation = new FormationPosition(battleUnitModel.formation._xmlInfo);
						} else {
							index++;
						}
					}
					PosShuffle();
					int i = 0;
					foreach (BattleUnitModel battleUnitModel in BattleObjectManager.instance.GetList(Faction.Enemy))
					{
						battleUnitModel.view.ChangeScale(MapSize.L);
						if (FinnalConfig.HarmonyMode != 2) {
							if (i <= 4) {
								battleUnitModel.index = i;
								SingletonBehavior<UICharacterRenderer>.Instance.SetCharacter(battleUnitModel.UnitData.unitData, (i+5), forcelyReload: true);
								i++;
							} else {
								battleUnitModel.index = 0;
								SingletonBehavior<UICharacterRenderer>.Instance.SetCharacter(battleUnitModel.UnitData.unitData, (5), forcelyReload: true);
							}
						} else {
							SingletonBehavior<UICharacterRenderer>.Instance.SetCharacter(battleUnitModel.UnitData.unitData, battleUnitModel.index+8, forcelyReload: true);
						}
						battleUnitModel.moveDetail.ReturnToFormationByBlink(ignoreView: true);
					}
					// We refresh the UI after the registrations are all done, in a try loop due to an error.
					try {
						BattleObjectManager.instance.InitUI();
					} catch (IndexOutOfRangeException) {}
				}
				else
				{
					if (this.phase == 13)
					{
						BattleObjectManager.instance.RegisterUnit(Pt);
						Pt.view.EnableView(false);
					}
					if (this.phase > 13)
					{
						finished = true;
					}
				}
			}
		}

		public void CheckFloorFinnal()
		{
			if (BattleObjectManager.instance.GetAliveList(Faction.Player).Count <= 0)
			{
				Debug.Log($"Finnal: Starting Floor Transition, changing from {currentFloor} to {remains[0].Sephirah}");
				Debug.Log("Finnal: Cleaning current floor...");
				foreach (BattleUnitModel battleUnitModel in BattleObjectManager.instance.GetList(Faction.Player))
				{
					BattleObjectManager.instance.UnregisterUnit(battleUnitModel);
					// Debug.LogError("Finnal: Unregistered Librarian " + battleUnitModel.id);
				}
				Debug.Log("Finnal: Setting up floor...");
				var battleSceneRoot = SingletonBehavior<BattleSceneRoot>.Instance;
				bool sephirahMapActive = battleSceneRoot.mapList.Contains(battleSceneRoot.currentMapObject);
				if (remains.Count > 1)
				{
					MapChangeStart();
					/*
					if (sephirahMapActive) {
						MapChangeStart();
					}
					*/
					Singleton<StageController>.Instance.SetCurrentSephirah(remains[0].Sephirah);
					StageLibraryFloorModel currentStageFloorModel = Singleton<StageController>.Instance.GetCurrentStageFloorModel();
					Singleton<StageController>.Instance.GetStageModel().GetFloor(currentFloor).Defeat();
					// Debug.LogError("Finnal: currentStageFloorModel.GetUnitBattleDataList includes:");
					for (int i = 0; i < currentStageFloorModel.GetUnitBattleDataList().Count; i++)
					{
						// Debug.LogError("Finnal: Count Index: " + i);
						BattleUnitModel battleUnitModel = Singleton<StageController>.Instance.CreateLibrarianUnit_fromBattleUnitData(i);
						// Debug.LogError("Finnal: CreateLibrarianUnit: " + battleUnitModel.id);
						battleUnitModel.OnWaveStart();
						battleUnitModel.emotionDetail.SetEmotionLevel(Mathf.Min(phase + 1, 5));
						battleUnitModel.cardSlotDetail.RecoverPlayPoint(5);
						battleUnitModel.allyCardDetail.DrawCards(6);
						SingletonBehavior<UICharacterRenderer>.Instance.SetCharacter(battleUnitModel.UnitData.unitData, i, forcelyReload: true);
						battleUnitModel.view.ChangeScale(MapSize.L);
					}
					// MapChange needs to be called before remains is updated
					MapChange(sephirahMapActive);
					remains.Remove(remains[0]);
				}
				else
				{
					if (!angelaappears)
					{
						angelaappears = true;
						MapChangeStart();
						/*
						if (sephirahMapActive) {
							MapChangeStart();
						}
						*/
						Singleton<StageController>.Instance.SetCurrentSephirah(SephirahType.Keter);
						StageLibraryFloorModel currentStageFloorModel2 = Singleton<StageController>.Instance.GetCurrentStageFloorModel();
						UnitDataModel unitDataModel = new UnitDataModel(9100501, SephirahType.Keter, isSephirahChar: true);
						BattleUnitModel battleUnitModel2 = BattleObjectManager.CreateDefaultUnit(Faction.Player);
						battleUnitModel2.index = 0;
						battleUnitModel2.grade = unitDataModel.grade;
						battleUnitModel2.formation = currentStageFloorModel2.GetFormationPosition(battleUnitModel2.index);
						UnitBattleDataModel unitBattleDataModel = new UnitBattleDataModel(Singleton<StageController>.Instance.GetStageModel(), unitDataModel);
						unitBattleDataModel.Init();
						battleUnitModel2.SetUnitData(unitBattleDataModel);
						battleUnitModel2.OnCreated();
						BattleObjectManager.instance.RegisterUnit(battleUnitModel2);
						battleUnitModel2.passiveDetail.OnUnitCreated();
						battleUnitModel2.passiveDetail.OnWaveStart();
						battleUnitModel2.emotionDetail.SetEmotionLevel(5);
						battleUnitModel2.cardSlotDetail.RecoverPlayPoint(5);
						battleUnitModel2.allyCardDetail.DrawCards(4);
						SingletonBehavior<UICharacterRenderer>.Instance.SetCharacter(battleUnitModel2.UnitData.unitData, 0, forcelyReload: true);
						battleUnitModel2.view.ChangeScale(battleSceneRoot.currentMapObject.mapSize);
						MapChange(sephirahMapActive);
					}
				}
				// Refresh UI after floor setup is complete, in a try loop due to an error.
				try {
					BattleObjectManager.instance.InitUI();
				} catch (IndexOutOfRangeException) {}
				Debug.Log("Finnal: Floor Setup Complete");
			}
			if (angelaappears)
			{
				BattleUnitModel battleUnitModel3 = BattleObjectManager.instance.GetAliveList(Faction.Player).Find((BattleUnitModel x) => x.Book.GetBookClassInfoId() == 9100501);
				foreach (BattleDiceCardModel battleDiceCardModel in battleUnitModel3.personalEgoDetail.GetHand())
				{
					battleUnitModel3.personalEgoDetail.RemoveCard(battleDiceCardModel.GetID());
				}
				if (phase >= 9)
				{
					battleUnitModel3.personalEgoDetail.AddCard(9910020);
					battleUnitModel3.personalEgoDetail.AddCard(9910011);
					battleUnitModel3.personalEgoDetail.AddCard(9910012);
					battleUnitModel3.personalEgoDetail.AddCard(9910013);
					battleUnitModel3.personalEgoDetail.AddCard(9910014);
				}
				if (phase >= 10)
				{
					battleUnitModel3.personalEgoDetail.AddCard(9910015);
					battleUnitModel3.personalEgoDetail.AddCard(9910016);
					battleUnitModel3.personalEgoDetail.AddCard(9910017);
				}
				if (phase >= 11)
				{
					battleUnitModel3.personalEgoDetail.AddCard(9910018);
					battleUnitModel3.personalEgoDetail.AddCard(9910019);
				}
				if (phase >= 12)
				{
					if (!battleUnitModel3.bufListDetail.GetActivatedBufList().Exists((BattleUnitBuf x) => x is BattleUnitBuf_KeterFinal_Cogito))
					{
						battleUnitModel3.bufListDetail.AddBuf(new BattleUnitBuf_KeterFinal_Cogito());
					}
				}
				if (phase >= 13)
				{
					battleUnitModel3.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_KeterFinal_Cogito).Destroy();
				}
			}
		}
		public void CleanUp(bool psuedo = false)
		{
			Debug.Log("Finnal: Cleaning dead enemies...");
			int i = 0;
			if (psuedo) {
				if (FinnalConfig.HarmonyMode != 2) {
					Debug.Log("Finnal: Psuedo clean, skipping unregistration");
				} else {
					Debug.Log("Finnal: Psuedo clean, SummonLiberation active and thusly doing nothing");
					return;
				}
			}
			foreach (BattleUnitModel battleUnitModel in BattleObjectManager.instance.GetList(Faction.Enemy))
			{
				if (battleUnitModel.IsDead())
				{
					if (!psuedo) {
						BattleObjectManager.instance.UnregisterUnit(battleUnitModel);
						// Debug.LogError("Finnal: Unregistered Enemy: " + battleUnitModel.id);
					} else {
						if (FinnalConfig.HarmonyMode != 2) {
							battleUnitModel.index = 4;
						}
					}
				} else if (FinnalConfig.HarmonyMode != 2) {
					if (i < 4 || !psuedo && i == 4) {
						battleUnitModel.index = i;
						SingletonBehavior<UICharacterRenderer>.Instance.SetCharacter(battleUnitModel.UnitData.unitData, (i+5), forcelyReload: true);
						i++;
					} else {
						battleUnitModel.index = 0;
						SingletonBehavior<UICharacterRenderer>.Instance.SetCharacter(battleUnitModel.UnitData.unitData, (5), forcelyReload: true);
					}
				}
			}
			this.PosShuffle();
			// We refresh the UI after the registrations are all done
			SingletonBehavior<HexagonalMapManager>.Instance.ResetMapSetting();
			SingletonBehavior<HexagonalMapManager>.Instance.OnRoundStart();
			SingletonBehavior<BattleCamManager>.Instance.ResetCamSetting();
			// In a try catch due to an error.
			try {
				BattleObjectManager.instance.InitUI();
			} catch (IndexOutOfRangeException) {}
			Debug.Log("Finnal: Cleaning Finished");
		}
		public void MapChangeStart() {
			SingletonBehavior<BattleSceneRoot>.Instance._mapChangeFilter.StartMapChangingEffect(Direction.RIGHT, true);
			// List<MapManager> list = SingletonBehavior<BattleSceneRoot>.Instance.mapList;
			// MapManager x2 = (list != null) ? list.Find((MapManager x) => x.sephirahType == this.currentFloor) : null;
			// if (x2 == SingletonBehavior<BattleSceneRoot>.Instance.currentMapObject) {
			// 		try {
			// 			MapChangeFilter mapChangeFilter = new MapChangeFilter();
			// 			mapChangeFilter.StartMapChangingEffect(Direction.RIGHT, true);
			// 		} catch { Debug.LogError("MapChangeEffectError"); }
			// }
			// SingletonBehavior<BattleSceneRoot>.Instance.ChangeToSephirahMap(this.currentFloor, true);
			// Singleton<StageController>.Instance.CheckMapChange();
		}
		public void MapChange(bool active)
		{
			//If the floor is angela, make it Keter, otherwise base on this.remains
			if (!angelaappears) {
				this.currentFloor = remains[0].Sephirah;
			} else {
				this.currentFloor = SephirahType.Keter;
			}
			Debug.Log($"Finnal: Changing map, new map is {currentFloor} and is {(active ? "active" : "inactive")}");
			// Emulate map related init functions.
			var battleSceneRoot = SingletonBehavior<BattleSceneRoot>.Instance;
			foreach (MapManager manager in battleSceneRoot.mapList) {
				manager.ResetMap();
			}
			var currentMap = battleSceneRoot.currentMapObject;
			battleSceneRoot.ClearFloorMap();
			battleSceneRoot.HideAllFloorMap();
			battleSceneRoot.InitFloorMap(currentFloor);
			Singleton<StageController>.Instance.GetStageModel().GetFloor(currentFloor).SetEmotionTeamUnit();
			// SingletonBehavior<BattleCamManager>.Instance.ResetCamSetting();
			// SingletonBehavior<BattleSoundManager>.Instance.CheckTheme();
			if (!active) {
				battleSceneRoot.currentMapObject.EnableMap(false);
				battleSceneRoot.currentMapObject = currentMap;
			}
		}
		public void PosShuffle()
		{
			// Debug.LogError("Finnal: PosShuffle: Starting");
			var unitList = BattleObjectManager.instance.GetAliveList(Faction.Enemy);
			int maxPoints = unitList.Count;
			if (FinnalConfig.Instance.ScatterMode || gridUnsupportedPhase.Contains(this.phase)) {
				if (!FinnalConfig.Instance.ScatterMode) {
					Debug.Log("Finnal: PosShuffle: Gridmode is not currently supported for this phase");
				}
				Debug.Log("Finnal: PosShuffle: Using Scattermode");
				int current = 0;
				int loopCounter = 0;
				int maxIterations = 65536 * maxPoints;
				var minClosestDistance = 16;
				int[] x = new int[maxPoints];
				int[] y = new int[maxPoints];
				while (current < maxPoints && loopCounter < maxIterations) {
					int xPossible = RandomUtil.Range(1, 26);
					int yPossible = RandomUtil.Range(-12, 12);
					if (current == 0) {
						x[current] = xPossible;
						y[current] = yPossible;
						current++;
						continue;
					}
					// float[] result1 = new float[current];
					// float[] result2 = new float[current];
					float[] distances = new float[current];
					for (int i = 0; i < current; i++) {
						distances[i] = Mathf.Sqrt(Mathf.Pow(x[i]-xPossible, 2) + Mathf.Pow(y[i] - yPossible, 2));
					}
					// Debug.LogError("Finnal: PosShuffle: "+current+"-min distance: "+distances.Min());
					if (distances.Min() >= minClosestDistance) {
						x[current] = xPossible;
						y[current] = yPossible;
						current++;
					}
					loopCounter++;
					if (new[] {8192, 16384, 32768}.Contains(loopCounter)) {
						minClosestDistance /= 2;
						Debug.Log($"{current}: Too many loops, dropping max distance to {minClosestDistance}");
					}
				}
				Debug.Log($"Finnal: PosShuffle: Found {current} points in {loopCounter} tries");
				if (current != maxPoints) {
					Debug.Log($"Finnal: PosShuffle: Filling in {maxPoints - current} out of {maxPoints} entries");
					while (current < maxPoints) {
						x[current] = RandomUtil.Range(1, 26);
						y[current] = RandomUtil.Range(-12, 12);
						current++;
					}
				}
				current = 0;
				foreach (BattleUnitModel battleUnitModel in unitList) {
					var newPos = new Vector2Int(x[current], y[current]);
					battleUnitModel.formation.ChangePos(newPos);
					// Debug.LogError(current+": "+newPos);
					current++;
				}
			} else {
				Debug.Log("Finnal: PosShuffle: Using Gridmode");
				if (maxPoints <= 1) {
					foreach (BattleUnitModel battleUnitModel in unitList) {
						battleUnitModel.formation.ChangePos(new Vector2Int(11, 0));
					}
					return;
				}
				// Debug.LogError(maxPoints);
				float x = 1;
				float y;
				float incrementx = 24/(Mathf.Sqrt(maxPoints));
				float incrementy;
				if (maxPoints == 2) {
					y = 0;
					incrementy = 0;
				} else {
					y = 12;
					incrementy = 24/(Mathf.Sqrt(maxPoints)-1);
				}
				// Debug.LogError(incrementx);
				// Debug.LogError(incrementy);
				Vector2Int[] newPos = new Vector2Int[maxPoints];
				int i;
				bool stepping = false;
				var incrementHalf = (incrementx/2);
				for (i = 0; i < maxPoints; i++) {
				//	Debug.LogError("x-"+i+": "+(x));
				//	Debug.LogError("x-int"+i+": "+((int)x+1));
				//	Debug.LogError("y-"+i+": "+(y));
				//	Debug.LogError("y-int"+i+": "+((int)y+12));
				//	Debug.LogError("");
					if (stepping) {
						newPos[i] = new Vector2Int((int)(x+incrementHalf), (int)y);
					} else {
						newPos[i] = new Vector2Int((int)x, (int)y);
					}
					x += incrementx;
					if (x >= 25) {
						x = 1;
						if (stepping) {
							stepping = false;
						} else {
							stepping = true;
						}
						y -= incrementy;
					}
				}
				i = 0;
				foreach (BattleUnitModel battleUnitModel in unitList) {
					battleUnitModel.formation.ChangePos(newPos[i]);
					i++;
				}
				Debug.Log($"Finnal: PosShuffle: Arranged {maxPoints} characters");
			}
		}
		public void PassiveReplacer(BattleUnitModel battleUnitModel) {
			foreach (var passive in battleUnitModel.passiveDetail.PassiveList) {
				if (passives.TryGetValue(passive.GetType(), out var replacement)) {
					try {
						if (replacement == typeof(PassiveCommandPlaceholder)) {
							Debug.Log($"Finnal: PassiveReplacer: Running special command for {passive}.");
							PassiveCommand(passive);
							continue;
						}
						battleUnitModel.passiveDetail.DestroyPassive(passive);
						if (replacement is null) {
							Debug.Log($"Finnal: PassiveReplacer: Removing {passive}.");
							continue;
						}
						Debug.Log($"Finnal: PassiveReplacer: Replacing {passive} with Finnal version.");
						var newPassive = battleUnitModel.passiveDetail.AddPassive(Activator.CreateInstance(replacement) as PassiveAbilityBase);
					} catch (Exception ex) {
						Debug.LogException(ex);
					}
				}
			}
		}
		private class PassiveCommandPlaceholder {}
		public void PassiveCommand(PassiveAbilityBase input) {
			switch (input) {
				case PassiveAbility_240528 _:
					passiveAbility_240528_list.Add(input);
					break;
				default:
					break;
			}
		}
		public readonly Dictionary<Type, Type> passives = new Dictionary<Type, Type>(){
			{typeof(PassiveAbility_170101), typeof(PassiveAbility_170101_Finnal)}, // The Hatching Dead
			{typeof(PassiveAbility_170201), typeof(PassiveAbility_170201_Finnal)},
			{typeof(PassiveAbility_170211), typeof(PassiveAbility_170211_Finnal)},
			{typeof(PassiveAbility_180001), typeof(PassiveAbility_180001_Finnal)}, // The Head
			{typeof(PassiveAbility_180002), typeof(PassiveAbility_180002_Finnal)}, // The Claw
			{typeof(PassiveAbility_250227), typeof(PassiveAbility_250227_Finnal)},
			{typeof(PassiveAbility_1410014), null},
			{typeof(PassiveAbility_240528), typeof(PassiveCommandPlaceholder)},
			{typeof(PassiveAbility_1300001), typeof(PassiveAbility_1300001_Finnal)},
			{typeof(PassiveAbility_240028), typeof(PassiveAbility_240028_Finnal)},

			// Map Override Passives
			{typeof(PassiveAbility_150036), typeof(PassiveAbility_150036_Finnal)},
			{typeof(PassiveAbility_150037), typeof(PassiveAbility_150037_Finnal)},
			{typeof(PassiveAbility_230013), null},
			{typeof(PassiveAbility_230124), typeof(PassiveAbility_230124_Finnal)},
			// More complicated Map Override Passives
			{typeof(PassiveAbility_150138), typeof(PassiveAbility_150138_Finnal)},
			{typeof(PassiveAbility_151139), typeof(PassiveAbility_151139_Finnal)},
			{typeof(PassiveAbility_230028), typeof(PassiveAbility_230028_Finnal)},

			// Passive replacements handled by settings
			// PassiveAbility_150238, XiaoNullSettingCheck()
		};
		public readonly List<PassiveAbilityBase> passiveAbility_240528_list = new List<PassiveAbilityBase>();

		public BattleUnitModel Pt { get => specialUnits["pt"]; set => specialUnits["pt"] = value; }
		public Dictionary<string, BattleUnitModel> specialUnits = new Dictionary<string, BattleUnitModel>();

		public int phase;
		public SephirahType currentFloor;

		public bool finished = false;

		public bool angelaappears;

		public readonly List<StageLibraryFloorModel> remains = new List<StageLibraryFloorModel>();
		public readonly int[] gridUnsupportedPhase = new[] {7, 9, 11};
	}
}
