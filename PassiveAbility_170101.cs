using System;
using System.Collections.Generic;
using UI;

namespace FinallyBeyondTheTime.PassiveAbilities
{
	public class PassiveAbility_170101_Finnal : PassiveAbility_170101
	{
		public override void OnRoundEndTheLast()
		{
			CreateEnemys();
		}

		new public void CreateEnemys() {
			return; //TODO Change Hatching Dead
			if (owner.hp <= 300.0)
				return;
			List<BattleUnitModel> all = BattleObjectManager.instance.GetList(Faction.Enemy).FindAll(x => x.IsDead() && x != owner);
			if (all.Count == 0)
				return;
			foreach (BattleUnitModel battleUnitModel1 in all) {
				BattleUnitModel battleUnitModel2 = Singleton<StageController>.Instance.AddNewUnit(Faction.Enemy, CREATE_ENEMY_ID, battleUnitModel1.index);
				if (battleUnitModel2 != null) {
					battleUnitModel2.SetDeadSceneBlock(false);
					owner.SetHp((int) owner.hp - 50);
				}
			}
			int num = 0;
			foreach (BattleUnitModel battleUnitModel in (IEnumerable<BattleUnitModel>) BattleObjectManager.instance.GetList())
				SingletonBehavior<UICharacterRenderer>.Instance.SetCharacter(battleUnitModel.UnitData.unitData, num++, true);
			BattleObjectManager.instance.InitUI();
		}
	}
}