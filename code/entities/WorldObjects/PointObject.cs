namespace TerryDefense.entities.WorldObjects {
	public class PointObject : WorldObject {
		public override void DebugObject() {
			base.DebugObject();
			Debug.Line(Position, Position + new Vector3(0, 0, 100), TileObject.Color);
		}
	}
}
