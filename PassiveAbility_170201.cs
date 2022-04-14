using UnityEngine;

namespace FinallyBeyondTheTime.PassiveAbilities {
	public class PassiveAbility_170201_Finnal : PassiveAbility_170201 {
		public override void OnRoundStart() {
			_bHide = IsAngelicaDead();
			SetCard();
		}
		public override void OnCreated() {
			name = Singleton<PassiveDescXmlList>.Instance.GetName(170201);
			desc = Singleton<PassiveDescXmlList>.Instance.GetDesc(170201);
			base.OnCreated();
		}
		new private void SetCard() {
			if (_bHide || owner.IsBreakLifeZero())
				return;
			owner.allyCardDetail.ExhaustAllCards();
			int num;
			if (Singleton<StageController>.Instance.EnemyStageManager is EnemyTeamStageManager_UltimaAgain enemyStageManager)
				num = enemyStageManager.BSH.PsuedoManager.thirdPhaseElapsed;
			else
				num = -1;
			switch (num) {
				case 0:
					AddNewCard(705203, 100);
					AddNewCard(705204, 90);
					AddNewCard(705205, 80);
					AddNewCard(705201, 60);
					AddNewCard(705202, 50);
					break;
				case 1:
					AddNewCard(705205, 100);
					AddNewCard(705204, 90);
					AddNewCard(705206, 80);
					AddNewCard(705201, 60);
					AddNewCard(705202, 50);
					break;
				case 2:
					AddNewCard(705209, 100);
					AddNewCard(705203, 90);
					AddNewCard(705206, 80);
					AddNewCard(705206, 70);
					AddNewCard(705201, 60);
					AddNewCard(705201, 50);
					AddNewCard(705202, 40);
					break;
				case 3:
					AddNewCard(705207, 100);
					AddNewCard(705208, 90);
					AddNewCard(705207, 80);
					AddNewCard(705208, 70);
					break;
				case 4:
					AddNewCard(705207, 100);
					AddNewCard(705208, 90);
					AddNewCard(705207, 80);
					AddNewCard(705208, 70);
					AddNewCard(705206, 60);
					break;
				default:
					Debug.Log("SetCard Phase Error in " + GetType().ToString());
					break;
			}
		}
	}
}