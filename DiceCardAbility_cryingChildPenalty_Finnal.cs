using System;
using System.Collections;
using UnityEngine;
using FinallyBeyondTheTime;

namespace FinallyBeyondTheTime {
	public class DiceCardAbility_cryingChildPenalty_Finnal : DiceCardAbility_cryingChildPenalty {
		public override void OnLoseParrying() {
			EnemyTeamStageManager_UltimaAgain finnalStageManager = Singleton<StageController>.Instance.EnemyStageManager as EnemyTeamStageManager_UltimaAgain;
			finnalStageManager.CCH.SetAllWeak();
			base.OnLoseParrying();
		}
	}
}