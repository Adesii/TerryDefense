using System;
using System.Collections.Generic;
using Hammer;
using Sandbox;
using TerryDefense.systems.Base;

namespace TerryDefense.entities {
	[Library("td_room"), BoundsHelper(nameof(MinBounds), nameof(MaxBounds), true, true)]
	public partial class BaseRoom : ModelEntity {
		[Property, Net] public RoomType Type { get; set; }
		[Property, Net] public Vector3 MinBounds { get; set; } = new Vector3(-100);
		[Property, Net] public Vector3 MaxBounds { get; set; } = new Vector3(100);

		[Net] public RoomData RoomData { get; set; }

		public Transform? CameraTransform {
			get {
				return GetAttachment("m_camera");
			}
		}

		public override void Spawn() {
			base.Spawn();
			Transmit = TransmitType.Always;
			SetupPhysicsFromAABB(PhysicsMotionType.Static, MinBounds, MaxBounds);
			switch(Type) {
				case RoomType.Empty:
					SetModel("models/rooms/empty.vmdl");
					break;
				case RoomType.Collapsed:
					break;
				case RoomType.HQ:
					SetModel("models/rooms/hq.vmdl");
					RoomData = new HQ();
					break;
				case RoomType.Research:
					SetModel("models/rooms/research.vmdl");
					RoomData = new Research();
					break;
				case RoomType.Armory:
					SetModel("models/rooms/armory.vmdl");
					RoomData = new Armory();
					break;
				case RoomType.Factory:
					SetModel("models/rooms/factory.vmdl");
					RoomData = new Factory();
					break;
				case RoomType.Intelligence:
					SetModel("models/rooms/empty.vmdl");
					break;
				default:
					break;
			}
			if(RoomData != null) {
				RoomData.Room = this;
				RoomData.Created();
			}
		}
		[Event.Tick]
		public void RoomTick() {
			if(RoomData != null) {
				RoomData.Tick();
			}
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

