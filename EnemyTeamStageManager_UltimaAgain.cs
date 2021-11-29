using System;
using System.Collections.Generic;
using System.Linq;
using UI;
using UnityEngine;

namespace FinallyBeyondTheTime
{
	public class EnemyTeamStageManager_UltimaAgain : EnemyTeamStageManager
	{
		public override void OnWaveStart()
		{
			#if DEBUG_PhaseSkip
			#warning Compiled with PhaseSkip debug option
			this.phase = 6;
			#else
			this.phase = 0;
			#endif
			this.currentFloor = Singleton<StageController>.Instance.GetCurrentStageFloorModel().Sephirah;
			Debug.Log("Finall: Initial floor is " + this.currentFloor);
			this._angelaappears = false;
			this.remains.Clear();
			foreach (LibraryFloorModel libraryFloorModel in LibraryModel.Instance.GetOpenedFloorList())
			{
				if (libraryFloorModel.GetUnitDataList().Count > 0)
				{
					StageLibraryFloorModel stageLibraryFloorModel = new StageLibraryFloorModel();
					stageLibraryFloorModel.Init(Singleton<StageController>.Instance.GetStageModel(), libraryFloorModel, isRebattle: false);
					if (stageLibraryFloorModel.Sephirah != this.currentFloor || this.currentFloor == SephirahType.Keter)
					{
						this.remains.Add(stageLibraryFloorModel);
					} else {
						Debug.Log("Finall: Floor list skipping over " + this.currentFloor);
					}
				}
			}
			if (this.currentFloor != SephirahType.Keter)
			{
				Debug.Log("Finall: Inserting Keter at top of floor list");
				this.remains.Insert(0, this.remains[this.remains.Count - 1]);
			}
			// finnalFormation = new FormationModel(Singleton<StageController>.Instance.GetCurrentWaveModel().GetFormation().XmlInfo);
		}

		public override void OnRoundEndTheLast()
		{
			this.CheckFloor();
			// Phases from T3 SotC onwards have more complicated mechanics and less characters, so we stop cleaning every round just to be sure
			if (this.phase < 7)
			{
				this.CleanUp();
			} else {
				this.CleanUp(psuedo: true);
			}
			this.CheckPhase();
			// If the harmony finalizer is not implemented, disable enemy targeting toggles to prevent a soft-lock
			if (FinnalConfig.HarmonyMode == 0) {
				SingletonBehavior<BattleManagerUI>.Instance.ui_emotionInfoBar.targetingToggle.SetToggle(0, false);
			}
		}

		public override bool IsStageFinishable()
		{
			return this._finished;
		}

		public override void OnRoundStart()
		{
			foreach (BattleUnitModel battleUnitModel in BattleObjectManager.instance.GetAliveList(Faction.Enemy))
			{
				PassiveAbilityBase passiveAbilityBase = battleUnitModel.passiveDetail.PassiveList.Find((PassiveAbilityBase x) => x is PassiveAbility_240528);
				if (passiveAbilityBase != null && !passiveAbilityBase.destroyed)
				{
					List<BattleUnitModel> aliveList = BattleObjectManager.instance.GetAliveList(Faction.Player);
					Debug.Log("Finall: ChildImmobilizeNerf = "+FinnalConfig.Instance.ChildImmobilizeNerf);
					if (aliveList.Count > 1 || !FinnalConfig.Instance.ChildImmobilizeNerf && aliveList.Count > 0)
					{
						RandomUtil.SelectOne<BattleUnitModel>(aliveList).bufListDetail.AddKeywordBufThisRoundByCard(KeywordBuf.Stun, 1, actor: null);
						break;
					}
					break;
				}
			}
		}

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

		public override bool HideEnemyTarget()
		{
			foreach (BattleUnitModel battleUnitModel in BattleObjectManager.instance.GetAliveList(Faction.Enemy))
			{
				PassiveAbilityBase passiveAbilityBase = battleUnitModel.passiveDetail.PassiveList.Find((PassiveAbilityBase x) => x is PassiveAbility_240428);
				if (passiveAbilityBase != null && !passiveAbilityBase.destroyed)
				{
					return true;
				}
			}
			return false;
		}

		public override bool BlockEnemyAggroChange()
		{
			foreach (BattleUnitModel battleUnitModel in BattleObjectManager.instance.GetAliveList(Faction.Enemy))
			{
				PassiveAbilityBase passiveAbilityBase = battleUnitModel.passiveDetail.PassiveList.Find((PassiveAbilityBase x) => x is PassiveAbility_240428);
				if (passiveAbilityBase != null && !passiveAbilityBase.destroyed)
				{
					return true;
				}
			}
			return false;
		}

