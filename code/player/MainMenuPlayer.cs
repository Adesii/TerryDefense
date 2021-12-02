using System;
using Sandbox;
using TerryDefense.player.controllers;
using TerryDefense.UI;

namespace TerryDefense.Player {
	public class MainMenuPlayer : Entity {

		public void Respawn() {
			Camera = new MenuCamera();
			Transmit = TransmitType.Always;

			LifeState = LifeState.Alive;
			Velocity = Vector3.Zero;
		}
		public override void ClientSpawn() {
			base.ClientSpawn();
			TerryDefenseHud.SetNewMainPanel(new MainMenuRoot());
		}

	}
}
