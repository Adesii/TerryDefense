using System;
using Hammer;
using Sandbox;


namespace TerryDefense.entities {
	[Library("td_room"), BoundsHelper(nameof(MinBounds), nameof(MaxBounds), true, true)]
	public partial class BaseRoom : ModelEntity {
		[Property, Net] public RoomType Type { get; set; }
		[Property, Net] public Vector3 MinBounds { get; set; } = new Vector3(-100);
		[Property, Net] public Vector3 MaxBounds { get; set; } = new Vector3(100);

		public override void Spawn() {
			base.Spawn();
			Transmit = TransmitType.Always;
			SetupPhysicsFromAABB(PhysicsMotionType.Static, MinBounds, MaxBounds);
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

			DebugOverlay.Sphere(Position, 100f, col);
			DebugOverlay.Box(WorldSpaceBounds.Mins + 10, WorldSpaceBounds.Maxs - 10, col);




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
