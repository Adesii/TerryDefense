using System;
using System.Collections.Generic;
using System.Linq;
using Sandbox;
using static TerryDefense.TerryDefenseGame;

namespace TerryDefense.systems {
	public static class SaveSystem {
		private static SaveFile m_savefile;
		public static SaveFile SaveFile => m_savefile;
		public static List<SaveFile> AllSaves { get; private set; } = new();

		public static void RefreshSaves() {
			AllSaves = new();
			FileSystem.Data.CreateDirectory("saves");
			foreach(var item in FileSystem.Data.FindFile("saves")) {
				try {
					AllSaves.Add(FileSystem.Data.ReadJson<SaveFile>("saves/" + item));
				} catch(System.Exception) {
					Log.Error($"Failed to load save file {item}");
				}
			}
			AllSaves = AllSaves.OrderBy(e => e.LastSaveTime).Reverse().ToList();
		}

		public static SaveFile CreateNewSave(CreateSaveFile saveFile) {
			if(AllSaves.Any(x => x.SaveGameName == saveFile.SaveGameName)) {
				Log.Error($"Save file with name {saveFile.SaveGameName} already exists");
				return null;
			}
			m_savefile = new SaveFile {
				saveid = Guid.NewGuid(),
				OriginalSaveTime = DateTime.Now,
				LastSaveTime = DateTime.Now,
				TimePlayed = 0, // TODO: Get time played
				GameState = saveFile.GameState,
				SaveGameName = saveFile.SaveGameName,
				SaveGameCurrentMission = "", //TODO: Get current mission
				SaveData = new()
			};

			Log.Info($"Created new save file {m_savefile.saveid}");
			Save();
			RefreshSaves();

			return m_savefile;
		}

		public static void Save() {
			if(m_savefile == null) {
				CreateNewSave(new());
			}
			foreach(var item in Entity.All.Where(x => x is ISaveable)) {
				var saveable = item as ISaveable;
				saveable.Save(ref m_savefile);
			}
			FileSystem.Data.CreateDirectory("saves");
			try {
				FileSystem.Data.WriteJson($"saves/{m_savefile.SaveGameName}.json", m_savefile);
			} catch(System.Exception) {
				Log.Error($"Failed to save save file {m_savefile.SaveGameName}");
			}
		}
		public static void Load(SaveFile save) {
			m_savefile = save;
			if(m_savefile == null) {
				Log.Error("Failed to load save file");
				return;
			}
			if(m_savefile.WorldData != null && m_savefile.WorldData.MapFile != Global.MapName) {
				WorldManager.LoadWorld(m_savefile.WorldData);
			}
			foreach(var item in Entity.All.Where(x => x is ISaveable)) {
				var saveable = item as ISaveable;
				saveable.Load(ref save);
			}

			Event.Run(TDEvent.Game.LoadedSaveFile.Name, m_savefile);
		}

	}
	[System.Serializable]
	public class SaveFile {
		public Guid saveid { get; set; }
		public DateTime OriginalSaveTime { get; set; }
		public DateTime LastSaveTime { get; set; }
		public float TimePlayed { get; set; }

		public GameState GameState { get; set; }

		public string SaveGameName { get; set; }
		public int Difficulty { get; set; }

		public string SaveGameCurrentMission { get; set; }

		public WorldData WorldData { get; set; }

		public Dictionary<string, SaveData> SaveData { get; set; }
	}
	public struct CreateSaveFile {
		public string SaveGameName { get; set; }
		public int Difficulty { get; set; }
		public GameState GameState { get; set; }
	}
	public interface ISaveable {
		void Save(ref SaveFile save);
		void Load(ref SaveFile save);
	}
	public abstract class SaveData {
	}
}
