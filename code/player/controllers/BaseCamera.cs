using System;
using System.Numerics;
using Sandbox;
using TerryDefense.systems;

namespace TerryDefense.Player {
	public class BaseCamera : Camera {
		public float ZoomLevel { get; set; }
		public Vector3 LookAt;
		public float Speed => 300f;
		public Vector3 Velocity;
		public override void Activated() {
			var cameraConfig = Config.Current.Camera;

			FieldOfView = cameraConfig.FOV;
			ZoomLevel = 1f;
			ZNear = cameraConfig.ZNear;
			ZFar = cameraConfig.ZFar;
			Ortho = cameraConfig.Ortho;

			base.Activated();
		}
		public override void Update() {
			if(Local.Pawn is not BasePlayer player) return;

			Velocity = Vector3.Zero;

			Velocity.z += Input.Forward;
			Velocity.y += Input.Left;


			if(Input.MouseWheel != 0) {
				ZoomLevel -= Input.MouseWheel * 0.1f;
				ZoomLevel = ZoomLevel.Clamp(1f, 8f);
			}
			Velocity *= Speed * (ZoomLevel * 0.5f);
			if(Input.Down(InputButton.Run)) {
				Velocity *= 2f;
			}

			LookAt = LookAt.LerpTo(LookAt + Velocity, 5 * Time.Delta);
			LookAt.x = -1000 * ZoomLevel;

			Position = Position.LerpTo(LookAt, 4 * Time.Delta);
		}
	}
}
