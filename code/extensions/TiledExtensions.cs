using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using DZetko.Xml;
using Sandbox;
using TiledCS;


namespace TerryDefense {
	public static class TiledExtensions {
		public const string ObjectsPath = "data/maps/objecttypes.xml";
		public static Dictionary<string, TileObjectTypes> tileObjectTypes;
		public static string GetCustomProperty(this TiledObject obj, string name) {
			if(obj.properties != null)
				foreach(var prop in obj.properties) {
					if(prop.name == name)
						return prop.value;
				}
			if(tileObjectTypes == null) {
				ParseObjecTypes();
			}
			if(tileObjectTypes.TryGetValue(obj.type, out var types)) {
				foreach(var prop in types.properties) {
					if(prop.name == name) {
						//Log.Error($"{obj.type} has property {prop.name} with value {prop.value}");
						return prop.value;
					}
				}
			}
			return null;
		}

		public class TileObjectTypes {
			public List<TiledProperty> properties;
		}

		public static void ParseObjecTypes() {
			var content = "";

			// Check the file
			if(!FileSystem.Mounted.FileExists(ObjectsPath)) {
				throw new TiledException($"{ObjectsPath} not found");
			} else {
				content = FileSystem.Mounted.ReadAllText(ObjectsPath);
			}

			XmlParser idk = new(XmlParser.InputType.Text, content);
			var document = idk.Parse();

			var objecttypes = document.RootNode.GetChildren("objecttype");
			tileObjectTypes = new();
			foreach(var item in objecttypes) {
				tileObjectTypes.Add(item["name"].Content, new TileObjectTypes {
					properties = ParseProperties(item.Children).ToList()
				});
			}

		}
		private static TiledProperty[] ParseProperties(XmlElementList nodeList) {
			var result = new List<TiledProperty>();

			foreach(XmlElement node in nodeList) {
				var property = new TiledProperty();
				property.name = node["name"].Content;
				property.type = node["type"]?.Content;
				property.value = node["default"]?.Content;

				//Log.Error($"{property.name} {property.type} {property.value}");

				if(property.value == null && node.Content != null) {
					property.value = node.Content;
				}

				result.Add(property);
			}

			return result.ToArray();
		}
	}
}
