using Gamelib.Network;
using Sandbox;
using Sandbox.UI.Construct;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using TerryDefense.Player;
using TerryDefense.systems;
using TerryDefense.UI;

namespace TerryDefense {

	public partial class TerryDefenseGame : Game {
		public static TerryDefenseGame Instance { get; protected set; }
		[Net] public WorldManager WorldManager { get; protected set; }

		[Net] protected bool PlayerJoined { get; set; }
		public TerryDefenseGame() {
			Instance = this;
			if(IsServer) {
				WorldManager = new();
				_ = new TerryDefenseHud();
			}
			if(Global.Lobby.GetData("Switching") == "true") {
				SaveSystem.RefreshSaves();
				Log.Error("Switching to new save");
				var SaveName = Global.Lobby.GetData("SaveFile");
				if(!string.IsNullOrEmpty(SaveName)) {
					SaveSystem.Load(SaveSystem.AllSaves.Where(x => x.SaveGameName == SaveName).First());
					return;
				}
			}



		}
		public override void Shutdown() {
			if(Instance == this)
				Instance = null;
		}

		public override void ClientJoined(Client client) {
			if(PlayerJoined) {
				client.Kick();
				return;
			}
			PlayerPawn Player = null;
			if(SaveSystem.SaveFile != null) {
				switch(SaveSystem.SaveFile.GameState) {
					case GameState.Menu:
						Player = new MainMenuPlayer();
						break;
					case GameState.Base:
						Player = new BasePlayer();
						break;
					case GameState.Ingame:
						break;
					case GameState.PostGame:
						break;
					default:
						break;
				}
			} else {
				Player = new MainMenuPlayer();
			}



			client.Pawn = Player;

			Player.Respawn();
			PlayerJoined = true;
		}

		public override void DoPlayerSuicide(Client cl) {
		}
	}
}
