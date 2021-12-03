using Sandbox;
using System;
using System.Linq;

namespace TerryDefense.Player {
	public partial class TerryDefensePlayer : PlayerPawn {
		public new TDCamera Camera {
			get => base.Camera as TDCamera;
			set => base.Camera = value;
		}

		public override void Respawn() {
			Camera = new TDCamera();
			Transmit = TransmitType.Always;

			LifeState = LifeState.Alive;
			Velocity = Vector3.Zero;

		}

		public override void Simulate(Client cl) {
			Debug.Log("Simulate");
		}

		public override void OnKilled() {

		}
	}
}
