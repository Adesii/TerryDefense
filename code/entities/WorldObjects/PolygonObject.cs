using System.Collections.Generic;

namespace TerryDefense.entities.WorldObjects {
	public class PolygonObject : WorldObject {
		public override void DebugObject() {

			base.DebugObject();
			//draw lines the polygon where the points list is a list of floats and its in the format of x,y,x,y,x,y,x,y foreach point in the list
			var points = new List<Vector3>();
			for(int i = 0; i < TileObject.polygon.points.Count - 1; i += 2) {
				float x = TileObject.polygon.points[i] * -2;
				float y = TileObject.polygon.points[i + 1] * 2;
				points.Add(new Vector3(x, y, TileObject.Size.z) + Position);
			}
			Debug.Polygon(points, TileObject.Color);




		}
	}
}
