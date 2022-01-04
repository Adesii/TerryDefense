using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TerryDefense.Towers {
	public class TurretInstance {
		public string SaveLocation = "";
		public string Name { get; set; } = "TestName";
		public string Description { get; set; }



		[JsonConverter(typeof(DictionaryStringObjectJsonConverter))]
		public Dictionary<string, object> SerializableComponents { get; set; } = new();
	}
}
