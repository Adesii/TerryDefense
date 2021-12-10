using System;
using Gamelib.FlowFields;
using Sandbox;

namespace TerryDefense.Units {
	public class BaseUnit : AnimEntity {
		private Pathfinder Pathfinder { get; set; }
		public Vector3 Target { get; set; }
		public virtual float Speed { get; private set; } = 100f;
		public virtual float Health { get; private set; } = 100f;

		private PathRequest pr;

		public override void Spawn() {
			base.Spawn();
			SetModel("models/citizen/citizen.vmdl");
			SetupPhysicsFromModel(PhysicsMotionType.Keyframed);
			Tags.Add("ff_ignore");
			PathManager.Create(25, 50);
			Pathfinder = PathManager.GetPathfinder(25, 50);
			pr = Pathfinder.Request(Target);
		}

		[Event.Tick.Server]
		public virtual void Update() {
			Move();


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
	}
}
