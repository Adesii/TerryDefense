using System;
using Sandbox;


namespace TerryDefense.systems {
	public partial class WorldManager : BaseNetworkable {
		[Net] public BBox WorldSize { get; private set; } = new BBox(Vector3.One * -5000f, Vector3.One * 5000f);

	}
}
