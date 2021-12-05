using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
				float Currenttheight = 0;
				for(int i = 0; i < Instance.CurrentMap.Layers.Length; i++) {
					TiledLayer layer = Instance.CurrentMap.Layers[i];
					if(layer.name == "blockers" || layer.name == "buildable") {
						float objectheight = 0f;
						if(layer.properties != null) {
							var depth = layer.properties.Where(x => x.name == "depth").FirstOrDefault()?.value?.ToFloat(10) ?? 0;
							objectheight = depth;
						}

						foreach(var obj in layer.objects) {
							TestBlockers.Add(new() {
								Size = new Vector3(obj.width * 2, obj.height * 2, objectheight),
								Position = new Vector3(obj.x * 2, obj.y * 2, Currenttheight),
								col = string.IsNullOrEmpty(layer.tintcolor) ? Color.Red : layer.tintcolor
							});
						}
						Currenttheight += objectheight;
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
				var idkk = (idk.Mins - idk.Maxs).z.AlmostEqual(0f) ? idk.Maxs + new Vector3(0, 0, 4f) : idk.Maxs;
				var mins = idk.Mins + new Vector3(0, 0, 1f);
				Debug.Box(mins, idkk, blocker.col);
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
