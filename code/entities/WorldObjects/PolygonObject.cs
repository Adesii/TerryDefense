using System.Collections.Generic;
using System.Runtime.Intrinsics;
using Sandbox;

namespace TerryDefense.entities.WorldObjects {
	public class PolygonObject : WorldObject {

		public override void RebuildObject() {
			base.RebuildObject();
			var points = new List<Vector3>();
			for(int i = 0; i < TileObject.polygon.points.Count - 1; i += 2) {
				float x = TileObject.polygon.points[i] * -2;
				float y = TileObject.polygon.points[i + 1] * 2;
				points.Add(new Vector3(x, y, 0));
				points.Add(new Vector3(x, y, TileObject.Size.z));
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

			var Bottompoints = new List<Vector2>();
			var Toppoints = new List<Vector2>();

			for(int i = 0; i < TileObject.polygon.points.Count - 1; i += 2) {
				float x = TileObject.polygon.points[i] * -2;
				float y = TileObject.polygon.points[i + 1] * 2;
				Bottompoints.Add(new Vector2(x, y));
				Toppoints.Add(new Vector2(x, y));
			}
			Triangulator.Triangulator.Triangulate(Bottompoints.ToArray(), Triangulator.WindingOrder.CounterClockwise, out var bvertices, out var bindices);
			Triangulator.Triangulator.Triangulate(Toppoints.ToArray(), Triangulator.WindingOrder.CounterClockwise, out var tvertices, out var tindices);

			var points = new List<Vector3>();

			for(int i = 0; i < bvertices.Length; i++) {
				points.Add(bvertices[i]);
				points.Add(new Vector3(tvertices[i].x, tvertices[i].y, TileObject.Size.z));
			}



			VertexBuffer vertices = new();
			vertices.Init(true);
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
			VertexBuffer TopVertices = new();
			TopVertices.Init(true);
			for(int i = 0; i < tvertices.Length; i++) {
				Vector2 item = tvertices[i];
				Vector3 point1T = new Vector3(item.x, item.y, TileObject.Size.z);
				Vertex vertex1T = new(point1T, Vector3.Up, Vector3.Right, new Vector2(0, 0));
				TopVertices.Add(vertex1T);
			}
			for(int i = 0; i < tindices.Length; i++) {
				int item = tindices[i];
				TopVertices.AddRawIndex(item);
			}

			Mesh idk = new(Material);
			idk.CreateBuffers(vertices);

			Mesh top = new(Material);
			top.CreateBuffers(TopVertices);
			//idk.CreateIndexBuffer(indices.Count, indices);

			_sceneObject = SceneObject.CreateModel(Model.Builder.AddMesh(idk).AddMesh(top).Create(), Transform);
		}
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
