using System;
using Sandbox;
using Sandbox.UI;
using TerryDefense.entities;
using TerryDefense.Player;
using TerryDefense.systems;

namespace TerryDefense.UI {
	[UseTemplate]
	public class IngameHudRoot : Panel {

		private TDCamera m_camera;
		public TowerBuilderPanel towerBuilderPanel;
		public override void OnMouseWheel(float value) {
			base.OnMouseWheel(value);
			if(m_camera == null)
				m_camera = (TDCamera)(Local.Pawn as PlayerPawn).Camera;
			else
				m_camera.ZoomBy(-value);
		}

		public void Open(string what) {
			switch(what) {
				case "tower":
					if(towerBuilderPanel == null) {
						towerBuilderPanel = new TowerBuilderPanel();
						AddChild(towerBuilderPanel);
					}
					towerBuilderPanel.Open();
					break;
			}
		}

	}
}
