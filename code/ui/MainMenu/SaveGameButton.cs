using Sandbox.UI;
using TerryDefense.systems;

namespace TerryDefense.UI {
	[UseTemplate]
	public class SaveGameButton : Panel {
		public SaveFile SaveData { get; set; }

		protected override void OnClick(MousePanelEvent e) {
			base.OnClick(e);
			if(e.Target == this) {
				SaveSystem.Load(SaveData);
			}
		}

	}
}
