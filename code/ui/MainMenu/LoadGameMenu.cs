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
		public void GetSaveList() {
			if(SaveSystem.AllSaves == null)
				SaveSystem.RefreshSaves();
			if(SaveSystem.AllSaves.Count == 0) {
				ListPanel.Add.Label("No saves found", "no-saves");
				return;
			}

			foreach(SaveFile data in SaveSystem.AllSaves) {
				ListPanel.Add.Button(data.SaveGameName, "SaveGame", () => {
					SaveSystem.Load(data);
				});
			}

		}
	}
}
