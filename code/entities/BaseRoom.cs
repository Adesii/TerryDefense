using System;
using Sandbox;


namespace TerryDefense.entities {
	[Library("td_room")]
	public partial class BaseRoom : Entity {
		[Property, Net] public RoomType Type { get; set; }

		public override void Spawn() {
			base.Spawn();
			Transmit = TransmitType.Always;
		}
		[Event.Tick.Server]
		public void DebugPrint() {
			if(!Debug.Enabled) return;
			Color col = Color.White;
			switch(Type) {
				case RoomType.Empty:
					col = Color.Gray;
					break;
				case RoomType.Collapsed:
					col = Color.Gray * 0.5f;
					break;
				case RoomType.HQ:
					col = Color.Red;
					break;
				case RoomType.Research:
					col = Color.Blue;
					break;
				case RoomType.Armory:
					col = Color.Yellow;
					break;
				case RoomType.Factory:
					col = Color.Green;
					break;
				case RoomType.Intelligence:
					col = Color.Orange;
					break;
			}

			DebugOverlay.Sphere(Position + Vector3.Up * 500, 100f, col);




		}

	}


	public enum RoomType {
		Empty,
		Collapsed,
		HQ,
		Research,
		Armory,
		Factory,
		Intelligence,
	}
}
