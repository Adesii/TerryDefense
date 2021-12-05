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
		[Net] public MechanicManager GameplayManager { get; protected set; }
		[Net] public MissionManager MissionManager { get; protected set; } = new();

		[Net] protected bool PlayerJoined { get; set; }
		public TerryDefenseGame() {
			Instance = this;
			if(IsServer) {
				WorldManager = new();
				_ = new TerryDefenseHud();
			}
			if(Global.Lobby.GetData("Switching") == "true") {
				SaveSystem.RefreshSaves();
				if(Guid.TryParse(Global.Lobby.GetData("SaveFile"), out Guid saveid) && SaveSystem.AllSaves != null) {
					SaveSystem.Load(SaveSystem.AllSaves.Where(x => x.saveid == saveid).First());
					return;
				}
			}

			Debug.Error("No save found");
			Debug.Error($"Saves: {SaveSystem.AllSaves?.Count}");
			Debug.Error($"Lobby Data {Global.Lobby.GetData("SaveFile")}   {Global.Lobby.GetData("Switching")}");
			return;
		}
		[Event.Tick]
		public void Tick() {
			GameplayManager?.Update();
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
			Debug.Error("Client joined");


			PlayerPawn Player = null;
			if(SaveSystem.SaveFile != null) {
				Debug.Log($"Loading save file {SaveSystem.SaveFile.GameState}");
				switch(SaveSystem.SaveFile.GameState) {
					case GameState.Menu:
						Player = new MainMenuPlayer();
						break;
					case GameState.Base:
						Player = new BasePlayer();
						GameplayManager = new BaseManager();
						break;
					case GameState.Ingame:
						Player = new TerryDefensePlayer();
						break;
					case GameState.PostGame:
						break;
					default:
						Player = new MainMenuPlayer();
						break;
				}
			} else {
				Player = new MainMenuPlayer();
			}

			Debug.Error("Player created");
			Debug.Error($"Player: {Player}");



			client.Pawn = Player;

			Player.Respawn();
			PlayerJoined = true;

			GameplayManager?.Init();
			MissionManager?.Init();
		}

		public override void DoPlayerSuicide(Client cl) {
		}
	}
}
