using UnityEngine;

namespace FinallyBeyondTheTime.PassiveAbilities
{
	public class PassiveAbility_250227_Finnal : PassiveAbility_250227
	{
		public override void OnRoundEndTheLast()
		{
			if (owner.UnitData.floorBattleData.param2 <= 0 && (_teleportReady || owner.hp <= _teleportCondition))
			{
				timerComponent = (TeleportTimer)BattleObjectLayer.instance.gameObject.AddComponent(typeof(TeleportTimer));
				timerComponent.Parent = this;
				owner.breakDetail.RecoverBreakLife(owner.MaxBreakLife, false);
				owner.breakDetail.nextTurnBreak = false;
				owner.breakDetail.RecoverBreak(owner.breakDetail.GetDefaultBreakGauge());
				owner.UnitData.floorBattleData.param2 = 1;
			} else if (returned) {
				timerComponent = (TeleportTimer)BattleObjectLayer.instance.gameObject.AddComponent(typeof(TeleportTimer));
				timerComponent.Parent = this;
			}
		}
		private class TeleportTimer : MonoBehaviour {

			public void FixedUpdate() {
				if (Parent != null) {
					Parent.TeleportUpdate(Time.deltaTime);
				}
			}
			public PassiveAbility_250227_Finnal Parent;
		}
		private void TeleportUpdate(float delta)
		{
			if (_elapsedTimeTeleport < Mathf.Epsilon)
			{
				Object @object = Resources.Load("Prefabs/Battle/BufEffect/Purple_Teleport");
				if (@object != null)
				{
					Teleport_Fast component = (Object.Instantiate(@object, owner.view.atkEffectRoot) as GameObject).GetComponent<Teleport_Fast>();
					if (component != null)
					{
						component.PlayEffect();
					}
				}
			}
			_elapsedTimeTeleport += delta;
			if (!viewChanged && _elapsedTimeTeleport > 0.8f)
			{
				owner.view.EnableView(returned);
				viewChanged = true;
			}
			if (_elapsedTimeTeleport > 1.1f) {
				if (!returned) {
					returned = true;
					BattleObjectManager.instance.UnregisterUnit(owner);
					SingletonBehavior<HexagonalMapManager>.Instance.ResetMapSetting();
					SingletonBehavior<HexagonalMapManager>.Instance.OnRoundStart();
				} else {
					returned = false;
				}
				_elapsedTimeTeleport = 0f;
				viewChanged = false;
				Object.Destroy(timerComponent);
			}
		}
		private bool returned = false;
		private bool viewChanged = false;
		private float _elapsedTimeTeleport = 0f;
		private TeleportTimer timerComponent;
	}
}