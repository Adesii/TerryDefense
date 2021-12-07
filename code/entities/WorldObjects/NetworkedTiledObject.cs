using System.Collections.Generic;
using Sandbox;

namespace TerryDefense.entities {
	public partial class NetworkedTiledObject : BaseNetworkable {
		/// <summary>
		/// The object id
		/// </summary>
		[Net] public int id { get; set; }
		/// <summary>
		/// The object's name
		/// </summary>
		[Net] public string name { get; set; }
		/// <summary>
		/// The object type if defined. Null if none was set.
		/// </summary>
		[Net] public string type { get; set; }
		[Net] public Vector3 Position { get; set; }
		/// <summary>
		/// The object's rotation
		/// </summary>
		[Net] public float Rotation { get; set; }
		[Net] public Vector3 Size { get; set; }
		/// <summary>
		/// The tileset gid when the object is linked to a tile
		/// </summary>
		[Net] public int gid { get; set; }
		/// <summary>
		/// An array of properties. Is null if none were defined.
		/// </summary>
		[Net] public List<NetworkedTiledProperty> properties { get; set; }
		/// <summary>
		/// If an object was set to a polygon shape, this property will be set and can be used to access the polygon's data
		/// </summary>
		[Net] public NetworkedTiledPolygon polygon { get; set; }

		[Net] public Color Color { get; set; }
	}

	public partial class NetworkedTiledProperty : BaseNetworkable {
		/// <summary>
		/// The property name or key in string format
		/// </summary>
		[Net] public string name { get; set; }
		/// <summary>
		/// The property type as used in Tiled. Can be bool, number, string, ...
		/// </summary>
		[Net] public string type { get; set; }
		/// <summary>
		/// The value in string format
		/// </summary>
		[Net] public string value { get; set; }
	}

	public partial class NetworkedTiledPolygon : BaseNetworkable {
		/// <summary>
		/// The array of vertices where each two elements represent an x and y position. Like 'x,y,x,y,x,y,x,y'.
		/// </summary>
		[Net] public List<float> points { get; set; }
	}
}
