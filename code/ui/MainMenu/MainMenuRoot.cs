using System;
using Gamelib.Extensions;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using TerryDefense.Player;

namespace TerryDefense.UI {
	[UseTemplate]
	public class MainMenuRoot : Panel {

		Panel recordingcircle { get; set; }
		Button ContinueButton { get; set; }

		GlitchyPostProcess postProcess { get; set; }

		private ModelEntity Planet;
		private ModelEntity SpaceStation;
		private Light Sun;

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
				Planet.SetupPhysicsFromModel(PhysicsMotionType.Keyframed);
				SpaceStation = new("models/spacestation.vmdl") {
					Position = new Vector3(-150, 50, 50),
					Rotation = Rotation.From(0, 30, 30),
					Scale = 0.1f,
					Parent = Planet,
					EnableShadowCasting = false
				};
				Sun = Light.Point(new(-500, -500, 100), 5000, Color.White);
				Sun.Falloff = 0;
				PostProcess.Add(new GlitchyPostProcess());
				postProcess = PostProcess.Get<GlitchyPostProcess>();

			}
			Planet.Rotation = Planet.Rotation.RotateAroundAxis(Vector3.Up, 0.04f);
			Planet.ResetInterpolation();
			SpaceStation.Rotation = SpaceStation.Rotation.RotateAroundAxis(Vector3.Up, -0.2f);
			SpaceStation.ResetInterpolation();
			if(HasClass("submenu") && FocusOn.IsValid()) {
				var trace = Trace.Ray(cam.Position, FocusOn.Position + Offset).Radius(2).Ignore(FocusOn).Run();
				Vector3 newOffset = new();
				if(trace.Hit) {
					newOffset = trace.Normal * 30f;
				}
				cam.Position = cam.Position.LerpTo(FocusOn.Position + Offset + newOffset, 2f * Time.Delta);
			} else {
				if(!HasClass("submenu") && FocusOn != null) {
					FocusOn = null;
					postProcess.EnabledChromaticAberration = true;
				}
				cam.Position = cam.Position.LerpTo(new Vector3(-1000, 0, 0), 2f * Time.Delta);
			}

			if(lastblink > 1f) {
				lastblink = 0;
				recording = !recording;
				recordingcircle.SetClass("blinking", recording);
			}
		}

		public void Continue() {
		}

		public override void OnDeleted() {
			base.OnDeleted();
			Planet.Delete();
			SpaceStation.Delete();
			Sun.Delete();
			postProcess.Enabled = false;
		}

		public void NewGameMenu() {
			AddClass("submenu");
			AddChild(new NewGameMenu());
			FocusOn = SpaceStation;
			Offset = new Vector3(-100, 0, -5);

			postProcess.EnabledChromaticAberration = false;

		}
		public void LoadGameMenu() {
			AddClass("submenu");
			AddChild(new LoadGameMenu());
			FocusOn = SpaceStation;
			Offset = new Vector3(-100, 0, -5);

			postProcess.EnabledChromaticAberration = false;
		}
		public void OptionsMenu() {
			AddClass("submenu");
			AddChild(new OptionsMenu());
			FocusOn = Planet;
			Offset = new Vector3(-200, 0, 0);

			postProcess.EnabledChromaticAberration = false;
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
	public class OptionsMenu : SubMenu {
		public OptionsMenu() : base() {
			AddClass("optionsmenu");
		}

		public void Options() { }
	}
}
