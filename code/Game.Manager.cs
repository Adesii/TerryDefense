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
			Debug.Error("GameState changed to " + state);
			Event.Run(TDEvent.Game.StateChanged.Name, state);
			if(!Host.IsServer)
				SetFromClientState(state.ToString());
		}
		[ServerCmd]
		public static void SetFromClientState(string state) {
			GameState newState = (GameState)Enum.Parse(typeof(GameState), state);
			Instance.State = newState;
			Debug.Error("GameState changed to " + newState);

		}

		private void OnStateEnded(GameState state) {
			Debug.Error("GameState ended " + state);
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
