using Sandbox;

namespace TerryDefense.UI {
	public class GlitchyPostProcess : MaterialPostProcess {
		public GlitchyPostProcess() : base("materials/postprocessing/mainmenuglitchy.vmat") {
			Enabled = true;
			EnabledScanLines = true;
			EnabledNoise = true;
			EnabledChromaticAberration = true;
		}

		public bool Enabled {
			set => SetCombo("D_ENABLED", value);
		}
		//SCANLINES
		public bool EnabledScanLines {
			set => SetCombo("D_SCANLINES", value);
		}
		public float ScanLinesIntensity {
			set => Set("g_fScanLinesIntensity", value);
		}
		public float ScanLinesSize {
			set => Set("g_fScanLinesSize", value);
		}
		public float ScanLinesSpeed {
			set => Set("g_fScanLinesSpeed", value);
		}

		//FUZZYNOISE
		public bool EnabledNoise {
			set => SetCombo("D_NOISE", value);
		}
		public float NoiseIntensity {
			set => Set("g_fNoiseIntensity", value);
		}

		//CHROMATICABBERATION
		public bool EnabledChromaticAberration {
			set => SetCombo("D_CHROMATIC", value);
		}
		public float ChromaticAberrationIntensity {
			set => Set("g_fChromaticAberrationIntensity", value);
		}
	}
}
