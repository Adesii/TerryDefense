using System;
using Gamelib.UI;
using Sandbox;
using Sandbox.UI;

namespace TerryDefense.UI {
	public static class MouseCursor {
		[Event.BuildInput]
		public static void BuildInput(InputBuilder input) {
			var hovered = UIUtility.GetHoveredPanel(TerryDefenseHud.Instance.MainPanel);
			if(hovered == null || hovered is GameRootPanel)
				return;

			if(input.Pressed(InputButton.Attack2)) {
				hovered.CreateEvent(new MousePanelEvent("onclick", hovered, "mouseleft"));
				Log.Info("Clicked on " + hovered);
			}
			if(input.Pressed(InputButton.Attack1)) {
				hovered.CreateEvent(new MousePanelEvent("onclick", hovered, "mouseright"));
				Log.Info("Clicked on " + hovered);
			}



		}
	}
}
