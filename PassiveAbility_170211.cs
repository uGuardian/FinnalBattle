using System;
using UnityEngine;
#pragma warning disable IDE0051,IDE0059,IDE0051

namespace FinallyBeyondTheTime.PassiveAbilities
{
	public class PassiveAbility_170211_Finnal : PassiveAbility_170211 {
		public override void OnRoundStart() {
			base.OnRoundStart();
			SetCard();
		}
		public override void OnCreated() {
			name = Singleton<PassiveDescXmlList>.Instance.GetName(170211);
			desc = Singleton<PassiveDescXmlList>.Instance.GetDesc(170211);
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
					AddNewCard(705213, 100);
					AddNewCard(705214, 90);
					AddNewCard(705215, 80);
					AddNewCard(705211, 60);
					AddNewCard(705212, 50);
					break;
				case 1:
					AddNewCard(705215, 100);
					AddNewCard(705214, 90);
					AddNewCard(705218, 80);
					AddNewCard(705211, 60);
					AddNewCard(705212, 40);
					break;
				case 2:
					AddNewCard(705217, 100);
					AddNewCard(705213, 90);
					AddNewCard(705218, 80);
					AddNewCard(705218, 70);
					AddNewCard(705211, 60);
					AddNewCard(705212, 50);
					AddNewCard(705212, 40);
					break;
				case 3:
					AddNewCard(705214, 100);
					AddNewCard(705215, 90);
					AddNewCard(705213, 80);
					AddNewCard(705218, 70);
					break;
				case 4:
					AddNewCard(705216, 100);
					AddNewCard(705214, 90);
					AddNewCard(705215, 80);
					AddNewCard(705213, 70);
					AddNewCard(705218, 60);
					break;
				default:
					Debug.Log("SetCard Phase Error in " + GetType().ToString());
					break;
			}
		}
	}
}