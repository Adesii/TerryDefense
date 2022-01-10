using System;
using System.Linq;
using System.Threading.Tasks;
using Gamelib.FlowFields.Entities;
using Sandbox;
using TiledCS;

namespace TerryDefense.entities {
	[Library("world_object")]
	public partial class WorldObject : Entity {
		[Net] public NetworkedTiledObject TileObject { get; set; }

		[Net] protected FlowFieldBlocker _blocker { get; set; }

		[Net] public Material Material { get; set; } = Material.Load("materials/cliffside.vmat");

		protected SceneObject _sceneObject;

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
			worldObject.TileObject.Position = worldObject.TileObject.Position.WithZ(layerHeight + tiledObject.GetCustomProperty("z").ToFloat());
			worldObject.Position = worldObject.TileObject.Position;
			//Log.Info("WTF");
			worldObject.TileObject.Color = Color.Parse(string.Concat("#", tiledObject.GetCustomProperty("DebugColor")?.Substring(3) ?? "ffffff")) ?? Color.Black;
			var depth = tiledObject.GetCustomProperty("Depth")?.ToFloat(10) ?? 0f;
			worldObject.Material = Material.Load(tiledObject.GetCustomProperty("CustomMaterial") ?? "materials/cliffside.vmat");

			worldObject.TileObject.BottomWidth = tiledObject.GetCustomProperty("BottomWidth")?.ToFloat(0) ?? 0f;

			worldObject.TileObject.Size = worldObject.TileObject.Size.WithZ(depth.AlmostEqual(0) ? 10 : depth);
			worldObject.Rotation = Rotation.FromAxis(Vector3.Up, -worldObject.TileObject.Rotation);
			return worldObject;
		}

		public override void Spawn() {
			base.Spawn();
			Transmit = TransmitType.Always;
			Tags.Add("ff_ignore");

			_blocker?.SetInteractsAs(CollisionLayer.PLAYER_CLIP);
			_blocker?.SetupPhysicsFromModel(PhysicsMotionType.Static, true);
			if(_blocker != null)
				_blocker.Transmit = TransmitType.Always;
		}

		protected override void OnDestroy() {
			base.OnDestroy();
			if(Host.IsClient)
				return;
			_blocker?.Delete();
			_sceneObject?.Delete();
		}
		~WorldObject() {
			_blocker?.Delete();
			_sceneObject?.Delete();
		}


		public virtual void RebuildObject() {
			//Host.AssertServer();
			_blocker = new() {
				Position = Position,
				Rotation = Rotation,
				Parent = this,
			};
			if(Host.IsServer)
				RpcGenerateObject();
		}

		public virtual void GenerateObject() {
			//if(IsServer || TileObject.type == TileObjectTypes.Buildable) return;
			//Log.Error(TileObject.type);


		}
		public virtual void DebugObject() {
			if(!Debug.Enabled) return;
		}

		[ClientRpc]
		public virtual void RpcGenerateObject() {
			GenerateObject();
			RebuildObject();
		}


		public static Vector3 CalculateNormal(Vertex v1, Vertex v2, Vertex v3) {
			Vector3 u = v2.Position - v1.Position;
			Vector3 v = v3.Position - v1.Position;

			return new Vector3 {
				x = (u.y * v.z) - (u.z * v.y),
				y = (u.z * v.x) - (u.x * v.z),
				z = (u.x * v.y) - (u.y * v.x)
			};
		}

		public static Vector4 CalculateTangent(SimpleVertex v1, SimpleVertex v2, SimpleVertex v3) {
			Vector4 u = v2.texcoord - v1.texcoord;
			Vector4 v = v3.texcoord - v1.texcoord;

			float r = 1.0f / (u.x * v.y - u.y * v.x);
			Vector3 tu = v2.position - v1.position;
			Vector3 vu = v3.position - v1.position;

			return new Vector4((tu * v.y - vu * u.y) * r, 1.0f);
		}

	}
}
