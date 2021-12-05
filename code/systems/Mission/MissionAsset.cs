using System;
using System.Collections.Generic;
using Sandbox;
using TerryDefense.systems;

namespace TerryDefense {
	[Library("mssn"), AutoGenerate]
	public class MissionAsset : Asset {
		public Mission Objective { get; set; }
		/// <summary>
		/// Dictionary of all the missions.
		/// </summary>
		public static IReadOnlyList<Mission> All => _all;
		internal static List<Mission> _all = new();

		protected override void PostLoad() {
			base.PostLoad();
			if(!_all.Contains(Objective))
				_all.Add(Objective);
			Log.Error($"Mission {Name} loaded.");
		}
	}


	public struct Mission {
		public string Title { get; set; }
		public string Description { get; set; }

		public MissionDetails Details { get; set; }
		public List<Requirement> Requirements { get; set; }

		public ObjectiveStatus Status;

		internal void CheckMission(MissionType type, string eventname) {
			foreach(var item in Requirements) {
				if(item.MissionType == type && item.Event == eventname) {

				}
			}
		}
	}
	public struct Requirement {
		/// <summary>
		/// Stuff like Enemy XY. example: terry_small. this would add to the objective each time an enemy of this type is killed.
		/// Or Building XY. example: lightning_tower. this would add to the objective each time a building of this type is built.
		/// </summary>
		public string Event { get; set; }
		public MissionType MissionType { get; set; }
		public int CurrentAmount;
		public int RequiredAmount { get; set; } = 1;

		public ObjectiveStatus Status;

		public void Check(MissionType type, string eventname) {
			if(MissionType == type && Event == eventname) {
				CurrentAmount++;
				if(RequiredAmount >= CurrentAmount) {
					Status = ObjectiveStatus.Incomplete;
				} else {
					Status = ObjectiveStatus.Complete;
				}
			}
		}
	}
	public struct MissionDetails {
		[ResourceType("vmap")] public string MapFile { get; set; }
		/// <summary>
		/// data/maps/ will be prefixed to this. making it easier to select the right map just by name
		/// </summary>
		public string TileFile { get; set; }
		public GameState Scope { get; set; } = GameState.Ingame;

	}

	public enum ObjectiveType {
		Defend,
		Kill,
		Destroy,
		Capture,
		None
	}

	public enum ObjectiveStatus {
		Incomplete,
		Complete,
		Failed
	}
	public enum MissionType {
		Kill,
		Build,
		Capture,
		Timed,
		Research,
	}

}
