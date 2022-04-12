using System;

namespace FinallyBeyondTheTime.PassiveAbilities {
	public class PassiveAbility_240028_Finnal : PassiveAbility_240028
	{
		public override void OnRoundStart()
		{
			SetCards();
			if (owner.allyCardDetail.GetHand().Exists((BattleDiceCardModel x) => x.GetID() == 508001))
			{
				MapManager currentMapObject = SingletonBehavior<BattleSceneRoot>.Instance.currentMapObject;
				if (currentMapObject is CryingChildMapManager)
				{
					(currentMapObject as CryingChildMapManager).PlayAreaLaserSound();
				}
			}
		}

		new protected void SetCards()
		{
			owner.allyCardDetail.ExhaustAllCards();
			var id = owner.UnitData.unitData.EnemyUnitId.id;
			if (id == 42001)
			{
				_diceAdder = 2;
				AddNewCard(508003);
				AddNewCard(508004);
				if (RandomUtil.valueForProb < 0.5f)
				{
					AddNewCard(508003);
					return;
				}
				AddNewCard(508004);
				return;
			}
			else
			{
				if (id == 42101)
				{
					_diceAdder = 3;
					AddNewCard(508002);
					AddNewCard(508003);
					AddNewCard(508004);
					AddNewCard(508005);
					return;
				}
				_diceAdder = 4;
				AddNewCard(508001);
				AddNewCard(508002);
				AddNewCard(508002);
				if (RandomUtil.valueForProb < 0.5f)
				{
					AddNewCard(508003);
				}
				else
				{
					AddNewCard(508004);
				}
				AddNewCard(508005);
				return;
			}
		}
	}
}