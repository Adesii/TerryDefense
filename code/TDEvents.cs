using System;
using Sandbox;

namespace TerryDefense {
	public static class TDEvent {

		public static class Game {
			public class StateChanged : EventAttribute {
				public static string Name = "Game.StateChanged";
				public StateChanged() : base(Name) { }
			}
			public class StateEnded : EventAttribute {
				public static string Name = "Game.StateEnded";
				public StateEnded() : base(Name) { }
			}

			public class LoadedSaveFile : EventAttribute {
				public static string Name = "Game.LoadedSaveFile";
				public LoadedSaveFile() : base(Name) { }
			}
			public class SavedSaveFile : EventAttribute {
				public static string Name = "Game.SavedSaveFile";
				public SavedSaveFile() : base(Name) { }
			}

		}
		public static class Base {
			public class Attacked : EventAttribute {
				public static string Name = "Base.BeingAttacked";
				public Attacked() : base(Name) { }
			}
			public class Destroyed : EventAttribute {
				public static string Name = "Base.Destroyed";
				public Destroyed() : base(Name) { }
			}
		}
		public static class Wave {
			public class Started : EventAttribute {
				public static string Name = "Wave.Started";
				public Started() : base(Name) { }
			}
			public class Ended : EventAttribute {
				public static string Name = "Wave.Ended";
				public Ended() : base(Name) { }
			}
			public class Reinforcement : EventAttribute {
				public static string Name = "Wave.Reinforcement";
				public Reinforcement() : base(Name) { }
			}
			public class NewEnemy : EventAttribute {
				public static string Name = "Wave.NewEnemy";
				public NewEnemy() : base(Name) { }
			}
		}

	}
}
