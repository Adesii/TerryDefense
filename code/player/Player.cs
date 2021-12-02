using Sandbox;
using System;
using System.Linq;

namespace TerryDefense.Player {
	public partial class TerryDefensePlayer : Entity {
		public new TDCamera Camera {
			get => base.Camera as TDCamera;
			set => base.Camera = value;
		}

		public virtual void InitialRespawn() {
			Respawn();
		}

		public void Respawn() {
			Camera = new TDCamera();
			Transmit = TransmitType.Always;

			LifeState = LifeState.Alive;
			Velocity = Vector3.Zero;

		}

		public override void Simulate(Client cl) {
		}

		public override void OnKilled() {

		}
	}
}
