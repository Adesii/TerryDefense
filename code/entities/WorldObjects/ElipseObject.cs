using System;
using System.Collections.Generic;
using TiledCS;

namespace TerryDefense.entities.WorldObjects {
	public class ElipseObject : WorldObject {
		public override void DebugObject() {
			base.DebugObject();
			//Create a list of points that make up the elipse
			Debug.Ellipse(Position, TileObject.Size, TileObject.Color);


		}
	}
}
