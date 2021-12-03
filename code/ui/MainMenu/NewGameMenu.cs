using Sandbox.UI;

namespace TerryDefense.UI {
	public class NewGameMenu : SubMenu {
		public Panel GamesList { get; set; }
		public NewGameMenu() : base() {
			AddClass("newgamemenu");
			CreateUI();
		}

		public void CreateUI() {

		}


		public void NewGame() { }
	}
}
