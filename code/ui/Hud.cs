using Sandbox.UI;

namespace TerryDefense.UI {
	public partial class TerryDefenseHud : Sandbox.HudEntity<RootPanel> {
		public static TerryDefenseHud Instance;

		public Panel MainPanel;

		public TerryDefenseHud() {
			Instance = this;
			if(!IsClient) return;
		}

		public static void SetNewMainPanel(Panel panel) {
			Instance.RootPanel.DeleteChildren(true);
			Instance.MainPanel = panel;
			Instance.RootPanel.AddChild(panel);
		}
	}
}
