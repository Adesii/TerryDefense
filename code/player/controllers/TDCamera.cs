using System;
using Sandbox;
using TerryDefense.systems;

namespace TerryDefense.Player {
	public partial class TDCamera : Camera {
		public float ZoomLevel { get; set; }
		public Transform LookAt;
		public Vector3 CameraOffsetPosition;
		public Vector3 CameraOffset => new Vector3(-50, 0, 100);
		public float Speed => 50f;
		public Vector3 Velocity;

		Particles _particleSystem;

		public override void Activated() {
			var cameraConfig = Config.Current.Camera;

			FieldOfView = cameraConfig.FOV;
			ZoomLevel = 5f;
			ZNear = cameraConfig.ZNear;
			ZFar = cameraConfig.ZFar;
			Ortho = cameraConfig.Ortho;
			LookAt = new() {
				Rotation = Rotation.FromAxis(Vector3.Up, 0.01f),
			};

			_particleSystem = Particles.Create("particles/environment/rain/rain_main.vpcf");


			base.Activated();
		}
		public void ZoomBy(float amount) {
			ZoomLevel -= amount * 0.5f;
			ZoomLevel = ZoomLevel.Clamp(1f, 100f);
		}
		public override void Update() {
			if(Local.Pawn is not TerryDefensePlayer player) return;


			Velocity = Vector3.Zero;

			Velocity.x += Input.Forward;
			Velocity.y += Input.Left;

			Velocity *= Speed * (ZoomLevel * 0.25f);
			Velocity *= LookAt.Rotation;
			if(Input.Down(InputButton.Run)) {
				Velocity *= 2f;
			}
			if(Input.Down(InputButton.Use)) {
				LookAt.Rotation = LookAt.Rotation.RotateAroundAxis(Vector3.Up, 100f * Time.Delta);
			}
			if(Input.Down(InputButton.Menu)) {
				LookAt.Rotation = LookAt.Rotation.RotateAroundAxis(Vector3.Up, -100f * Time.Delta);
			}

			LookAt.Position = LookAt.Position.LerpTo(LookAt.Position + Velocity.WithZ(0), 5 * Time.Delta);
			CameraOffsetPosition = CameraOffset;
			CameraOffsetPosition *= 1.2f * ZoomLevel;
			CameraOffsetPosition.z = 50 * ZoomLevel;

			Position = Position.LerpTo(LookAt.PointToWorld(CameraOffsetPosition), 10 * Time.Delta);

			Rotation = Rotation.LookAt(LookAt.Position - Position, Vector3.Up);

			_particleSystem?.SetPosition(0, Position + Velocity);
		}

	}
}
