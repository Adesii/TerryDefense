using Sandbox;

namespace TerryDefense {
	public static partial class Debug {

		[ConVar.Replicated("debug_enable")]
		public static bool Enabled { get; set; }

		public static void Info(object obj) {
			if(!Debug.Enabled) return;

			Sandbox.Internal.GlobalGameNamespace.Log.Info($"[{(Host.IsClient ? "CL" : "SV")}] {obj}");
		}
		public static void Error(object obj) {
			if(!Debug.Enabled) return;

			Sandbox.Internal.GlobalGameNamespace.Log.Error($"[{(Host.IsClient ? "CL" : "SV")}] {obj}");
		}

		public static void TraceResult(TraceResult traceResult, float duration = 0) {
			if(!Debug.Enabled) return;
			DebugOverlay.TraceResult(traceResult, duration);
		}

		public static void Sphere(Vector3 position, float radius, Color color = default, float duration = 0, bool depthTest = true) {
			if(!Debug.Enabled) return;
			DebugOverlay.Sphere(position, radius, color, depthTest, duration);
		}

		public static void Line(Vector3 start, Vector3 end, Color color = default, float duration = 0, bool depthTest = true) {
			if(!Debug.Enabled) return;
			DebugOverlay.Line(start, end, color, duration, depthTest);
		}

		public static void Axis(Vector3 position, Rotation rotation, float length = 10, float duration = 0, bool depthTest = true) {
			if(!Debug.Enabled) return;
			DebugOverlay.Axis(position, rotation, length, duration, depthTest);
		}

		public static void Skeleton(Entity ent, Color color, float duration = 0, bool depthTest = true) {
			if(!Debug.Enabled) return;
			DebugOverlay.Skeleton(ent, color, duration, depthTest);
		}

		public static void Circle(Vector3 position, Rotation rotation, float radius, Color color = default, float duration = 0, bool depthTest = true) {
			if(!Debug.Enabled) return;
			DebugOverlay.Circle(position, rotation, radius, color, depthTest, duration);
		}

		public static void WorldText(Vector3 position, string text, Color color = default, float duration = 0, float maxDistance = 500, int offset = 0) {
			if(!Debug.Enabled) return;
			DebugOverlay.Text(position, offset, text, color, duration, maxDistance);
		}

		public static void ScreenText(string text, float duration = 0) {
			if(!Debug.Enabled) return;
			DebugOverlay.ScreenText(text, duration);
		}

		public static void ScreenText(string text, int line = 0, float duration = 0) {
			if(!Debug.Enabled) return;
			DebugOverlay.ScreenText(line, text, duration);
		}

		public static void ScreenText(Vector2 position, string text, Color color = default, float duration = 0) {
			if(!Debug.Enabled) return;
			DebugOverlay.ScreenText(position, 0, color, text, duration);
		}

		public static void ScreenText(Vector2 position, string text, int line, Color color = default, float duration = 0) {
			if(!Debug.Enabled) return;
			DebugOverlay.ScreenText(position, line, color, text, duration);
		}

		public static void Box(Vector3 mins, Vector3 maxs) {
			if(!Debug.Enabled) return;
			DebugOverlay.Box(mins, maxs);
		}

		public static void Box(Entity ent, Color color = default, float duration = 0) {
			if(!Debug.Enabled) return;
			DebugOverlay.Box(ent, color, duration);
		}

		public static void Box(Vector3 position, Vector3 mins, Vector3 maxs, Color color = default, bool depthTest = true) {
			if(!Debug.Enabled) return;
			DebugOverlay.Box(position, mins, maxs, color, depthTest);
		}

		public static void Box(Vector3 mins, Vector3 maxs, Color color = default, float duration = 0, bool depthTest = true) {
			if(!Debug.Enabled) return;
			DebugOverlay.Box(mins, maxs, color, duration, depthTest);
		}

		public static void Box(Vector3 position, Vector3 mins, Vector3 maxs, Color color = default, float duration = 0, bool depthTest = true) {
			if(!Debug.Enabled) return;
			DebugOverlay.Box(position, mins, maxs, color, duration, depthTest);
		}

		public static void Box(Vector3 position, Rotation rotation, Vector3 mins, Vector3 maxs, Color color = default, float duration = 0, bool depthTest = true) {
			if(!Debug.Enabled) return;
			DebugOverlay.Box(position, rotation, mins, maxs, color, duration, depthTest);
		}

		public static void Ellipse(Vector3 position, Vector3 size, Color color = default, float duration = 0, bool depthTest = true) {
			var points = new System.Collections.Generic.List<Vector3>();
			var radiusx = (int)(size.x / 2);
			var radiusy = (int)(size.y / 2);
			for(var i = 0; i < 360; i += 32) {
				var x = (int)(radiusx * System.MathF.Cos(i * System.MathF.PI / 180)) * -1;
				var y = (int)(radiusy * System.MathF.Sin(i * System.MathF.PI / 180));
				points.Add(new Vector3(x, y) + position + new Vector3(radiusx, radiusy));
			}
			int iterations = (size.z == 0f) ? 1 : 2;
			for(int y = 0; y < iterations; y++) {
				for(int i = 0; i < points.Count; i++) {
					if(i == points.Count - 1) {
						Debug.Line(new Vector3(points[i].x, points[i].y, size.z * y), new Vector3(points[0].x, points[0].y, size.z * y), color, duration, depthTest);
					} else {
						Debug.Line(new Vector3(points[i].x, points[i].y, size.z * y), new Vector3(points[i + 1].x, points[i + 1].y, size.z * y), color, duration, depthTest);
					}

				}
			}
			if(iterations > 1)
				foreach(var item in points) {
					Debug.Line(new Vector3(item.x, item.y, 0), new Vector3(item.x, item.y, size.z), color, duration, depthTest);
				}
		}
		public static void Polygon(System.Collections.Generic.List<Vector3> points, Color color = default, float duration = 0, bool depthTest = true) {
			for(int y = 0; y < 2; y++) {
				for(int i = 0; i < points.Count; i++) {
					if(i == points.Count - 1) {
						Debug.Line(new Vector3(points[i].x, points[i].y, points[i].z * y), new Vector3(points[0].x, points[0].y, points[0].z * y), color, duration, depthTest);
					} else {
						Debug.Line(new Vector3(points[i].x, points[i].y, points[i].z * y), new Vector3(points[i + 1].x, points[i + 1].y, points[i + 1].z * y), color, duration, depthTest);
					}

				}
			}
			foreach(var item in points) {
				Debug.Line(new Vector3(item.x, item.y, 0), new Vector3(item.x, item.y, item.z), color, duration, depthTest);
			}
		}
	}
}
