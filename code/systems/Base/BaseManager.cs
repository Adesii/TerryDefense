using System;
using System.Collections.Generic;
using System.Linq;
using Sandbox;
using TerryDefense.entities;

namespace TerryDefense.systems {
	public partial class BaseManager : MechanicManager {
		public static BaseManager Instance => TerryDefenseGame.Instance.GameplayManager as BaseManager;


		[Net] public List<BaseRoom> AllRooms { get; protected set; }

		[Net] BaseRoom HQ { get; set; }
		public override void Destroy() {

		}

		public override void Init() {
			AllRooms = Entity.All.OfType<BaseRoom>().ToList();
			HQ = GetMainRoom(RoomType.HQ);
		}

		public BaseRoom GetMainRoom(RoomType type) {
			var room = AllRooms.First(r => r.Type == type);
			return room;
		}

		public override void Update() {
			if(Host.IsServer) {
				if(HQ.IsValid() && HQ.Models.Count > 0 && HQ.Models[0].IsValid())
					HQ.Models[0].Rotation = HQ.Models[0].Rotation.RotateAroundAxis(Vector3.Up, 0.3f);
			}


		}
	}
}
