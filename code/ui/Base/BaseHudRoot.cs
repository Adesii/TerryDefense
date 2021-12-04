using System;
using Sandbox;
using Sandbox.UI;
using TerryDefense.Player;
using TerryDefense.systems;

namespace TerryDefense.UI {
	[UseTemplate]
	public class BaseHudRoot : Panel {

		private BaseCamera m_camera;

		public override void OnMouseWheel(float value) {
			base.OnMouseWheel(value);
			if(m_camera == null)
				m_camera = (Local.Pawn as BasePlayer).Camera;
			m_camera.ZoomBy(-value);
		}

		public void FocusOn(string type) {
			if(m_camera == null)
				m_camera = (Local.Pawn as BasePlayer).Camera;
			switch(type.ToInt()) {
				case 0:
					m_camera.LookAt = BaseManager.Instance.GetMainRoom(entities.RoomType.HQ).Position;
					break;
				case 1:
					m_camera.LookAt = BaseManager.Instance.GetMainRoom(entities.RoomType.Research).Position;
					break;
				case 2:
					m_camera.LookAt = BaseManager.Instance.GetMainRoom(entities.RoomType.Armory).Position;
					break;
				case 3:
					m_camera.LookAt = BaseManager.Instance.GetMainRoom(entities.RoomType.Factory).Position;
					break;

				default:
					break;
			}
			m_camera.ZoomLevel = 1.5f;
		}

	}
}
