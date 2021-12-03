using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using TerryDefense.systems;

namespace TerryDefense.UI {
	public class LoadGameMenu : SubMenu {
		Panel ListPanel;
		public LoadGameMenu() : base() {
			AddClass("loadgamemenu");
			StyleSheet.Load("/ui/mainmenu/LoadGameMenu.scss");
			ListPanel = Add.Panel("SaveList");
			GetSaveList();
		}
		public async void GetSaveList() {
			await GameTask.DelayRealtime(100);
			if(SaveSystem.AllSaves == null || SaveSystem.AllSaves.Count == 0)
				SaveSystem.RefreshSaves();
			if(SaveSystem.AllSaves.Count == 0) {
				ListPanel.Add.Label("No saves found", "no-saves");
				return;
			}

			foreach(SaveFile data in SaveSystem.AllSaves) {
				ListPanel.AddChild(new SaveGameButton() {
					SaveData = data
				});
			}
		}
	}
}
