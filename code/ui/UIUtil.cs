using Sandbox;
using Sandbox.UI;

namespace Gamelib.UI {
	public static class UIUtility {
		public static Panel GetHoveredPanel(Panel root = null) {
			root ??= Local.Hud;

			if(root.PseudoClass.HasFlag(PseudoClass.Hover)) {
				if(!string.IsNullOrEmpty(root.ComputedStyle.PointerEvents) && root is not TerryDefense.UI.GameRootPanel) {
					if(root.ComputedStyle.PointerEvents == "visible" && root.ComputedStyle.PointerEvents != "none" && root is Button)
						return root;
				}
			}

			foreach(var child in root.Children) {
				var panel = GetHoveredPanel(child);

				if(panel != null)
					return panel;
			}

			return null;
		}
	}
}
