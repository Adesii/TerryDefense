using System;
using System.Collections.Generic;
using Sandbox;
using TiledCS;

namespace TerryDefense.entities.WorldObjects {
	public class ElipseObject : WorldObject {

		public override void RebuildObject() {
			base.RebuildObject();

			var points = new System.Collections.Generic.List<Vector3>();
			var radiusx = (int)(TileObject.Size.x / 2);
			var radiusy = (int)(TileObject.Size.y / 2);
			for(var i = 0; i < 360; i += 32) {
				var x = (int)(radiusx * System.MathF.Cos(i * System.MathF.PI / 180)) * -1;
				var y = (int)(radiusy * System.MathF.Sin(i * System.MathF.PI / 180));
				points.Add(new Vector3(x, y) + new Vector3(radiusx, radiusy));
				points.Add(new Vector3(x, y, TileObject.Size.z) + new Vector3(radiusx, radiusy));
			}

			var indices = new List<int>();
			//calculate the indices on a polygonal 3d mesh from its points
			for(int i = 0; i < points.Count; i++) {
				indices.Add(i);
				indices.Add((i + 1) % points.Count);
				indices.Add((i + 2) % points.Count);
			}
			_blocker.SetModel(Model.Builder.AddCollisionMesh(points.ToArray(), indices.ToArray()).Create());
			_blocker.SetupPhysicsFromModel(PhysicsMotionType.Static, true);
		}

		public override void GenerateObject() {
			base.GenerateObject();
			var points = new System.Collections.Generic.List<Vector3>();
			var radiusx = (int)(TileObject.Size.x / 2);
			var radiusy = (int)(TileObject.Size.y / 2);
			for(var i = 0; i < 360; i += 32) {
				var x = (int)(radiusx * System.MathF.Cos(i * System.MathF.PI / 180)) * -1;
				var y = (int)(radiusy * System.MathF.Sin(i * System.MathF.PI / 180));
				points.Add(new Vector3(x, y) + new Vector3(radiusx, radiusy));
				points.Add(new Vector3(x, y, TileObject.Size.z) + new Vector3(radiusx, radiusy));
			}

			//var indices = new List<int>();
			////calculate the indices on a polygonal 3d mesh from its points
			//for(int i = 0; i < points.Count; i++) {
			//	indices.Add(i);
			//	indices.Add((i + 1) % points.Count);
			//	indices.Add((i + 2) % points.Count);
			//}
			VertexBuffer vertices = new();
			for(int i = 0; i < points.Count; i += 2) {
				Vector3 point1 = points[i];
				Vector3 point2 = points[(i + 1) % points.Count];
				Vector3 point3 = points[(i + 2) % points.Count];
				Vector3 point4 = points[(i + 3) % points.Count];

				Vertex vertex1 = new(point1, Vector3.Up, Vector3.Right, new Vector2(0, 0));
				Vertex vertex2 = new(point2, Vector3.Up, Vector3.Right, new Vector2(1, 0));
				Vertex vertex3 = new(point3, Vector3.Up, Vector3.Right, new Vector2(1, 1));
				Vertex vertex4 = new(point4, Vector3.Up, Vector3.Right, new Vector2(0, 1));
				Vector3 facenomral = CalculateNormal(vertex1, vertex2, vertex3);
				vertex1.Normal = facenomral;
				vertex2.Normal = facenomral;
				vertex3.Normal = facenomral;
				vertex4.Normal = facenomral;
				vertices.AddTriangle(vertex3, vertex4, vertex2);
				vertices.AddTriangle(vertex3, vertex2, vertex1);
			}
			for(int i = 3; i < points.Count; i += 2) {
				Vector3 point1 = points[1];
				Vector3 point2 = points[(i - 2)];
				Vector3 point3 = points[(i)];

				Vertex vertex1 = new(point1, Vector3.Up, Vector3.Right, new Vector2(0, 0));
				Vertex vertex2 = new(point2, Vector3.Up, Vector3.Right, new Vector2(1, 0));
				Vertex vertex3 = new(point3, Vector3.Up, Vector3.Right, new Vector2(1, 1));
				Vector3 facenomral = CalculateNormal(vertex1, vertex2, vertex3);
				vertex1.Normal = facenomral;
				vertex2.Normal = facenomral;
				vertex3.Normal = facenomral;
				vertices.AddTriangle(vertex1, vertex2, vertex3);
			}

			Mesh idk = new(Material);
			idk.CreateBuffers(vertices);
			//idk.CreateIndexBuffer(indices.Count, indices);
			_sceneObject = SceneObject.CreateModel(Model.Builder.AddMesh(idk).Create(), Transform);
		}
		public override void DebugObject() {
			base.DebugObject();
			//Create a list of points that make up the elipse
			Debug.Ellipse(Position, TileObject.Size, TileObject.Color);


		}
	}
}
