using System;
using Sandbox;


namespace TerryDefense.Player {
	public class PlayerPawn : Entity {

		public PlayerPawn() {
			Transmit = TransmitType.Always;
		}

		public virtual void Respawn() {
			Transmit = TransmitType.Always;
		}

		public override void Simulate(Client cl) {
			base.Simulate(cl);
		}

		public override void OnKilled() {

		}

	}
}
