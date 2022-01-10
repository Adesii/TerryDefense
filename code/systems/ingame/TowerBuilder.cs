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

		[Event("build_turret")]
		public void EnterBuildMode(string TowerName) {
			Debug.Info($"Entering build mode for {TowerName}");
			TowerPrefabName = TowerName;
			if(TowerGhost == null) {
				TowerGhost = new SceneObject(Model.Load("models/towers/towerghost.vmdl"), Transform.Zero);
			}

			justEnteredBuildMode = true;

		}
		bool justEnteredBuildMode = false;
		[Event.Tick]
		public void Tick() {
			if(Host.IsClient && TowerGhost != null && TowerGhost.IsValid() && !justEnteredBuildMode) {
				Debug.Box(TowerGhost.Position, Vector3.One * -10, Vector3.One * 10, Color.Red, 0f);
				TowerGhost.Position = Trace.Ray(Input.Cursor, 1000).HitLayer(CollisionLayer.All).Run().EndPos;
				if(Input.Pressed(InputButton.Attack2)) {
					TowerGhost.Delete();
					TowerGhost = null;
				}
				if(Input.Pressed(InputButton.Attack1)) {
					CreateTowerAt(TowerGhost.Position, TowerGhost.Rotation, TowerPrefabName);
					TowerGhost.Delete();
					TowerGhost = null;
				}

			} else if(Host.IsClient && TowerGhost != null && TowerGhost.IsValid()) {
				justEnteredBuildMode = false;
			} else if(Host.IsClient && TowerGhost != null && !TowerGhost.IsValid()) {
				TowerGhost = null;
			}
		}
		[ServerCmd]
		public static void CreateTowerAt(Vector3 Position, Rotation Rotation, string TowerName) {
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
