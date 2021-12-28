using System;
using Sandbox;
using TerryDefense.Player;
using TerryDefense.Towers;

namespace TerryDefense.systems {
	public partial class TowerBuilder : BaseNetworkable {

		[Net] BaseTower tower { get; set; }

		SceneObject TowerGhost;
		string TowerLibraryName;

		public void EnterBuildMode(string TowerName) {
			TowerLibraryName = TowerName;
			if(TowerGhost == null)
				TowerGhost = SceneObject.CreateModel("models/towers/towerghost.vmdl");

		}

		public static void CreateTower(Vector3 Position, Rotation Rotation, string TowerName) {
			if(ConsoleSystem.Caller != null && ConsoleSystem.Caller.Pawn is TerryDefensePlayer player) {
				var builttower = Library.Create<BaseTower>(TowerName);
				builttower.Position = Position;
				builttower.Rotation = Rotation;
				player.towerBuilder.tower = null;
			}

		}

	}
}
