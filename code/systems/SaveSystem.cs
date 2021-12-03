using System;
using System.Collections.Generic;
using System.Linq;
using Sandbox;

namespace TerryDefense.systems {
	public static class SaveSystem {
		public static SaveFile CurrentSaveFile { get; private set; }
		public static List<SaveFile> AllSaves { get; private set; }

		public static void RefreshSaves() {
			AllSaves = new();
			FileSystem.Data.CreateDirectory("saves");
			foreach(var item in FileSystem.Data.FindFile("saves")) {
				try {
					AllSaves.Add(FileSystem.Data.ReadJson<SaveFile>(item));
				} catch(System.Exception) {
					Log.Error($"Failed to load save file {item}");
				}
			}
		}

		public static SaveFile CreateNewSave(string SaveName) {
			CurrentSaveFile = new SaveFile {
				saveid = Guid.NewGuid(),
				OriginalSaveTime = DateTime.Now,
				LastSaveTime = DateTime.Now,
				TimePlayed = 0, // TODO: Get time played
				SaveGameName = "SaveName",
				SaveGameCurrentMission = "", //TODO: Get current mission
				SaveData = new()
			};

			return CurrentSaveFile;
		}

		public static void Save() {
			if(CurrentSaveFile == null) {
				CreateNewSave("New Save");
			}
			foreach(var item in Entity.All.Where(x => x is ISaveable)) {
				var saveable = item as ISaveable;
				saveable.Save(CurrentSaveFile);
			}
			FileSystem.Data.CreateDirectory("saves");
			try {
				FileSystem.Data.WriteJson($"saves/{CurrentSaveFile.SaveGameName}.json", CurrentSaveFile);
			} catch(System.Exception) {
				Log.Error($"Failed to save save file {CurrentSaveFile.SaveGameName}");
			}
		}
		public static void Load(SaveFile save) {
			CurrentSaveFile = save;
			foreach(var item in Entity.All.Where(x => x is ISaveable)) {
				var saveable = item as ISaveable;
				saveable.Load(save);
			}
		}

	}
	[System.Serializable]
	public class SaveFile {
		public Guid saveid { get; set; }
		public DateTime OriginalSaveTime { get; set; }
		public DateTime LastSaveTime { get; set; }
		public float TimePlayed { get; set; }

		public string SaveGameName { get; set; }

		public string SaveGameCurrentMission { get; set; }

		public Dictionary<int, SaveData> SaveData { get; set; }

	}
	public interface ISaveable {
		void Save(SaveFile save);
		void Load(SaveFile save);
	}
	public abstract class SaveData {
	}
}
