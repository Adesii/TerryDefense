using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Sandbox;
using TerryDefense.components.turret;

namespace TerryDefense.Towers {
	public class TurretInstance {
		public const string TurretDirectory = "/data/Turrets";
		public static Dictionary<string, TurretInstance> CachedInstances = new();
		private static FileWatch Watcher;

		private static bool ListDirty = true;


		public string SaveLocation = "";
		public string Name { get; set; } = "TestName";
		public string Description { get; set; }



		[JsonConverter(typeof(DictionaryStringObjectJsonConverter))]
		public Dictionary<string, object> SerializableComponents { get; set; } = new();

		public Dictionary<string, BaseTurretComponent> Components { get; set; } = new();

		[Event.Entity.PostSpawn]
		public static void Hotload() {
			if(Watcher == null) {
				Watcher = FileSystem.Mounted.Watch(TurretInstance.TurretDirectory + "/*");
				Watcher.OnChangedFile += (e) => {
					ListDirty = true;
				};
			}
		}

		public static void RecacheTurrets() {
			if(!ListDirty) return;
			CachedInstances.Clear();
			var files = FileSystem.Mounted.FindFile(TurretDirectory);
			int index = 0;
			foreach(var file in files) {
				var towerName = System.IO.Path.ChangeExtension(file, null);
				var data = FileSystem.Mounted.ReadAllText($"{TurretDirectory}/{towerName.ToLower()}.turret");
				var instance = JsonSerializer.Deserialize<TurretInstance>(data);
				Debug.Info($"Loading {towerName}");
				instance.Components = new();
				foreach(var item in instance.SerializableComponents) {
					Debug.Info($"Loading {item.Key}");
					var comp = Library.Create<BaseTurretComponent>(Type.GetType(item.Key).Name);
					Dictionary<string, object> dict = item.Value as Dictionary<string, object>;
					foreach(var compProperties in Reflection.GetProperties(comp)) {
						if(dict.ContainsKey(compProperties.Name)) {
							if(compProperties.PropertyType.IsEnum) {
								compProperties.SetValue(comp, Enum.Parse(compProperties.PropertyType, dict[compProperties.Name].ToString()));
							} else {
								compProperties.SetValue(comp, Convert.ChangeType(dict[compProperties.Name], compProperties.PropertyType));
							}
							Debug.Info($"Setting {compProperties.Name} to {dict[compProperties.Name]}");
						}
					}
					instance.Components.Add(item.Key, comp);
				}
				CachedInstances.Add(file, instance);
				index++;
			}
		}
	}
}
