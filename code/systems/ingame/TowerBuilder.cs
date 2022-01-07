using System;
using System.Linq;
using Sandbox;
using TerryDefense.Player;
using TerryDefense.Towers;

namespace TerryDefense.systems {
	public partial class TowerBuilder : EntityComponent {

		[Net] BaseTower tower { get; set; }

		SceneObject TowerGhost;
		string TowerPrefabName;

		public void EnterBuildMode(string TowerName) {
			TowerPrefabName = TowerName;
			if(TowerGhost == null)
				TowerGhost = SceneObject.CreateModel("models/towers/towerghost.vmdl");

		}
		[Event.Tick]
		public void Tick() {
			if(Host.IsClient && TowerGhost != null) {
				TowerGhost.Position = Trace.Ray(Input.Cursor, 1000).Run().EndPos;

			}
		}

		public static void CreateTower(Vector3 Position, Rotation Rotation, string TowerName) {
			if(ConsoleSystem.Caller != null && ConsoleSystem.Caller.Pawn is TerryDefensePlayer player) {
				var builttower = Library.Create<BaseTower>();
				builttower.LoadPrefab(TowerName);

				builttower.Position = Position;
				builttower.Rotation = Rotation;
				player.towerBuilder.tower = null;
			}
		}

		[ServerCmd]
		public static void CreateTower(string TowerName) {
			if(ConsoleSystem.Caller != null && ConsoleSystem.Caller.Pawn is TerryDefensePlayer player) {
				var builttower = Library.Create<BaseTower>();
				builttower.LoadPrefab(TowerName);
			}
		}
		[ServerCmd]
		public static void DestroyAllTowers() {
			foreach(var tower in Entity.All.OfType<BaseTower>()) {
				tower.Delete();
			}
		}

	}
}
