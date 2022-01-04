using System;
using System.Collections.Generic;
using Sandbox;

namespace TerryDefense.systems {
	public class MissionManager : MechanicManager, ISaveable {

		public static MissionManager Instance => TerryDefenseGame.Instance?.MissionManager;

		public List<Mission> AvailableMissions { get; private set; } = new();

		public List<Mission> ActiveMissions { get; private set; } = new();
		public List<Mission> CompletedMissions { get; private set; } = new();
		public override void Destroy() {

		}

		public override void Init() {

		}
		public static void StartMission(Mission item) {
			AddMission(item);
			TerryDefenseGame.Instance.State = GameState.Ingame;
			if(string.IsNullOrEmpty(item.MapFile) || string.IsNullOrEmpty(item.TileFile)) {
				return;
			}

			WorldManager.LoadWorld(new() {
				MapFile = item.MapFile,
				TileFile = item.TileFile,
			});
		}

		public override void Update() {

		}
		public static void AddAvailableMission(Mission item) {
			if(Instance == null) {
				return;
			}
			if(Instance.AvailableMissions.Contains(item) || Instance.ActiveMissions.Contains(item) || Instance.CompletedMissions.Contains(item)) {
				return;
			}
			Instance.AvailableMissions.Add(item);
		}
		public static void AddMission(Mission mission) {
			Instance.ActiveMissions.Add(mission);
		}
		[TDEvent.Game.ObjectiveCheck]
		public static void CheckMissions(MissionType type, string eventname) {
			if(Instance == null) return;
			foreach(Mission mission in Instance.ActiveMissions) {
				if(mission.Scope == TerryDefenseGame.Instance.State) {
					mission.CheckMission(type, eventname);
				}
			}

		}

		public void Save(ref SaveFile save) {
			save.SaveData.TryAdd("MissionManager", new MissionSaveData() {
				ActiveMissions = ActiveMissions,
				CompletedMissions = CompletedMissions,
				AvailableMissions = AvailableMissions
			});
		}

		public void Load(ref SaveFile save) {
			save.SaveData.TryGetValue("MissionManager", out SaveData data);
			if(data is MissionSaveData missionData) {
				ActiveMissions = missionData.ActiveMissions;
				CompletedMissions = missionData.CompletedMissions;
				AvailableMissions = missionData.AvailableMissions;
			}

		}
		public class MissionSaveData : SaveData {
			public List<Mission> ActiveMissions;
			public List<Mission> CompletedMissions;
			public List<Mission> AvailableMissions;
		}
	}
}
