using System;
using Sandbox;
using TerryDefense.systems;

namespace TerryDefense.entities.WorldObjects {
	public class BoxObject : WorldObject {

		public override void RebuildObject() {
			base.RebuildObject();


			_blocker.SetModel(Model.Builder.AddCollisionBox(TileObject.Size / 2, TileObject.Size / 2).Create());
			_blocker.SetupPhysicsFromModel(PhysicsMotionType.Static, true);
		}
		public override void GenerateObject() {
			base.GenerateObject();
			if(TileObject.type == "buildable") return;
			Mesh idks = new Mesh(Material);
			VertexBuffer vb = new();
			vb.AddCube(TileObject.Size / 2, -TileObject.Size.WithZ(TileObject.Size.z), Rotation.Identity);
			idks.CreateBuffers(vb);
			Model idk = Model.Builder.AddMesh(idks).Create();
			_sceneObject = SceneObject.CreateModel(idk, Transform);

		}
		public override void DebugObject() {
			base.DebugObject();
			//Debug.Box(Position, Vector3.One * 50, -Vector3.One * 50, TileObject.Color, true);
			BBox idk = new(TileObject.Position, (TileObject.Position + TileObject.Size));
			//idk += WorldManager.Instance.CurrentMapBounds.Mins.WithX(0);
			//idk += WorldManager.Instance.CurrentMapBounds.Maxs.WithY(0);
			Debug.Box(idk.Mins, idk.Maxs, TileObject.Color);

			Debug.Box(_blocker.WorldSpaceBounds.Mins, _blocker.WorldSpaceBounds.Maxs, Color.Red);
		}
	}
}
