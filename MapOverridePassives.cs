using System;
using System.Collections.Generic;
using Sound;
using UnityEngine;

namespace FinallyBeyondTheTime.PassiveAbilities {
	public class PassiveAbility_150036_Finnal : PassiveAbility_150036 {
		public override void OnRoundStart() {
			/*
			if (this.owner.faction == Faction.Enemy) {
				int emotionTotalCoinNumber = Singleton<StageController>.Instance.GetCurrentWaveModel().team.emotionTotalCoinNumber;
				Singleton<StageController>.Instance.GetCurrentWaveModel().team.emotionTotalBonus = -emotionTotalCoinNumber;
			}
			*/
			this._speedDiceNumAdder = 2;
			// this.owner.SetDeadSceneBlock(true);
			if (!this._defeated && (int) this.owner.hp > this._defeatCondition)
				return;
			this._defeated = true;
			this.owner.view.charAppearance.ChangeMotion(ActionDetail.Damaged);
		}
	}
	public class PassiveAbility_150037_Finnal : PassiveAbility_150037 {
		public override void OnRoundStart() {}
	}
	/* This can be removed outright
	public class PassiveAbility_230013_Finnal : PassiveAbility_230013 {
		public override void OnRoundStart() {}
	}
	*/
	public class PassiveAbility_230124_Finnal : PassiveAbility_230124 {
		public override void OnRoundStart() {}
	}
	public class PassiveAbility_1300001_Finnal : PassiveAbility_1300001 {
		public override void OnRoundStart() {}
	}
	public class PassiveAbility_150138_Finnal : PassiveAbility_150138 {
		public override void OnRoundStart() {
			List<BattleUnitModel> aliveList = BattleObjectManager.instance.GetAliveList_opponent(owner.faction);
			foreach (BattleUnitModel battleUnitModel in aliveList)
				battleUnitModel.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Burn, 2);
		}
	}
	public class PassiveAbility_151139_Finnal : PassiveAbility_151139 {
		public override void OnRoundStart() {}
	}
	/* Black Silence MapReset methods are unused and do not need to be overridden.
	public class PassiveAbility_170301_Finnal : PassiveAbility_170301 {}
	public class PassiveAbility_170311_Finnal : PassiveAbility_170311 {}
	*/
	public class PassiveAbility_230028_Finnal : PassiveAbility_230028 {
		public override void OnRoundStart() {
			BurnAll();
			// int emotionTotalCoinNumber = Singleton<StageController>.Instance.GetCurrentStageFloorModel().team.emotionTotalCoinNumber;
			// Singleton<StageController>.Instance.GetCurrentWaveModel().team.emotionTotalBonus = emotionTotalCoinNumber + 1;
			_speedDiceAdder = 0;
			owner.Book.SetOriginalResists();
			if (owner.IsBreakLifeZero()) {
				owner.bufListDetail.AddBuf(new PhilipBuf4(3 - _patternCount + 1));
			}
			else {
				if (_patternCount == 0 || _patternCount == 1) {
					owner.bufListDetail.AddBuf(new PhilipBuf2(2 - _patternCount));
					owner.bufListDetail.AddBuf(new PhilipBuf4(3 - _patternCount));
					// Singleton<StageController>.Instance.GetStageModel().SetCurrentMapInfo(0);
					owner.view.ChangeSkin("PhilipEgoProtection");
					if (_protectionEffect == null) {
						UnityEngine.Object original = Resources.Load("Prefabs/Battle/BufEffect/PhilipProtectionEffect");
						if (original != null)
							_protectionEffect = UnityEngine.Object.Instantiate(original, owner.view.atkEffectRoot) as GameObject;
					}
					owner.allyCardDetail.ExhaustAllCards();
					for (int index = 0; index < 2; ++index)
						owner.allyCardDetail.AddNewCard(408010)?.SetCostToZero();
					for (int index = 0; index < 1; ++index)
						owner.allyCardDetail.AddNewCard(408011)?.SetCostToZero();
				}
				else if (_patternCount == 2) {
					owner.bufListDetail.AddBuf(new PhilipBuf1(3 - _patternCount));
					owner.bufListDetail.AddBuf(new PhilipBuf4(3 - _patternCount));
					// Singleton<StageController>.Instance.GetStageModel().SetCurrentMapInfo(1);
					owner.view.ChangeSkin("PhilipEgo");
					if (_protectionEffect != null) {
						UnityEngine.Object.Destroy(_protectionEffect);
						_protectionEffect = null;
					}
					owner.allyCardDetail.ExhaustAllCards();
					for (int index = 0; index < 2; ++index)
						owner.allyCardDetail.AddNewCard(408008)?.SetCostToZero();
					for (int index = 0; index < 1; ++index)
						owner.allyCardDetail.AddNewCard(408009)?.SetCostToZero();
				}
				else if (_patternCount == 3) {
					owner.bufListDetail.AddBuf(new PhilipBuf3());
					// Singleton<StageController>.Instance.GetStageModel().SetCurrentMapInfo(2);
					owner.view.ChangeSkin("PhilipEgoFury");
					SingletonBehavior<SoundEffectManager>.Instance.PlayClip("Battle/Philip_MapChange_Strong");
					if (_protectionEffect != null) {
						UnityEngine.Object.Destroy(_protectionEffect);
						_protectionEffect = null;
					}
					_speedDiceAdder = 2;
					owner.allyCardDetail.ExhaustAllCards();
					for (int index = 0; index < 1; ++index)
						owner.allyCardDetail.AddNewCard(408007)?.SetCostToZero();
					for (int index = 0; index < 4; ++index)
						owner.allyCardDetail.AddNewCard(408008)?.SetCostToZero();
					_patternCount = 0;
					return;
				}
				++_patternCount;
			}
		}
	}
}