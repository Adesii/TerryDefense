using System;
using Sandbox;

namespace TerryDefense.systems {
	public abstract class MechanicManager : BaseNetworkable {
		public abstract void Init();
		public abstract void Update();
		public abstract void Destroy();
	}
}
