using System;
using Sandbox;
using TerryDefense.UI;

namespace TerryDefense.Player {
	public class BasePlayer : PlayerPawn {
		public new BaseCamera Camera {
			get => base.Camera as BaseCamera;
			set => base.Camera = value;
		}
		public override void Respawn() {
			base.Respawn();
			Camera = new BaseCamera();
		}

		public override void ClientSpawn() {
			base.ClientSpawn();
			TerryDefenseHud.SetNewMainPanel(new BaseHudRoot());
		}

		public override void Simulate(Client cl) {
			base.Simulate(cl);
			//Camera.Position += Vector3.Up * 100 * Time.Delta;
		}

	}
}
