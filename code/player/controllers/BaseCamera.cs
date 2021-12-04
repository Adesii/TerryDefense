
using Sandbox;
using TerryDefense.systems;
using TerryDefense.UI;

namespace TerryDefense.Player {
	public class BaseCamera : Camera {
		public float ZoomLevel { get; set; }
		public Vector3 LookAt;
		public Rotation LookAtRotation = Rotation.FromAxis(Vector3.Forward, 0);
		public float Speed => 300f;
		public Vector3 Velocity;

		private bool Locked;
		public override void Activated() {
			var cameraConfig = Config.Current.Camera;

			FieldOfView = cameraConfig.FOV;
			ZoomLevel = 1f;
			ZNear = cameraConfig.ZNear;
			ZFar = cameraConfig.ZFar;
			Ortho = cameraConfig.Ortho;

			base.Activated();
		}
		public void ZoomBy(float amount) {
			ZoomLevel -= amount * 0.3f;
			ZoomLevel = ZoomLevel.Clamp(1f, 8f);
		}
		public override void Update() {
			if(Local.Pawn is not BasePlayer player) return;
			if(Locked) {
				Position = Position.LerpTo(LookAt.WithX(Position.x), 7 * Time.Delta);
				Position = Position.LerpTo(LookAt, 4 * Time.Delta);
				Rotation = Rotation.Lerp(Rotation, LookAtRotation, 4 * Time.Delta);
				return;
			}

			Velocity = Vector3.Zero;

			Velocity.z += Input.Forward;
			Velocity.y += Input.Left;

			Velocity *= Speed * (ZoomLevel * 0.5f);
			if(Input.Down(InputButton.Run)) {
				Velocity *= 2f;
			}

			LookAt = LookAt.LerpTo(LookAt + Velocity, 5 * Time.Delta);
			LookAt.x = -1000 * ZoomLevel;

			Position = Position.LerpTo(LookAt, 4 * Time.Delta);
			Rotation = Rotation.Lerp(Rotation, LookAtRotation, 9 * Time.Delta);
		}

		public void SetTarget(Transform target, bool shouldTeleport = false) {
			LookAt = target.Position;
			LookAtRotation = target.Rotation;

			if(shouldTeleport) {
				Position = LookAt + LookAtRotation.Backward * 250f;
				Rotation = Rotation.Lerp(Rotation, LookAtRotation, 0.7f);
			}


			Locked = true;

		}
		public void ClearTarget() {
			LookAtRotation = Rotation.FromAxis(Vector3.Forward, 0);
			Locked = false;
		}
	}
}
