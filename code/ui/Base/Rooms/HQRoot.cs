using System;
using Sandbox.UI;
using Sandbox.UI.Construct;
using TerryDefense.systems;

namespace TerryDefense.UI {
	[UseTemplate]
	public class HQRoot : Panel {


		public Panel MissionList { get; set; }

		public HQRoot() {
			Log.Error($"AvailableMissions.Count: {TerryDefenseGame.Instance.MissionManager.AvailableMissions.Count}");
			foreach(var item in TerryDefenseGame.Instance.MissionManager.AvailableMissions) {
				MissionList.AddChild(new MissionButton(item));
				Log.Error("Added mission " + item.Title);
			}
		}

	}

	internal class MissionButton : Panel {
		private Mission item;

		public MissionButton(Mission item) {
			this.item = item;
			Add.Label(item.Title, "title");
			Add.Label(item.Description, "description");
		}

		protected override void OnClick(MousePanelEvent e) {
			base.OnClick(e);
			if(e.Target == this) {
				MissionManager.StartMission(item);
			}
		}
	}
}
