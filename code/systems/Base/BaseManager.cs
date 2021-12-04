using System;
using System.Collections.Generic;
using System.Linq;
using Sandbox;
using TerryDefense.entities;

namespace TerryDefense.systems {
	public partial class BaseManager : MechanicManager {
		public static BaseManager Instance => TerryDefenseGame.Instance.GameplayManager as BaseManager;


		[Net] public List<BaseRoom> AllRooms { get; protected set; }

		public override void Destroy() {

		}

		public override void Init() {
			AllRooms = Entity.All.OfType<BaseRoom>().ToList();
		}

		public BaseRoom GetMainRoom(RoomType type) {
			var room = AllRooms.First(r => r.Type == type);
			return room;
		}

		public override void Update() {



		}
	}
}
