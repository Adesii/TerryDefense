using System;
using Sandbox;
using TerryDefense.systems;

namespace TerryDefense.entities.WorldObjects {
	public class BoxObject : WorldObject {
		public override void DebugObject() {
			base.DebugObject();
			//Debug.Box(Position, Vector3.One * 50, -Vector3.One * 50, TileObject.Color, true);
			BBox idk = new(TileObject.Position, (TileObject.Position + TileObject.Size));
			//idk += WorldManager.Instance.CurrentMapBounds.Mins.WithX(0);
			//idk += WorldManager.Instance.CurrentMapBounds.Maxs.WithY(0);
			var idkk = (idk.Mins - idk.Maxs).z.AlmostEqual(0f) ? idk.Maxs + new Vector3(0, 0, 4f) : idk.Maxs;
			var mins = idk.Mins + new Vector3(0, 0, 1f);
			Debug.Box(mins, idkk, TileObject.Color);
		}
	}
}
