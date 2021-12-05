using Sandbox.UI;
using TerryDefense.systems;

namespace TerryDefense.UI {
	[UseTemplate]
	public class NewGameMenu : SubMenu {
		public string SaveName { get; set; }
		public int Difficulty { get; set; }

		public NewGameMenu() : base() {
			AddClass("newgamemenu");
		}


		public void NewGame() {
			Log.Error($"Creating new game with name {SaveName} and difficulty {Difficulty}");
			SaveSystem.CreateNewSave(new() {
				SaveGameName = SaveName,
				Difficulty = Difficulty,
				GameState = GameState.Base
			});
			TerryDefenseHud.Instance.RootPanel.DeleteChildren();
			WorldManager.LoadWorld(TemplateWorldData.BaseWorld);
		}
	}
}
