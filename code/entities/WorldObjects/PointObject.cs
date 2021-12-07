using System;
using System.Linq;
using Sandbox;
using TerryDefense.Units;

namespace TerryDefense.entities.WorldObjects {
	public class PointObject : WorldObject {
		private PointObject Goal;
		private TimeSince LastEnemy = 2f;


		[Event.Tick.Server]
		public void SpawnEnemies() {
			if(!Goal.IsValid()) {
				Goal = All.OfType<PointObject>().Where(e => e.TileObject.type == TileObjectTypes.Objectives).First();
				if(!Goal.IsValid()) return;
			}
			if(LastEnemy > 2f && All.OfType<BaseUnit>().Count() < 10 && TileObject.type != TileObjectTypes.Objectives) {
				var enemy = new BaseUnit() {
					Position = Position.WithZ(0)
				};
				enemy.Target = Goal.Position.WithZ(0);
				LastEnemy = 0f;
			}




		}
		public override void DebugObject() {
			base.DebugObject();
			Debug.Line(Position, Position + new Vector3(0, 0, 100), TileObject.Color);
		}
	}
}
