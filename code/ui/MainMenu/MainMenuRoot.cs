using System;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using TerryDefense.player.controllers;

namespace TerryDefense.UI {
	[UseTemplate]
	public class MainMenuRoot : Panel {

		Panel recordingcircle { get; set; }

		private ModelEntity Planet;
		private ModelEntity SpaceStation;

		private Entity FocusOn;
		private Vector3 Offset;
		MenuCamera cam;
		public MainMenuRoot() {
			cam = Local.Pawn.Camera as MenuCamera;
			cam.Position = new Vector3(-1000, 0, 0);
			cam.FieldOfView = 25;





		}

		bool recording = true;
		RealTimeSince lastblink = 0;

		public override void Tick() {
			base.Tick();
			if(Planet == null) {
				Planet = new("models/earth.vmdl") {
					Position = new Vector3(0, 0, -20),
					Rotation = Rotation.From(0, -30, 0),
					Scale = 2f
				};
				SpaceStation = new("models/spacestation.vmdl") {
					Position = new Vector3(-150, 50, 50),
					Rotation = Rotation.From(0, 30, 30),
					Scale = 0.1f
				};
				var l = Light.Point(new(-500, -500, 100), 5000, Color.White);
				l.Falloff = 0;
				PostProcess.Add(new GlitchyPostProcess());

			}
			Planet.Rotation = Planet.Rotation.RotateAroundAxis(Vector3.Up, 0.04f);
			SpaceStation.Rotation = SpaceStation.Rotation.RotateAroundAxis(Vector3.Up, -0.04f);
			if(HasClass("submenu") && FocusOn.IsValid()) {
				cam.Position = cam.Position.LerpTo(FocusOn.Position + Offset, 2f * Time.Delta);
			} else {
				if(!HasClass("submenu")) {
					FocusOn = null;
				}
				cam.Position = cam.Position.LerpTo(new Vector3(-1000, 0, 0), 2f * Time.Delta);
			}

			if(lastblink > 1f) {
				lastblink = 0;
				recording = !recording;
				recordingcircle.SetClass("blinking", recording);
			}
		}

		public void NewGameMenu() {
			AddClass("submenu");
			AddChild(new NewGameMenu());
			FocusOn = SpaceStation;
			Offset = new Vector3(-100, 0, -5);

		}
		public void LoadGameMenu() {
			AddClass("submenu");
			AddChild(new LoadGameMenu());
			FocusOn = SpaceStation;
			Offset = new Vector3(-100, 0, -5);

		}
		public void OptionsMenu() {
			AddClass("submenu");
			AddChild(new OptionsMenu());
			FocusOn = Planet;
			Offset = new Vector3(-200, 0, 0);

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
	public class GlitchyPostProcess : MaterialPostProcess {
		public GlitchyPostProcess() : base("materials/postprocessing/mainmenuglitchy.vmat") { }
	}
}
