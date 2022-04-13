namespace FinallyBeyondTheTime.PassiveAbilities
{
	public class PassiveAbility_150238_Finnal : PassiveAbility_150238 {
		public override void OnWaveStart() {
			_patternCount = FinnalConfig.Instance.XiaoNullStart + 1;
		}

		public override void OnRoundStart() {
			_patternCount--;
			if (_patternCount == 0) {
				foreach (BattleUnitModel alive in BattleObjectManager.instance.GetAliveList())
					alive.bufListDetail.AddKeywordBufThisRoundByEtc(KeywordBuf.NullifyPower, 1);
				_patternCount = FinnalConfig.Instance.XiaoNullCooldown+1;
				if (_patternCount > 0) {
					owner.bufListDetail.AddBuf(new BattleUnitBuf_ready {
						stack = _patternCount
					});
				}
				_effect = true;
			} else if (_patternCount > 0) {
				owner.bufListDetail.AddBuf(new BattleUnitBuf_ready {
					stack = _patternCount
				});
				_effect = false;
			} else {
				_effect = false;
			}
		}
	}
}