		public override bool IsHideDiceAbilityInfo()
		{
			foreach (BattleUnitModel battleUnitModel in BattleObjectManager.instance.GetAliveList(Faction.Enemy))
			{
				PassiveAbilityBase passiveAbilityBase = battleUnitModel.passiveDetail.PassiveList.Find((PassiveAbilityBase x) => x is PassiveAbility_240328);
				if (passiveAbilityBase != null && !passiveAbilityBase.destroyed)
				{
					return true;
				}
			}
			return false;
		}

		private List<int> GetPhaseGuest(int phase)
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
					42005,
					42006,
					42007,
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

		private void CheckPhase()
		{
			if (BattleObjectManager.instance.GetAliveList(Faction.Enemy).Count <= 0)
			{
				// We're between phases, so clean up if it hasn't already been done.
				if (this.phase >= 7) {
					this.CleanUp();
				}
				this.phase++;
				Debug.Log("Finall: Starting Phase Transition, new phase is " + this.phase);
				if (this.phase <= 12)
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
					foreach (int num in this.GetPhaseGuest(this.phase))
					{
						// if (FinnalConfig.HarmonyMode != 2) {
						 	Singleton<StageController>.Instance.AddNewUnit(Faction.Enemy, num, index, -1);
						// } else {
						// 	SummonLiberation.Harmony_Patch.SummonUnit(Faction.Enemy, new LorId(num), null, index);
						// }
						BattleUnitModel battleUnitModel = BattleObjectManager.instance.GetUnitWithIndex(Faction.Enemy, index);
						PassiveReplacer(battleUnitModel);
						battleUnitModel.passiveDetail.OnUnitCreated();
						battleUnitModel.passiveDetail.OnWaveStart();
						battleUnitModel.emotionDetail.SetEmotionLevel(Mathf.Min(this.phase + 1, 5));
						battleUnitModel.cardSlotDetail.RecoverPlayPoint(5);
						battleUnitModel.allyCardDetail.DrawCards(4);
						if (num == 50035)
						{
							pt = battleUnitModel;
						}
						if (FinnalConfig.HarmonyMode != 2) {
							if (index == 4 && !loop) {
								Debug.Log("Finall: Hit capacity, starting alternative fill method.");
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
					this.PosShuffle();
					int i = 0;
					foreach (BattleUnitModel battleUnitModel in BattleObjectManager.instance.GetList(Faction.Enemy))
					{
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
						BattleObjectManager.instance.RegisterUnit(pt);
						pt.view.EnableView(false);
					}
					if (this.phase > 13)
					{
						this._finished = true;
					}
				}
			}
		}

		private void CheckFloor()
		{
			if (BattleObjectManager.instance.GetAliveList(Faction.Player).Count <= 0)
			{
				Debug.Log("Finall: Starting Floor Transition, changing from " + this.currentFloor + " to " + this.remains[0].Sephirah);
				Debug.Log("Finall: Cleaning current floor...");
				foreach (BattleUnitModel battleUnitModel in BattleObjectManager.instance.GetList(Faction.Player))
				{
					BattleObjectManager.instance.UnregisterUnit(battleUnitModel);
					// Debug.LogError("Finall: Unregistered Librarian " + battleUnitModel.id);
				}
				Debug.Log("Finall: Setting up floor...");
				if (this.remains.Count > 1)
				{
					this.MapChangeStart();
					Singleton<StageController>.Instance.SetCurrentSephirah(this.remains[0].Sephirah);
					StageLibraryFloorModel currentStageFloorModel = Singleton<StageController>.Instance.GetCurrentStageFloorModel();
					// Debug.LogError("Finall: currentStageFloorModel.GetUnitBattleDataList includes:");
					for (int i = 0; i < currentStageFloorModel.GetUnitBattleDataList().Count; i++)
					{
						// Debug.LogError("Finall: Count Index: " + i);
						BattleUnitModel battleUnitModel = Singleton<StageController>.Instance.CreateLibrarianUnit_fromBattleUnitData(i);
						// Debug.LogError("Finall: CreateLibrarianUnit: " + battleUnitModel.id);
						battleUnitModel.OnWaveStart();
						battleUnitModel.emotionDetail.SetEmotionLevel(Mathf.Min(this.phase + 1, 5));
						battleUnitModel.cardSlotDetail.RecoverPlayPoint(5);
						battleUnitModel.allyCardDetail.DrawCards(6);
						SingletonBehavior<UICharacterRenderer>.Instance.SetCharacter(battleUnitModel.UnitData.unitData, i, forcelyReload: true);
					}
					// MapChange needs to be called before remains is updated
					this.MapChange();
					this.remains.Remove(this.remains[0]);
				}
				else
				{
					if (!this._angelaappears)
					{
						this._angelaappears = true;
						this.MapChangeStart();
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
						this.MapChange();
					}
				}
				// Refresh UI after floor setup is complete, in a try loop due to an error.
				try {
					BattleObjectManager.instance.InitUI();
				} catch (IndexOutOfRangeException) {}
				Debug.Log("Finall: Floor Setup Complete");
			}
			bool angelaappears = this._angelaappears;
			if (angelaappears)
			{
				BattleUnitModel battleUnitModel3 = BattleObjectManager.instance.GetAliveList(Faction.Player).Find((BattleUnitModel x) => x.Book.GetBookClassInfoId() == 9100501);
				foreach (BattleDiceCardModel battleDiceCardModel in battleUnitModel3.personalEgoDetail.GetHand())
				{
					battleUnitModel3.personalEgoDetail.RemoveCard(battleDiceCardModel.GetID());
				}
				if (this.phase >= 9)
				{
					battleUnitModel3.personalEgoDetail.AddCard(9910020);
					battleUnitModel3.personalEgoDetail.AddCard(9910011);
					battleUnitModel3.personalEgoDetail.AddCard(9910012);
					battleUnitModel3.personalEgoDetail.AddCard(9910013);
					battleUnitModel3.personalEgoDetail.AddCard(9910014);
				}
				if (this.phase >= 10)
				{
					battleUnitModel3.personalEgoDetail.AddCard(9910015);
					battleUnitModel3.personalEgoDetail.AddCard(9910016);
					battleUnitModel3.personalEgoDetail.AddCard(9910017);
				}
				if (this.phase >= 11)
				{
					battleUnitModel3.personalEgoDetail.AddCard(9910018);
					battleUnitModel3.personalEgoDetail.AddCard(9910019);
				}
				if (this.phase >= 12)
				{
					if (!battleUnitModel3.bufListDetail.GetActivatedBufList().Exists((BattleUnitBuf x) => x is BattleUnitBuf_KeterFinal_Cogito))
					{
						battleUnitModel3.bufListDetail.AddBuf(new BattleUnitBuf_KeterFinal_Cogito());
					}
				}
				if (this.phase >= 13)
				{
					battleUnitModel3.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_KeterFinal_Cogito).Destroy();
				}
			}
		}
		private void CleanUp(bool psuedo = false)
		{
			Debug.Log("Finall: Cleaning dead enemies...");
			int i = 0;
			if (psuedo) {
				if (FinnalConfig.HarmonyMode != 2) {
					Debug.Log("Finall: Psuedo clean, skipping unregistration");
				} else {
					Debug.Log("Finall: Psuedo clean, SummonLiberation active and thusly doing nothing");
					return;
				}
			}
			foreach (BattleUnitModel battleUnitModel in BattleObjectManager.instance.GetList(Faction.Enemy))
			{
				if (battleUnitModel.IsDead())
				{
					if (!psuedo) {
						BattleObjectManager.instance.UnregisterUnit(battleUnitModel);
						// Debug.LogError("Finall: Unregistered Enemy: " + battleUnitModel.id);
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
			// Cleanup is called first, so we call it a phase "early"
			if (this.phase < 8) {
				int emotionTotalCoinNumber = Singleton<StageController>.Instance.GetCurrentWaveModel().team.emotionTotalCoinNumberWithBonus;
				Singleton<StageController>.Instance.GetCurrentStageFloorModel().team.emotionTotalBonus = emotionTotalCoinNumber + 999;
			} else {
				Singleton<StageController>.Instance.GetCurrentStageFloorModel().team.emotionTotalBonus = 0;
			}
			Singleton<StageController>.Instance.CheckMapChange();
			// We refresh the UI after the registrations are all done
			SingletonBehavior<HexagonalMapManager>.Instance.ResetMapSetting();
			SingletonBehavior<HexagonalMapManager>.Instance.OnRoundStart();
			SingletonBehavior<BattleCamManager>.Instance.ResetCamSetting();
			// In a try loop due to an error.
			try {
				BattleObjectManager.instance.InitUI();
			} catch (IndexOutOfRangeException) {}
			Debug.Log("Finall: Cleaning Finished");
		}
		private void MapChangeStart() {
			// List<MapManager> list = SingletonBehavior<BattleSceneRoot>.Instance.mapList;
			// MapManager x2 = (list != null) ? list.Find((MapManager x) => x.sephirahType == this.currentFloor) : null;
			// if (x2 == SingletonBehavior<BattleSceneRoot>.Instance.currentMapObject) {
			// 		try {
			// 			MapChangeFilter mapChangeFilter = new MapChangeFilter();
			// 			mapChangeFilter.StartMapChangingEffect(Direction.RIGHT, true);
			// 		} catch { Debug.LogError("MapChangeEffectError"); }
			// }
			// SingletonBehavior<BattleSceneRoot>.Instance.ChangeToSephirahMap(this.currentFloor, true);
			Singleton<StageController>.Instance.CheckMapChange();
		}
		private void MapChange()
		{
			//If the floor is angela, make it Keter, otherwise base on this.remains
			if (!this._angelaappears)
			{
				this.currentFloor = this.remains[0].Sephirah;
			} else {
				this.currentFloor = SephirahType.Keter;
			}
			Debug.Log("Finall: Changing map, new map is " + this.currentFloor);
			// Emulate map related init functions.
			SingletonBehavior<BattleSceneRoot>.Instance.HideAllFloorMap();
			SingletonBehavior<BattleSceneRoot>.Instance.InitFloorMap(this.currentFloor);
			Singleton<StageController>.Instance.GetStageModel().GetFloor(this.currentFloor).SetEmotionTeamUnit();
			SingletonBehavior<BattleCamManager>.Instance.ResetCamSetting();
			// SingletonBehavior<BattleSoundManager>.Instance.CheckTheme();
		}
		private void PosShuffle()
		{
			// Debug.LogError("Finall: PosShuffle: Starting");
			var unitList = BattleObjectManager.instance.GetAliveList(Faction.Enemy);
			int maxPoints = unitList.Count;
			if (FinnalConfig.Instance.ScatterMode || gridUnsupportedPhase.Contains(this.phase)) {
				if (!FinnalConfig.Instance.ScatterMode) {
					Debug.Log("Finall: PosShuffle: Gridmode is not currently supported for this phase");
				}
				Debug.Log("Finall: PosShuffle: Using Scattermode");
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
					// Debug.LogError("Finall: PosShuffle: "+current+"-min distance: "+distances.Min());
					if (distances.Min() >= minClosestDistance) {
						x[current] = xPossible;
						y[current] = yPossible;
						current++;
					}
					loopCounter++;
					if (new[] {8192, 16384, 32768}.Contains(loopCounter)) {
						minClosestDistance /= 2;
						Debug.Log(current+": Too many loops, dropping max distance to "+minClosestDistance);
					}
				}
				Debug.Log("Finall: PosShuffle: Found "+current+" points in "+loopCounter+" tries");
				if (current != maxPoints) {
					Debug.Log("Finall: PosShuffle: Filling in "+(maxPoints-current)+" out of "+maxPoints+" entries");
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
				Debug.Log("Finall: PosShuffle: Using Gridmode");
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
				Debug.Log("Finall: PosShuffle: Arranged "+maxPoints+" characters");
			}
		}
		private void PassiveReplacer(BattleUnitModel battleUnitModel) {
			foreach (var passive in battleUnitModel.passiveDetail.PassiveList) {
				if (passives.TryGetValue(passive.GetType(), out var replacement)) {
					try {
						battleUnitModel.passiveDetail.DestroyPassive(passive);
						if (replacement is null) {
							Debug.Log($"Finall: PassiveReplacer: Removing {passive}.");
							continue;
						}
						Debug.Log($"Finall: PassiveReplacer: Replacing {passive} with Finnal version.");
						var newPassive = battleUnitModel.passiveDetail.AddPassive(Activator.CreateInstance(replacement) as PassiveAbilityBase);
						newPassive.OnWaveStart();
					} catch (Exception ex) {
						Debug.LogException(ex);
					}
				}
			}
		}
		private readonly Dictionary<Type, Type> passives = new Dictionary<Type, Type>(){
            {typeof(PassiveAbility_170101), typeof(PassiveAbility_170101_Finnal)},
            {typeof(PassiveAbility_170201), typeof(PassiveAbility_170201_Finnal)},
            {typeof(PassiveAbility_170211), typeof(PassiveAbility_170211_Finnal)},
            {typeof(PassiveAbility_180001), typeof(PassiveAbility_180001_Finnal)},
            {typeof(PassiveAbility_180002), typeof(PassiveAbility_180002_Finnal)},
            {typeof(PassiveAbility_250227), typeof(PassiveAbility_250227_Finnal)},
            {typeof(PassiveAbility_1410014), null},
        };

		internal BattleUnitModel pt;

		private int phase;
		private SephirahType currentFloor;

		private bool _finished = false;

		private bool _angelaappears;

		private readonly List<StageLibraryFloorModel> remains = new List<StageLibraryFloorModel>();
        readonly int[] gridUnsupportedPhase = new[] {7, 9, 11};
	}
}
