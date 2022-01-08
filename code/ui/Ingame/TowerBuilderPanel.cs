using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Sandbox;
using Sandbox.UI;
using TerryDefense.components.turret;
using TerryDefense.Towers;

namespace TerryDefense.UI {
	[UseTemplate]
	public class TowerBuilderPanel : Panel {
		public Panel ListPanel { get; set; }



		bool isdirty = false;
		public bool ListDirty {
			get { return isdirty; }
			set {
				isdirty = value;
				if(isdirty && IsVisible) {
					Event.Run("refresh_buildlist");
					isdirty = false;
				}
			}
		}

		public void Open() {
			SetClass("open", true);
			Refresh();
		}
		public void Close() {
			SetClass("open", false);
		}
		[Event.Hotload, Event("refresh_buildlist")]
		public void Refresh() {
			ListPanel.DeleteChildren();
			TurretInstance.RecacheTurrets();
			int index = 0;
			foreach(var item in TurretInstance.CachedInstances) {
				var instance = item.Value;
				ListPanel.AddChild(new Button(instance.Name, "", () => {
					Event.Run("build_turret", item.Key);
				}));
				index++;
			}
		}
	}
}
