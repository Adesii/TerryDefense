using System;
using Sandbox;


namespace TerryDefense.systems {
	public partial class WorldManager : BaseNetworkable {
		public static WorldManager Instance => TerryDefenseGame.Instance.WorldManager;
		public WorldData CurrentWorld { get; protected set; }


		public static void LoadWorld(WorldData world) {
			Instance.CurrentWorld = world;
			SaveSystem.Save();
			Global.Lobby.SetData("Switching", "true");
			Global.Lobby.SetData("SaveFile", SaveSystem.SaveFile.SaveGameName);

			if(Instance.CurrentWorld.MapFile != Global.MapName) {
				ChangeWorld(Instance.CurrentWorld.MapFile);
			}
		}
		[ServerCmd]
		public static void ChangeWorld(string file) {
			Global.ChangeLevel(file);
		}
	}

	public partial class WorldData : BaseNetworkable {
		public BBox WorldSize { get; set; } = new BBox(Vector3.One * -5000f, Vector3.One * 5000f);
		public string MapFile { get; set; } = "empty";
		public string TileFile { get; set; } = "";

	}

	public static class TemplateWorldData {
		public static WorldData BaseWorld => new() {
			MapFile = "base",
			TileFile = "base",
			WorldSize = new BBox(0, 0)
		};
	}
}
