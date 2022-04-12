using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UI;
using UnityEngine;
using LOR_DiceSystem;

namespace FinallyBeyondTheTime {
	public partial class EnemyTeamStageManager_UltimaAgain : EnemyTeamStageManager {
		public class CryingChildrenHandler {
			public class PsuedoCryingManager : EnemyTeamStageManager_TheCrying {
				public PsuedoCryingManager(EnemyTeamStageManager_UltimaAgain parent) {
					Parent = parent;
					GetType().BaseType.GetField("_currentPhase", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(this, Phase.OneUnitPhase);
				}
				internal void ChangeToBasic() {
					GetType().BaseType.GetField("_currentPhase", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(this, Phase.FiveUnitPhase);
				}
				public override void OnWaveStart() {}
				public override void OnRoundEndTheLast() {}
				public override bool IsStageFinishable() {return Parent.finished;}
				public override void OnRoundStart() {}
				public override bool HideEnemyTarget() {return Parent.HideEnemyTarget();}
				public override bool BlockEnemyAggroChange() {return Parent.BlockEnemyAggroChange();}
				public override bool IsHideDiceAbilityInfo() {return Parent.IsHideDiceAbilityInfo();}
				public override void OnAllEnemyEquipCard() {}
				readonly EnemyTeamStageManager_UltimaAgain Parent;
			}
			public void ChildDiceEffect()
			{
				if (_weak) {
					return;
				}
				childList = BattleObjectManager.instance.GetAliveList(Faction.Enemy).FindAll(model => childIDList.Contains(model.UnitData.unitData.EnemyUnitId.id));
				List<BattlePlayingCardDataInUnitModel> list = new List<BattlePlayingCardDataInUnitModel>();
				foreach (BattleUnitModel battleUnitModel in childList)
				{
					foreach (BattlePlayingCardDataInUnitModel battlePlayingCardDataInUnitModel in battleUnitModel.cardSlotDetail.cardAry)
					{
						if (battlePlayingCardDataInUnitModel != null)
						{
							battlePlayingCardDataInUnitModel.card.ResetToOriginalData();
							battlePlayingCardDataInUnitModel.card.CopySelf();
							battlePlayingCardDataInUnitModel.ResetCardQueue();
							list.Add(battlePlayingCardDataInUnitModel);
						}
					}
				}
				if (list.Count > 0)
				{
					BattlePlayingCardDataInUnitModel battlePlayingCardDataInUnitModel2 = RandomUtil.SelectOne(list);
					battlePlayingCardDataInUnitModel2.card.XmlData.DiceBehaviourList[0].Script = "cryingChildPenalty_Finnal";
					battlePlayingCardDataInUnitModel2.ResetCardQueue();
				}
			}
			public void SetAllWeak()
			{
				if (!_weak)
				{
					foreach (BattleUnitModel battleUnitModel in BattleObjectManager.instance.GetAliveList(Faction.Enemy))
					{
						battleUnitModel.Book.SetResistHP(BehaviourDetail.Slash, AtkResist.Weak);
						battleUnitModel.Book.SetResistHP(BehaviourDetail.Penetrate, AtkResist.Weak);
						battleUnitModel.Book.SetResistHP(BehaviourDetail.Hit, AtkResist.Weak);
						battleUnitModel.Book.SetResistBP(BehaviourDetail.Slash, AtkResist.Weak);
						battleUnitModel.Book.SetResistBP(BehaviourDetail.Penetrate, AtkResist.Weak);
						battleUnitModel.Book.SetResistBP(BehaviourDetail.Hit, AtkResist.Weak);
						battleUnitModel.bufListDetail.AddBuf(new BattleUnitBuf_theCryingWeak());
					}
				}
				_weak = true;
			}
			public void CryingChildrenEdit(BattleUnitModel battleUnitModel, bool special) {
				if (!special) {
					battleUnitModel.allyCardDetail.AddNewCard(508007, false);
					battleUnitModel.allyCardDetail.AddNewCardToDeck(508006, false);
					battleUnitModel.allyCardDetail.AddNewCardToDeck(508007, false);
					battleUnitModel.allyCardDetail.AddNewCardToDeck(508006, false);
					battleUnitModel.allyCardDetail.AddNewCardToDeck(508007, false);
				} else {
					battleUnitModel.allyCardDetail.AddNewCard(508008, false);
					battleUnitModel.allyCardDetail.AddNewCard(508008, false);
					battleUnitModel.allyCardDetail.AddNewCard(508008, false);
					battleUnitModel.allyCardDetail.AddNewCard(508008, false);
					battleUnitModel.allyCardDetail.AddNewCard(508008, false);
					battleUnitModel.allyCardDetail.AddNewCard(508008, false);
					battleUnitModel.allyCardDetail.AddNewCard(508008, false);
					battleUnitModel.passiveDetail.AddPassive(new PassiveAbility_10001());
					battleUnitModel.forceRetreat = true;
					PassiveAbilityBase passiveAbilityBase = battleUnitModel.passiveDetail.PassiveList.Find((PassiveAbilityBase x) => x is PassiveAbility_240228);
					if (passiveAbilityBase != null)
					{
						battleUnitModel.passiveDetail.DestroyPassive(passiveAbilityBase);
					}
					PassiveAbilityBase passiveAbilityBase2 = battleUnitModel.passiveDetail.PassiveList.Find((PassiveAbilityBase x) => x is PassiveAbility_240628);
					if (passiveAbilityBase2 != null)
					{
						battleUnitModel.passiveDetail.DestroyPassive(passiveAbilityBase2);
					}
				}
			}

			public readonly List<string> dlgIdList = new List<string> {
				"ChildCovering_nomal_1",
				"ChildCovering_nomal_2",
				"ChildCovering_nomal_3",
				"ChildCovering_nomal_4",
				"ChildCovering_nomal_5",
				"ChildCovering_nomal_6",
				"ChildCovering_nomal_7",
				"ChildCovering_nomal_8",
			};
			public readonly List<string> dlgIdList2 = new List<string> {
				"TheCryingChildren_nomal_1",
				"TheCryingChildren_nomal_2",
				"TheCryingChildren_nomal_3",
				"TheCryingChildren_nomal_4",
				"TheCryingChildren_nomal_5",
				"TheCryingChildren_nomal_6",
				"TheCryingChildren_nomal_7",
				"TheCryingChildren_nomal_8",
				"TheCryingChildren_nomal_9",
				"TheCryingChildren_nomal_10",
				"TheCryingChildren_nomal_11",
				"TheCryingChildren_nomal_12",
			};
			private bool _weak;
			public IEnumerable<BattleUnitModel> childList;
			public readonly List<int> childIDList = new List<int> {
				42002,
				42003,
				42004,
				42005,
				42006,
				42007,
				42008,
			};
		}
	}
}