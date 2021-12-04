using System.Collections.Generic;
using Sandbox;
using TerryDefense.entities;


namespace TerryDefense.systems.Base {
	public abstract partial class RoomData : BaseNetworkable {

		public virtual RoomType RoomType => RoomType.Empty;

		public BaseRoom Room { get; set; }


		[Net] public List<ModelEntity> Models { get; set; } = new List<ModelEntity>();

		public virtual void Created() {

		}
		public virtual void Tick() {

		}
		public virtual void Selected() {

		}
		public virtual void Deselected() {

		}


	}
}
