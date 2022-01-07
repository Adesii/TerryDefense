using System;
using Sandbox;
using Sandbox.UI;

namespace TerryDefense.UI {
	[UseTemplate]
	public class TowerBuilderPanel : Panel {
		public Panel ListPanel { get; set; }

		private const string WatchDirectory = "/data/Turrets";
		private FileWatch Watcher;
		bool isopen = false;

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
			SetClass("open", isopen = true);
			if(Watcher == null) {
				Watcher = FileSystem.Mounted.Watch(WatchDirectory + "/*");
				Watcher.OnChangedFile += (e) => {
					ListDirty = true;
				};
				Refresh();
			}
		}
		public void Close() {
			SetClass("open", isopen = false);
		}
		[Event.Hotload, Event("refresh_buildlist")]
		public void Refresh() {
			ListPanel.DeleteChildren();
			foreach(var item in FileSystem.Mounted.FindFile(WatchDirectory)) {
				Log.Error(item);
				ListPanel.AddChild(new Button(item, "", () => {
					Event.Run("build_turret", item);
				}));
			}
		}
	}
}
