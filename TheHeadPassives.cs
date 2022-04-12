namespace FinallyBeyondTheTime.PassiveAbilities
{
	public class PassiveAbility_180001_Finnal : PassiveAbility_180001 {
		public override void OnWaveStart() {
			_stageManager = (Singleton<StageController>.Instance.EnemyStageManager as EnemyTeamStageManager_UltimaAgain)?.FFH;
			SetStartingCards();
			owner.view.EnableStatNumber(false);
			owner.SetHp(owner.MaxHp);
		}
		public override BattleUnitModel ChangeAttackTarget(BattleDiceCardModel card, int idx) => null;
		public override void OnRoundStartAfter() {
			if (FinnalConfig.Instance.HardmodeHead == 0) {
				base.OnRoundStartAfter();
			}
		}
		public override void OnRoundStart() {
			if (FinnalConfig.Instance.HardmodeHead >= 2) {
				owner.allyCardDetail.DrawCards(8);
				owner.cardSlotDetail.RecoverPlayPoint(12);
			} else {
				base.OnRoundStart();
			}
		}
	}
	public class PassiveAbility_180002_Finnal : PassiveAbility_180002 {
		public override void OnWaveStart() {
			_stageManager = (Singleton<StageController>.Instance.EnemyStageManager as EnemyTeamStageManager_UltimaAgain)?.FFH;
			SetStartingCards();
			owner.view.EnableStatNumber(false);
			owner.SetHp(owner.MaxHp);
			owner.bufListDetail.AddBuf(new BattleUnitBuf_clawCounter());
		}
		public override BattleUnitModel ChangeAttackTarget(BattleDiceCardModel card, int idx) => null;
		public override void OnRoundStartAfter() {
			if (FinnalConfig.Instance.HardmodeHead == 0) {
				base.OnRoundStartAfter();
			}
		}
		public override void OnRoundStart() {
			if (FinnalConfig.Instance.HardmodeHead >= 2) {
				owner.allyCardDetail.DrawCards(8);
				owner.cardSlotDetail.RecoverPlayPoint(12);
			} else {
				base.OnRoundStart();
			}
		}
	}
}
namespace FinallyBeyondTheTime {
	// public class PsuedoFinalFinalManager : EnemyTeamStageManager_FinalFinal {}
}