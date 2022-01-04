using System;
using System.Collections.Generic;
using System.Linq;
using Gamelib.FlowFields;
using Sandbox;
using TerryDefense.Towers;

namespace TerryDefense.Units {
	public class BaseUnit : AnimEntity {
		private Pathfinder Pathfinder { get; set; }
		public Vector3 Target { get; set; }
		public virtual float Speed { get; private set; } = 100f;
		public virtual float MaxHealth { get; private set; } = 100f;

		public static List<BaseUnit> Units { get; set; } = new List<BaseUnit>();

		private PathRequest pr;

		public override void Spawn() {
			base.Spawn();
			SetModel("models/citizen/citizen.vmdl");
			SetupPhysicsFromModel(PhysicsMotionType.Keyframed);
			Tags.Add("ff_ignore");
			PathManager.Create(25, 50);
			Pathfinder = PathManager.GetPathfinder(25, 50);
			pr = Pathfinder.Request(Target);
			Units.Add(this);

			Health = MaxHealth;
		}

		protected override void OnDestroy() {
			base.OnDestroy();
			Units.Remove(this);
		}

		[Event.Tick.Server]
		public virtual void Update() {
			Move();
		}

		public void TakeDamage(float damage) {
			if(IsServer && Health > 0f && LifeState == LifeState.Alive) {
				Health -= damage;
				if(Health <= 0f) {
					Health = 0f;
					OnKilled();
				}
			}
		}

		public void Move() {
			if(Pathfinder == null || pr == null || !pr.IsValid()) {
				RebuildPath();
				return;
			}
			if(Position.Distance(Target) < 50) {
				Delete();
			}
			if(!pr.HasDestination()) {
				pr = Pathfinder.Request(Target);
			}
			Vector3 next = pr.GetDirection(Position);
			Position += next * Speed * Time.Delta;
		}

		public void RebuildPath() {
			//Log.Error("Rebuilding path");
			pr = null;
			Pathfinder = null;
			PathManager.Create(25, 50);
			Pathfinder = PathManager.GetPathfinder(25, 50);
			pr = Pathfinder.Request(Target);
		}


		public static BaseUnit GetFarthestTarget(BaseTower Tower, float Range) {
			return BaseUnit.Units.Where(x => x.Position.Distance(Tower.Position) <= Range).OrderByDescending(x => x.Position.Distance(Tower.Position)).FirstOrDefault();
		}

		public static BaseUnit GetStrongestTarget(BaseTower Tower, float Range) {
			return BaseUnit.Units.Where(x => x.Position.Distance(Tower.Position) <= Range).OrderByDescending(x => x.MaxHealth).FirstOrDefault();
		}

		public static BaseUnit GetWeakestTarget(BaseTower Tower, float Range) {
			return BaseUnit.Units.Where(x => x.Position.Distance(Tower.Position) <= Range).OrderBy(x => x.MaxHealth).FirstOrDefault();
		}

		public static BaseUnit GetClosestTarget(BaseTower Tower, float Range) {
			return BaseUnit.Units.Where(x => x.Position.Distance(Tower.Position) <= Range).OrderBy(x => x.Position.Distance(Tower.Position)).FirstOrDefault();
		}
	}
}
