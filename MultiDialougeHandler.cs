using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UI;
using UnityEngine;
using LOR_DiceSystem;

namespace FinallyBeyondTheTime {
	public partial class EnemyTeamStageManager_UltimaAgain : EnemyTeamStageManager {
		public class MultiDialougeHandler {
			private class MultiDialog : MonoBehaviour {
				public void Init(List<string> dlgIdList, Color dlgColor) {
					_dlgIdList = dlgIdList;
					_dlgColor = dlgColor;
				}
				public void CreateDialog()
				{
					if (_dlgIdList.Count <= 0 || _dlgIdx >= _dlgIdList.Count)
					{
						return;
					}
					string text = TextDataModel.GetText(_dlgIdList[_dlgIdx], Array.Empty<object>());
					if (_dlgEffect != null && _dlgEffect.gameObject != null)
					{
						_dlgEffect.FadeOut();
					}
					_dlgEffect = SingletonBehavior<CreatureDlgManagerUI>.Instance.SetDlg(text, _dlgColor, null);
				}
				#pragma warning disable IDE0051
				private void Update() {
					if (stop) {
						if (_dlgEffect != null && _dlgEffect.gameObject != null && !_dlgEffect.DisplayDone) {
							_dlgEffect.FadeOut();
						} else {
							Destroy(this);
						}
						return;
					}
					if (_dlgEffect != null && _dlgEffect.gameObject != null)
					{
						if (_dlgEffect.DisplayDone)
						{
							_dlgIdx = (_dlgIdx + 1) % _dlgIdList.Count;
							CreateDialog();
							return;
						}
					}
					else
					{
						CreateDialog();
					}
				}
				#pragma warning restore IDE0051

				private List<string> _dlgIdList;
				private int _dlgIdx = 0;
				private Color _dlgColor;
				private CreatureDlgEffectUI _dlgEffect;
				public bool stop;
			}
			public void InitDialog(List<string> dlgIdList, Color txtColor) {
				var dialog = SingletonBehavior<BattleSceneRoot>.Instance.currentMapObject.gameObject.AddComponent<MultiDialog>();
				dialog.Init(dlgIdList, txtColor);
				dialogList[dlgIdList] = dialog;
			}
			public void ClearDialog(List<string> dlgIdList) {
				if (dialogList.TryGetValue(dlgIdList, out var dialog)) {
					dialog.stop = true;
				}
			}
			public void ClearDialog() {
				foreach (var dialog in dialogList) {
					dialog.Value.stop = true;
				}
				foreach (var dialog in EffectList) {
					dialog.FadeOut();
				}
			}
			public List<string> yanDlgIdList = new List<string>{
				"Index_Yan_1",
				"Index_Yan_2",
				"Index_Yan_3",
				"Index_Yan_4",
				"Index_Yan_5",
				"Index_Yan_6",
				"Index_Yan_7",
				"Index_Yan_8",
				"Index_Yan_9",
				"Index_Yan_10",
				"Index_Yan_11",
				"Index_Yan_12",
				"Index_Yan_13",
				"Index_Yan_14",
				"Index_Yan_15",
				"Index_Yan_16",
				"Index_Yan_17",
				"Index_Yan_18",
				"Index_Yan_19",
				"Index_Yan_20",
			};
			
			private readonly FieldInfo _effectListOrigin = SingletonBehavior<CreatureDlgManagerUI>.Instance.GetType().GetField("_effectList", BindingFlags.NonPublic | BindingFlags.Instance);
			private List<CreatureDlgEffectUI> EffectList { get => _effectListOrigin.GetValue(SingletonBehavior<CreatureDlgManagerUI>.Instance) as List<CreatureDlgEffectUI>; }
			private readonly Dictionary<List<string>, MultiDialog> dialogList = new Dictionary<List<string>, MultiDialog>();
		}
	}
}