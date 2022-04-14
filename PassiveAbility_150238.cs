namespace FinallyBeyondTheTime.PassiveAbilities
{
	public class PassiveAbility_150238_Finnal : PassiveAbility_150238 {
		public override void OnWaveStart() {
			_patternCount = FinnalConfig.Instance.XiaoNullStart + 1;
		}
		public override void OnCreated() {
			name = Singleton<PassiveDescXmlList>.Instance.GetName(150238);
			desc = Singleton<PassiveDescXmlList>.Instance.GetDesc(150238);
			base.OnCreated();
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