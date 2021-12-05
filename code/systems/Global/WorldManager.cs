using System;
using System.Collections.Generic;
using System.IO;
using Gamelib.Extensions;
using Gamelib.FlowFields;
using Sandbox;
using TiledCS;

namespace TerryDefense.systems {
	public partial class WorldManager : BaseNetworkable {
		public static WorldManager Instance => TerryDefenseGame.Instance.WorldManager;
		public WorldData CurrentWorld { get; protected set; }

		public TiledMap CurrentMap { get; protected set; }

		public BBox CurrentMapBounds { get; protected set; }

		public static List<blocker> TestBlockers { get; protected set; } = new();


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

		public static void InitWorld() {
			if(Instance.CurrentWorld == null) {
				if(SaveSystem.SaveFile == null && !SaveSystem.ReloadSave()) return;
				Instance.CurrentWorld = SaveSystem.SaveFile.WorldData;
			}
			if(Instance.CurrentMap == null) {
				Instance.CurrentMap = new TiledMap("data/maps/" + Instance.CurrentWorld.TileFile + ".tmx");
				Vector3 tempsize = new(Instance.CurrentMap.Width * Instance.CurrentMap.TileWidth, Instance.CurrentMap.Height * Instance.CurrentMap.TileHeight, 0);
				Instance.CurrentMapBounds = new BBox(-tempsize, tempsize);
			}
			PathManager.SetBounds(Instance.CurrentMapBounds);
			if(Instance.CurrentMap.Layers != null) {
				for(int i = 0; i < Instance.CurrentMap.Layers.Length; i++) {
					TiledLayer layer = Instance.CurrentMap.Layers[i];
					if(layer.name == "blockers" || layer.name == "buildable") {
						foreach(var obj in layer.objects) {
							TestBlockers.Add(new() {
								Position = new Vector3(obj.x * 2, obj.y * 2, (layer.name == "blockers" ? 0 : 65)),
								Size = new Vector3(obj.width * 2, obj.height * 2, layer.name == "blockers" ? 64 : 8),
								col = layer.name == "blockers" ? new(1, 0, 0) : new(0, 1, 0)
							});
						}
					}
				}
			}
		}
		[Event.Hotload]
		public static void OnHotload() {
			if(Instance.CurrentWorld == null) return;
			Debug.Info("Hotloading world " + Instance.CurrentWorld);
			TestBlockers.Clear();
			Instance.CurrentMap = null;
			InitWorld();
		}
		[Event.Tick]
		public static void ShowBlocker() {
			if(Instance == null) return;
			Debug.Box(Instance.CurrentMapBounds.Mins, Instance.CurrentMapBounds.Maxs, Color.Green);
			foreach(var blocker in TestBlockers) {
				BBox idk = new(blocker.Position * new Vector3(-1, 1, 1), (blocker.Position + blocker.Size) * new Vector3(-1, 1, 1));
				idk += Instance.CurrentMapBounds.Mins.WithX(0);
				idk += Instance.CurrentMapBounds.Maxs.WithY(0);
				Debug.Box(idk.Mins, idk.Maxs, blocker.col);
			}
		}

		public struct blocker {
			public Vector3 Position;
			public Vector3 Size;
			public Color col;
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
