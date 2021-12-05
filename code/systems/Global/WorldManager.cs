using System;
using System.IO;
using Sandbox;


namespace TerryDefense.systems {
	public partial class WorldManager : BaseNetworkable {
		public static WorldManager Instance => TerryDefenseGame.Instance.WorldManager;
		public WorldData CurrentWorld { get; protected set; }

		public BBox WorldSize { get; set; } = new BBox(new Vector3(-1000, -1000, -1000), new Vector3(1000, 1000, 1000));


		public static void LoadWorld(WorldData world) {
			Instance.CurrentWorld = world;
			Global.Lobby.SetData("Switching", "true");
			Global.Lobby.SetData("SaveFile", SaveSystem.SaveFile.saveid.ToString());

			if(Instance.CurrentWorld.MapFile != Global.MapName && !string.IsNullOrEmpty(Instance.CurrentWorld.MapFile)) {
				SaveSystem.Save();
				ChangeWorld(Instance.CurrentWorld.MapFile);
			}
		}
		[ServerCmd]
		public static void ChangeWorld(string file) {
			string sterl = StringX.NormalizeFilename(file);
			sterl = Path.GetFileNameWithoutExtension(sterl);
			Global.ChangeLevel(sterl);
		}
	}
	[System.Serializable]
	public partial class WorldData {
		public string MapFile { get; set; } = "empty";
		public string TileFile { get; set; } = "";

	}

	public static class TemplateWorldData {
		public static WorldData BaseWorld => new() {
			MapFile = "base",
			TileFile = "base"
		};
	}
}
