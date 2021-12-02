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
				_ = new TerryDefenseHud();
			}
		}
		public override void Shutdown() {
			if(Instance == this)
				Instance = null;

			foreach(var item in ConfigGlobals.All.OfType<Globals>()) {
				item.Delete();
			}
		}

		public override void ClientJoined(Client client) {
			if(PlayerJoined) {
				client.Kick();
				return;
			}

			var Player = new MainMenuPlayer();
			client.Pawn = Player;

			Player.Respawn();
			PlayerJoined = true;
		}

		public override void DoPlayerSuicide(Client cl) {
		}
	}
}
