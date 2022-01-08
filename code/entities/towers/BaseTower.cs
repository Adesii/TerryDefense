using System;
using Sandbox;
using TerryDefense.Units;
using System.Linq;
using TerryDefense.components.turret;
using System.Text.Json;
using System.Collections.Generic;

namespace TerryDefense.Towers {
	public class BaseTower : AnimEntity {

		FileWatch Watcher;

		[TDEvent.Tower.Tick]
		public virtual void Tick() {
			Debug.Box(Position, Vector3.One * -10, Vector3.One * 10, Color.Red, 0f);
			foreach(var item in Components.GetAll<BaseTurretComponent>()) {
				item.Tick();
			}
		}
		[TDEvent.Tower.Attacked]
		public virtual void OnAttacked() {

		}
		public virtual void OnDeploy() {

		}

		public void LoadPrefab(string towerName) {
			Components.RemoveAll();
			TurretInstance.RecacheTurrets();
			if(TurretInstance.CachedInstances.ContainsKey(towerName)) {
				foreach(var item in TurretInstance.CachedInstances[towerName].Components) {
					Components.Add(item.Value);
				}
			} else
			if(FileSystem.Mounted.DirectoryExists("data/Turrets")) {
				if(FileSystem.Mounted.FileExists($"data/Turrets/{towerName.ToLower()}.turret")) {
					var data = FileSystem.Mounted.ReadAllText($"data/Turrets/{towerName.ToLower()}.turret");
					var instance = JsonSerializer.Deserialize<TurretInstance>(data);
					Debug.Info($"Loading {towerName}");
					foreach(var item in instance.SerializableComponents) {
						Debug.Info($"Loading {item.Key}");
						var comp = Components.Add(Library.Create<BaseTurretComponent>(Type.GetType(item.Key).Name));
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
					}
					if(Watcher == null) {
						Watcher = FileSystem.Mounted.Watch($"/data/Turrets/*");
						Watcher.OnChangedFile += (e) => {
							Debug.Info($"File changed: {e}");
							if(e.ToLower() == $"/data/Turrets/{towerName.ToLower()}.turret".ToLower()) {
								Debug.Info($"Reloading {towerName}");
								Components.RemoveAll();
								DelayedLoad(towerName);
							}
						};
					}


				} else {
					Debug.Error($"Failed to find turret prefab {towerName}");
					Delete();
				}
			} else {
				Debug.Error($"Turrets directory does not exist");
				Delete();
			}

		}
		private async void DelayedLoad(string tower) {
			await Task.Delay(500);
			LoadPrefab(tower);
		}

	}




}
