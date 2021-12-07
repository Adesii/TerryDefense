using Sandbox.UI;
using TerryDefense.systems;

namespace TerryDefense.UI {
	[UseTemplate]
	public class SaveGameButton : Panel {
		public SaveFile SaveData { get; set; }

		protected override void OnClick(MousePanelEvent e) {
			base.OnClick(e);
			if(SaveData != null) {
				SaveSystem.Load(SaveData);
			}
		}

	}
}
