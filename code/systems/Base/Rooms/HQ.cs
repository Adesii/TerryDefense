using System;
using Sandbox;
using TerryDefense.entities;


namespace TerryDefense.systems.Base {
	public class HQ : RoomData {
		public override RoomType RoomType => RoomType.HQ;

		ModelEntity MainRoomPlanet;

		public override void Created() {
			base.Created();

			Log.Info("HQ Created");
			MainRoomPlanet = new("models/earth.vmdl") {
				Position = Room.Position,
				Scale = 7,
				EnableShadowCasting = false,
			};
			MainRoomPlanet.RenderColor = Color.White.WithAlpha(0.9f);
			MainRoomPlanet.SetMaterialGroup("holographic");
			Models.Add(MainRoomPlanet);
		}

		public override void Selected() {
			base.Selected();

		}
		public override void Deselected() {
			base.Deselected();

		}

		public override void Tick() {
			base.Tick();
			if(Host.IsServer && MainRoomPlanet.IsValid()) {
				MainRoomPlanet.Rotation = MainRoomPlanet.Rotation.RotateAroundAxis(Vector3.Up, 0.3f);
			}

		}
	}
}
