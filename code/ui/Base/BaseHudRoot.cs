using System;
using Sandbox;
using Sandbox.UI;
using TerryDefense.entities;
using TerryDefense.Player;
using TerryDefense.systems;

namespace TerryDefense.UI {
	[UseTemplate]
	public class BaseHudRoot : Panel {

		private BaseCamera m_camera;
		private BaseRoom m_Room;
		private BaseRoom m_Hovered_Room;

		private WorldPanel HoverPanel;
		public BaseRoom SelectedRoom {
			get {
				return m_Room;
			}
			set {
				if(m_Room == value) return;
				bool shouldTeleport = m_Room != null;
				if(value != null) {
					Transform? transform = value.CameraTransform;
					if(transform.HasValue)
						m_camera.SetTarget(transform.Value, shouldTeleport);
				} else {
					m_camera.ClearTarget();
					m_Room.RoomData.Deselected();
				}
				m_Room = value;
			}
		}
		public BaseHudRoot() : base() {

		}

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
					SelectedRoom = BaseManager.Instance.GetMainRoom(entities.RoomType.HQ);
					break;
				case 1:
					SelectedRoom = BaseManager.Instance.GetMainRoom(entities.RoomType.Research);
					break;
				case 2:
					SelectedRoom = BaseManager.Instance.GetMainRoom(entities.RoomType.Armory);
					break;
				case 3:
					SelectedRoom = BaseManager.Instance.GetMainRoom(entities.RoomType.Factory);
					break;

				default:
					SelectedRoom = null;
					break;
			}
		}

		public override void Tick() {
			base.Tick();
			if(m_camera == null) {
				var room = BaseManager.Instance.GetMainRoom(RoomType.HQ);
				if(m_camera == null)
					m_camera = (Local.Pawn as BasePlayer).Camera;
				if(room != null) {
					m_camera.SetTarget(room.Transform);
					m_camera.ClearTarget();
				}
			}
			var trace = Trace.Ray(Input.Cursor, 10000).Run();
			if(trace.Hit && trace.Entity is BaseRoom Room) {
				SetHoverPanelTarget(Room);
			} else {
				SetHoverPanelTarget(null);
			}

		}
		protected override void OnClick(MousePanelEvent e) {
			base.OnClick(e);
			if(e.Target == this) {
				if(SelectedRoom == null && m_Hovered_Room != null)
					m_Hovered_Room.RoomData?.Selected();
				SelectedRoom = m_Hovered_Room;
			}
		}

		public void SetHoverPanelTarget(BaseRoom room) {
			if(HoverPanel == null) {
				HoverPanel = new WorldPanel();
				HoverPanel.StyleSheet.Load("ui/Base/BaseHudRoot.scss");
				HoverPanel.AddClass("hover-panel");
				HoverPanel.Position = new Vector3();
				HoverPanel.Rotation = Rotation.FromAxis(Vector3.Forward, 0);
				HoverPanel.WorldScale = 20;
			}
			m_Hovered_Room = room;
			HoverPanel.SetClass("hidden", m_Hovered_Room == null);
			if(m_Hovered_Room != null) {
				HoverPanel.Position = m_Hovered_Room.Transform.Position.WithX(-50);
				HoverPanel.PanelBounds = new Rect(-m_Hovered_Room.MaxBounds.y, -m_Hovered_Room.MaxBounds.z, m_Hovered_Room.MaxBounds.y * 2, m_Hovered_Room.MaxBounds.z * 2);
			}
		}
	}
}
