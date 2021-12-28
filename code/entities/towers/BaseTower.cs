using System;
using Sandbox;
using TerryDefense.Units;
using System.Linq;

namespace TerryDefense.Towers {
	public class BaseTower : AnimEntity {
		public virtual float Range { get; set; } = 200f;
		public virtual float Damage { get; set; } = 10f;
		public virtual float FireRate { get; set; } = 10f;

		public TimeSince LastFired { get; set; } = 0f;

		public BaseUnit Target { get; set; }
		public TargetingType TargetingType { get; set; }

		[TDEvent.Tower.Tick]
		public virtual void FireTick() {
			if(LastFired < FireRate / 60f) {
				return;
			}

			if(Target == null || !Target.IsValid()) {
				Target = GetNewTarget();
			}
			if(!Target.IsValid()) return;

			Target.TakeDamage(this, Damage);
			LastFired = 0f;

		}
		[TDEvent.Tower.Attacked]
		public virtual void OnAttacked() {

		}
		public virtual void OnDeploy() {

		}

		public virtual BaseUnit GetNewTarget() {
			switch(TargetingType) {
				case TargetingType.Closest:
					return GetClosestTarget();
				case TargetingType.Weakest:
					return GetWeakestTarget();
				case TargetingType.Strongest:
					return GetStrongestTarget();
				case TargetingType.Farthest:
					return GetFarthestTarget();
				default:
					return null;
			}
		}

		private BaseUnit GetFarthestTarget() {
			return BaseUnit.Units.Where(x => x.Position.Distance(Position) <= Range).OrderByDescending(x => x.Position.Distance(Position)).First();
		}

		private BaseUnit GetStrongestTarget() {
			return BaseUnit.Units.Where(x => x.Position.Distance(Position) <= Range).OrderByDescending(x => x.MaxHealth).First();
		}

		private BaseUnit GetWeakestTarget() {
			return BaseUnit.Units.Where(x => x.Position.Distance(Position) <= Range).OrderBy(x => x.MaxHealth).First();
		}

		private BaseUnit GetClosestTarget() {
			return BaseUnit.Units.Where(x => x.Position.Distance(Position) <= Range).OrderBy(x => x.Position.Distance(Position)).First();
		}
	}

	public enum TargetingType {
		Farthest,
		Closest,
		Weakest,
		Strongest,
	}
}
