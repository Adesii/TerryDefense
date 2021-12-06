using System;
using System.Linq;
using System.Threading.Tasks;
using Sandbox;
using TiledCS;

namespace TerryDefense.entities {
	[Library("world_object")]
	public partial class WorldObject : Entity {
		[Net] public NetworkedTiledObject TileObject { get; set; }

		private SceneObject sceneObject;

		private static NetworkedTiledObject FromTiledObject(TiledObject tiledObject) {
			var worldObject = new NetworkedTiledObject {
				id = tiledObject.id,
				gid = tiledObject.gid,
				name = tiledObject.name,
				type = tiledObject.type,
				Position = new Vector3(tiledObject.x * 2, tiledObject.y * 2),
				Size = new Vector3(tiledObject.width * 2, tiledObject.height * 2),
				Rotation = tiledObject.rotation,
			};
			worldObject.Position *= new Vector3(-1, 1, 1);
			worldObject.Size *= new Vector3(-1, 1, 1);

			worldObject.Position += systems.WorldManager.Instance.CurrentMapBounds.Mins.WithX(0);
			worldObject.Position += systems.WorldManager.Instance.CurrentMapBounds.Maxs.WithY(0);
			if(tiledObject.properties != null)
				foreach(var item in tiledObject.properties) {
					worldObject.properties.Add(new() {
						name = item.name,
						value = item.value,
						type = item.type
					});
				}
			if(tiledObject.polygon != null)
				worldObject.polygon = new() {
					points = tiledObject.polygon.points.ToList()
				};
			return worldObject;
		}

		public static T CreateFromTiledObject<T>(TiledObject tiledObject, TiledLayer layer = null, float layerHeight = 0) where T : WorldObject {
			var worldObject = Library.Create<T>();
			worldObject.TileObject = FromTiledObject(tiledObject);
			worldObject.TileObject.Position = worldObject.TileObject.Position.WithZ(layerHeight);
			worldObject.Position = worldObject.TileObject.Position;
			//Log.Info("WTF");
			worldObject.TileObject.Color = Color.Parse(string.Concat("#", tiledObject.GetCustomProperty("DebugColor")?.Substring(3) ?? "ffffff")) ?? Color.Black;
			var depth = tiledObject.GetCustomProperty("Depth")?.ToFloat(10) ?? 0;

			worldObject.TileObject.Size = worldObject.TileObject.Size.WithZ(depth);
			return worldObject;
		}

		public override void Spawn() {
			base.Spawn();
			Transmit = TransmitType.Always;
		}


		public virtual void RebuildObject() {

		}

		public virtual void GenerateObject() {
			if(IsServer) return;


		}
		public virtual void DebugObject() {
			if(!Debug.Enabled) return;
		}

		[ClientRpc]
		public virtual void RpcGenerateObject() {
			GenerateObject();
		}

	}
}
