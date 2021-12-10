using Sandbox;
using System;
using System.Linq;
using TerryDefense.systems;
using TerryDefense.UI;

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

			WorldManager.InitWorld();
		}

		public override void Simulate(Client cl) {
			base.Simulate(cl);

			if(IsClient) {
				if(Client.DevCamera != null)
					TerryDefenseHud.Instance.RootPanel.AddClass("dev");
				else
					TerryDefenseHud.Instance.RootPanel.RemoveClass("dev");
			}
		}

		public override void ClientSpawn() {
			base.ClientSpawn();
			TerryDefenseHud.SetNewMainPanel(new IngameHudRoot());
		}
	}
}
