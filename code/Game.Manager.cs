using System;
using Sandbox;


namespace TerryDefense {
	public partial class TerryDefenseGame : Game {
		private GameState state = GameState.Menu;



		public GameState State {
			get { return state; }
			set {
				if(state == value)
					return;
				OnStateEnded(state);
				state = value;
				OnStateChanged(state);
			}
		}
		private void OnStateChanged(GameState state) {
			Event.Run(TDEvent.Game.StateChanged.Name, state);
		}

		private void OnStateEnded(GameState state) {
			Event.Run(TDEvent.Game.StateEnded.Name, state);
		}


	}

	public enum GameState {
		Menu,
		Base,
		Ingame,
		PostGame,
	}
}
