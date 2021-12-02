using System;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TerryDefense.UI {
	[UseTemplate]
	public class MainMenuRoot : Panel {
		public MainMenuRoot() {

		}

		public void NewGameMenu() {
			AddClass("submenu");
			AddChild(new NewGameMenu());

		}
		public void LoadGameMenu() {
			AddClass("submenu");
			AddChild(new LoadGameMenu());

		}
		public void OptionsMenu() {
			AddClass("submenu");
			AddChild(new OptionsMenu());

		}
	}
	public class SubMenu : Panel {
		public Button backButton;
		public SubMenu() {
			AddClass("submenu visible");
			backButton = Add.ButtonWithIcon("Back", "arrow_back_ios", "back", () => {
				Back();
			});

		}

		public void Back() {
			Parent.RemoveClass("submenu");
			Delete();
		}
	}

	public class NewGameMenu : SubMenu {
		public NewGameMenu() : base() {

		}

		public void NewGame() { }
	}
	public class LoadGameMenu : SubMenu {
		public LoadGameMenu() : base() {

		}

		public void LoadGame() { }
	}
	public class OptionsMenu : SubMenu {
		public OptionsMenu() : base() {

		}

		public void Options() { }
	}
}
