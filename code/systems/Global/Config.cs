using System;
using Gamelib.Network;
using Sandbox;

namespace TerryDefense.systems {
	public struct CameraConfig {
		public bool Ortho;
		public float PanSpeed;
		public float ZoomScale;
		public float ZNear;
		public float ZFar;
		public float FOV;
		public float Backward;
		public float Left;
		public float Up;
	}

	public struct GameConfig {
		public CameraConfig Camera;
	}

	public static partial class Config {

		public static GameConfig CurrentConfig { get; set; }

		public static GameConfig Default => new() {
			Camera = new CameraConfig {
				Ortho = false,
				PanSpeed = 5000f,
				ZoomScale = 0.6f,
				FOV = 30f,
				ZNear = 1000f,
				ZFar = 10000f,
				Backward = 2500f,
				Left = 2500f,
				Up = 5000f
			}
		};

		public static GameConfig Current => CurrentConfig;
	}
}
