using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Gamelib.Extensions;
using Gamelib.FlowFields;
using Sandbox;
using TerryDefense.entities;
using TerryDefense.entities.WorldObjects;
using TerryDefense.Units;
using TiledCS;

namespace TerryDefense.systems {
	public partial class WorldManager : BaseNetworkable {
		public static WorldManager Instance => TerryDefenseGame.Instance.WorldManager;
		public WorldData CurrentWorld { get; protected set; }

		public TiledMap CurrentMap { get; protected set; }

		public BBox CurrentMapBounds { get; protected set; }

		[Net] public List<WorldObject> WorldObjects { get; protected set; } = new();

		FileWatch _fileWatch;


		public static void LoadWorld(WorldData world) {
			if(Instance == null) return;
			Instance.CurrentWorld = world;
			Global.Lobby.SetData("Switching", "true");
			Global.Lobby.SetData("SaveFile", SaveSystem.SaveFile.saveid.ToString());
			string sterl = StringX.NormalizeFilename(world.MapFile);
			sterl = Path.GetFileNameWithoutExtension(sterl);

			Log.Error("Loading world: " + sterl);
			Log.Error($"From Current World: {Global.MapName}");


			if(sterl != Global.MapName && !string.IsNullOrEmpty(Instance.CurrentWorld.MapFile)) {
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
				Instance.CurrentMapBounds = new BBox(-tempsize, tempsize.WithZ(128));
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
						if(Host.IsServer)
							wobj?.RebuildObject();
						Instance.WorldObjects.Add(wobj);
					}
					objectheight = Instance.WorldObjects[0].TileObject.Size.z; //TODO: Find proper way to adjust the height of the objects
					Currenttheight += objectheight;
				}


			} else {
				throw new Exception("No layers found in map");
			}



			if(Instance._fileWatch == null) {
				Debug.Error("File watcher not set watching now: " + Instance.CurrentWorld.TileFile);
				Instance._fileWatch = FileSystem.Mounted.Watch("/data/maps/*");

				Instance._fileWatch.Enabled = true;
				Instance._fileWatch.OnChanges += Instance.CheckFile;
			} else {
				Instance._fileWatch.OnChanges -= Instance.CheckFile;
				Instance._fileWatch.Dispose();

				Instance._fileWatch = FileSystem.Mounted.Watch("/data/maps/*");

				Instance._fileWatch.Enabled = true;
				Instance._fileWatch.OnChanges += Instance.CheckFile;
			}
		}

		public void CheckFile(FileWatch file) {
			//foreach(var item in file.Changes) {
			//	Log.Error("File changed: " + item);
			//}
			DelayHotload();
		}
		private async void DelayHotload() {
			await GameTask.DelaySeconds(0.1f);
			OnHotload();
		}
		[Event.Hotload, ServerCmd("rebuild_world")]
		public static void OnHotload() {
			if(Instance.CurrentWorld == null || string.IsNullOrEmpty(Instance.CurrentWorld.TileFile)) return;
			TiledExtensions.tileObjectTypes = null;
			PathManager.All.Clear();
			Debug.Info("Hotloading world " + Instance.CurrentWorld);
			foreach(var item in Instance.WorldObjects) {
				item.Delete();
			}
			Instance.WorldObjects.Clear();
			Instance.CurrentMap = null;
			InitWorld();

			PathManager.All.Clear();
			PathManager._pathfinders.Clear();
			PathManager._pathfinders = new();
			PathManager._smallest = null;
			PathManager._largest = null;
			PathManager._default = null;
			PathManager.Create(25, 50);
			PathManager.Update();
			AwaitCompletionOfFlowField();

		}
		private static async void AwaitCompletionOfFlowField() {
			await GameTask.DelaySeconds(0.5f);
			while(PathManager._pathfinders.Count < 0) {
				await GameTask.DelaySeconds(0.25f);
			}
			foreach(var item in Entity.All.OfType<BaseUnit>()) {
				item.RebuildPath();
			}
		}
		[Event.Tick.Server]
		public static void ShowBlocker() {
			if(Instance == null || !Debug.Enabled) return;
			//Debug.Box(Instance.CurrentMapBounds.Mins, Instance.CurrentMapBounds.Maxs, Color.Green);
			if(PathManager.Bounds.HasValue)
				Debug.Box(PathManager.Bounds.Value.Mins, PathManager.Bounds.Value.Maxs, Color.Magenta);
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
			TileFile = ""
		};
	}
}
