using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Gamelib.Extensions;
using Gamelib.FlowFields;
using Sandbox;
using TerryDefense.entities;
using TerryDefense.entities.WorldObjects;
using TiledCS;

namespace TerryDefense.systems {
	public partial class WorldManager : BaseNetworkable {
		public static WorldManager Instance => TerryDefenseGame.Instance.WorldManager;
		public WorldData CurrentWorld { get; protected set; }

		public TiledMap CurrentMap { get; protected set; }

		public BBox CurrentMapBounds { get; protected set; }

		[Net] public List<WorldObject> WorldObjects { get; protected set; } = new();


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
				float objectheight = 0f;
				for(int i = 0; i < Instance.CurrentMap.Layers.Length; i++) {
					TiledLayer layer = Instance.CurrentMap.Layers[i];
					foreach(var obj in layer.objects) {
						WorldObject wobj = null;
						if(obj.polygon != null) {
							wobj = WorldObject.CreateFromTiledObject<PolygonObject>(obj, layer, objectheight);
						} else if(obj.point != null) {
							wobj = WorldObject.CreateFromTiledObject<PointObject>(obj, layer, objectheight);
						} else if(obj.ellipse != null) {
							wobj = WorldObject.CreateFromTiledObject<ElipseObject>(obj, layer, objectheight);
						} else {
							wobj = WorldObject.CreateFromTiledObject<BoxObject>(obj, layer, objectheight);
						}

						Instance.WorldObjects.Add(wobj);
					}
					objectheight = Instance.WorldObjects[0].TileObject.Size.z;
					Currenttheight += objectheight;
				}
			}
		}
		[Event.Hotload, ServerCmd("rebuild_world")]
		public static void OnHotload() {
			if(Instance.CurrentWorld == null) return;
			TiledExtensions.tileObjectTypes = null;
			Debug.Info("Hotloading world " + Instance.CurrentWorld);
			foreach(var item in Instance.WorldObjects) {
				item.Delete();
			}
			Instance.WorldObjects.Clear();
			Instance.CurrentMap = null;
			InitWorld();
		}
		[Event.Tick.Server]
		public static void ShowBlocker() {
			if(Instance == null) return;
			Debug.Box(Instance.CurrentMapBounds.Mins, Instance.CurrentMapBounds.Maxs, Color.Green);
			foreach(var blocker in Instance.WorldObjects) {
				blocker.DebugObject();
			}
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
