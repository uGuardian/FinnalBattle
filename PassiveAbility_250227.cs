using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace FinallyBeyondTheTime
{
	public class PassiveAbility_250227_Finnal : PassiveAbility_250227
	{
		public override void OnRoundStart()
		{
			if (owner.UnitData.floorBattleData.param2 <= 0 && (TeleportReady || owner.hp <= (float)TeleportCondition))
			{
				timerComponent = BattleObjectLayer.instance.gameObject.AddComponent(new TeleportTimer().GetType()) as TeleportTimer;
				timerComponent.Parent = this;
				owner.breakDetail.RecoverBreakLife(owner.MaxBreakLife, false);
				owner.breakDetail.nextTurnBreak = false;
				owner.breakDetail.RecoverBreak(owner.breakDetail.GetDefaultBreakGauge());
				owner.UnitData.floorBattleData.param2 = 1;
			} else if (returned) {
				timerComponent = BattleObjectLayer.instance.gameObject.AddComponent(new TeleportTimer().GetType()) as TeleportTimer;
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
				UnityEngine.Object @object = Resources.Load("Prefabs/Battle/BufEffect/Purple_Teleport");
				if (@object != null)
				{
					Teleport_Fast component = (UnityEngine.Object.Instantiate(@object, owner.view.atkEffectRoot) as GameObject).GetComponent<Teleport_Fast>();
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
			if (_elapsedTimeTeleport > 2.0f) {
				if (!returned) {
					returned = true;
					_teleported = true;
				} else {
					returned = false;
				}
				_elapsedTimeTeleport = 0f;
				viewChanged = false;
				UnityEngine.Object.Destroy(timerComponent);
			}
		}
		public override void OnRoundEndTheLast() {
			if (_teleported) {
				_teleported = false;
				BattleObjectManager.instance.UnregisterUnit(owner);
			}
		}

		private bool returned = false;
		private bool viewChanged = false;
        private int TeleportCondition => (int)GetType().BaseType.GetField("_teleportCondition", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(this);
        private bool TeleportReady => (bool)GetType().BaseType.GetField("_teleportReady", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(this);
        private float _elapsedTimeTeleport = 0f;
		private bool _teleported = false;
		private TeleportTimer timerComponent;
	}
}