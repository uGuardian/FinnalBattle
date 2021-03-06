using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UI;
using UnityEngine;
using LOR_DiceSystem;

namespace FinallyBeyondTheTime {
	public partial class EnemyTeamStageManager_UltimaAgain : EnemyTeamStageManager {
		public class BlackSilenceHandler {
			public readonly List<string> dlgIdList = new List<string> {
				"Roland_Black_1_1",
				"Roland_Black_1_2",
				"Roland_Black_1_3",
				"Roland_Black_1_4",
				"Roland_Black_1_5",
				"Roland_Black_1_6",
				"Roland_Black_1_7",
				"Roland_Black_1_8",
				"Roland_Black_1_9",
				"Roland_Black_1_10",
			};
			public readonly List<string> dlgIdList2 = new List<string> {
				"Roland_Black_2_1",
				"Roland_Black_2_2",
				"Roland_Black_2_3",
				"Roland_Black_2_4",
				"Roland_Black_2_5",
				"Roland_Black_2_6",
				"Roland_Black_2_7",
				"Roland_Black_2_8",
				"Roland_Black_2_9",
				"Roland_Black_2_10",
				"Roland_Black_2_11",
				"Roland_Black_2_12",
				"Roland_Black_2_13",
				"Roland_Black_2_14",
				"Roland_Black_2_15",
			};
			public readonly List<string> dlgIdList3 = new List<string> {
				"Roland_Black_3_1",
				"Roland_Black_3_2",
				"Roland_Black_3_3",
				"Roland_Black_3_4",
				"Roland_Black_3_5",
				"Roland_Black_3_6",
				"Roland_Black_3_7",
				"Roland_Black_3_8",
				"Roland_Black_3_9",
				"Roland_Black_3_10",
				"Roland_Black_3_11",
				"Roland_Black_3_12",
				"Roland_Black_3_13",
				"Roland_Black_3_14",
				"Roland_Black_3_15",
			};
			public readonly EnemyTeamStageManager_BlackSilence PsuedoManager = new EnemyTeamStageManager_BlackSilence {
				curPhase = EnemyTeamStageManager_BlackSilence.Phase.THIRD,
				thirdPhaseElapsed = -1,
			};
			public readonly bool[] finishedPhases = new bool[4];
			public void CheckPhase() {
				for (int i = 0; i < finishedPhases.Length; i++) {
					if (finishedPhases[i]) {continue;}
					switch (i) {
						case 0 when PsuedoManager.IsFirstPhaseEnd():
							// PsuedoManager.EndFirstPhase();
							finishedPhases[i] = true;
							break;
						case 1 when PsuedoManager.IsSecondPhaseEnd():
							// PsuedoManager.EndSecondPhase();
							finishedPhases[i] = true;
							break;
						case 2 when PsuedoManager.IsThirdPhaseEnd():
							PsuedoManager.EndThirdPhase();
							finishedPhases[i] = true;
							break;
						case 2:
							PsuedoManager.CheckThirdPhasePattern();
							break;
						case 3 when PsuedoManager.IsFourthPhaseEnd():
							// PsuedoManager.EndFourthPhase();
							finishedPhases[i] = true;
							break;
					}
				}
			}
		}
	}
}