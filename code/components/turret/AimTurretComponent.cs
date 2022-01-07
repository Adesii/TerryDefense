using System;
using Sandbox;
using TerryDefense.Units;

namespace TerryDefense.components.turret {
	[Library]
	public partial class AimTurretComponent : BaseTurretComponent {

		public enum TargetingType {
			Farthest,
			Closest,
			Weakest,
			Strongest,
		}

		public TargetingType TargetType { get; set; } = TargetingType.Strongest;
		[Net] private BaseUnit CurrentTarget { get; set; }
		private Vector3 CurrentAimPosition;
		public float TurnRate { get; set; } = 10f;
		public float Range { get; set; } = 10f;

		public float FireRate { get; set; } = 1f;

		public float TargetUpdateTime = 1f;

		private TimeSince LastFire;
		private TimeSince LastTargetUpdate;

		public override void Tick() {
			if((CurrentTarget == null && LastTargetUpdate > TargetUpdateTime) || LastTargetUpdate > TargetUpdateTime) {
				CurrentTarget = GetTarget();
				LastTargetUpdate = 0f;
			}
			Debug.Sphere(Entity.Position, Range, Color.Red.WithAlpha(0.1f), 0f);
			if(CurrentTarget != null) {
				if(CurrentTarget.LifeState != LifeState.Alive || CurrentTarget.Position.Distance(Entity.Position) > Range) {
					CurrentTarget = null;
					return;
				}
				CurrentAimPosition = CurrentAimPosition.LerpTo(CurrentTarget.Position + CurrentTarget.Velocity, TurnRate * Time.Delta);
				Entity.SetAnimVector("aim_pos", CurrentAimPosition);
				Debug.Sphere(CurrentAimPosition, 10f, Color.Green, 0f);
				Debug.Sphere(CurrentTarget.Position, 10f, Color.Red, 0f);

				if(CurrentAimPosition.Distance(CurrentTarget.Position) < 20 && LastFire > FireRate) {
					foreach(var item in Entity.Components.GetAll<ElementalDamageComponent>()) {
						item.DealDamage(CurrentTarget);
					}

					LastFire = 0;
				}
			}
		}

		private BaseUnit GetTarget() {
			switch(TargetType) {
				case TargetingType.Farthest:
					return BaseUnit.GetFarthestTarget(Entity, Range);
				case TargetingType.Closest:
					return BaseUnit.GetClosestTarget(Entity, Range);
				case TargetingType.Weakest:
					return BaseUnit.GetWeakestTarget(Entity, Range);
				case TargetingType.Strongest:
					return BaseUnit.GetStrongestTarget(Entity, Range);
				default:
					throw new ArgumentOutOfRangeException();
			}

		}
	}

}
