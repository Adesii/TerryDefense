using System;
using System.Collections.Generic;
using System.Linq;
using DZetko.Xml;
using Sandbox;

namespace TiledCS {
	/// <summary>
	/// Represents a Tiled tileset
	/// </summary>
	public class TiledTileset {
		/// <summary>
		/// The Tiled version used to create this tileset
		/// </summary>
		public string TiledVersion { get; set; }
		/// <summary>
		/// The tileset name
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// The tile width in pixels
		/// </summary>
		public int TileWidth { get; set; }
		/// <summary>
		/// The tile height in pixels
		/// </summary>
		public int TileHeight { get; set; }
		/// <summary>
		/// The total amount of tiles
		/// </summary>
		public int TileCount { get; set; }
		/// <summary>
		/// The amount of horizontal tiles
		/// </summary>
		public int Columns { get; set; }
		/// <summary>
		/// The image definition used by the tileset
		/// </summary>
		public TiledImage Image { get; set; }
		/// <summary>
		/// The amount of spacing between the tiles in pixels
		/// </summary>
		public int Spacing { get; set; }
		/// <summary>
		/// The amount of margin between the tiles in pixels
		/// </summary>
		public int Margin { get; set; }
		/// <summary>
		/// An array of tile definitions
		/// </summary>
		/// <remarks>Not all tiles within a tileset have definitions. Only those with properties, animations, terrains, ...</remarks>
		public TiledTile[] Tiles { get; set; }
		/// <summary>
		/// An array of terrain definitions
		/// </summary>
		public TiledTerrain[] Terrains { get; set; }
		/// <summary>
		/// An array of tileset properties
		/// </summary>
		public TiledProperty[] Properties { get; set; }

		/// <summary>
		/// Returns an empty instance of TiledTileset
		/// </summary>
		public TiledTileset() {

		}

		/// <summary>
		/// Loads a tileset in TSX format and parses it
		/// </summary>
		/// <param name="path">The file path of the TSX file</param>
		/// <exception cref="TiledException">Thrown when the file could not be found or parsed</exception>
		public TiledTileset(string path) {
			var content = "";

			// Check the file
			if(!FileSystem.Mounted.FileExists(path)) {
				throw new TiledException($"{path} not found");
			} else {
				content = FileSystem.Mounted.ReadAllText(path);
			}

			if(path.EndsWith(".tsx")) {
				ParseXml(content);
			} else {
				throw new TiledException("Unsupported file format");
			}
		}

		/// <summary>
		/// Can be used to parse the content of a TSX tileset manually instead of loading it using the constructor
		/// </summary>
		/// <param name="xml">The tmx file content as string</param>
		/// <exception cref="TiledException"></exception>
		public void ParseXml(string xml) {
			try {
				XmlParser idk = new(XmlParser.InputType.Text, xml);
				var document = idk.Parse();

				var nodeTileset = document.RootNode.GetChild("tileset");
				var nodeImage = nodeTileset.GetChild("image");
				var nodesTile = nodeTileset.GetChildren("tile");
				var nodesProperty = nodeTileset.GetChildren("properties/property");
				var nodesTerrain = nodeTileset.GetChildren("terraintypes/terrain");

				var attrMargin = nodeTileset["margin"];
				var attrSpacing = nodeTileset["spacing"];

				TiledVersion = nodeTileset["tiledversion"].Content;
				Name = nodeTileset["name"]?.Content;
				TileWidth = int.Parse(nodeTileset["tilewidth"].Content);
				TileHeight = int.Parse(nodeTileset["tileheight"].Content);
				TileCount = int.Parse(nodeTileset["tilecount"].Content);
				Columns = int.Parse(nodeTileset["columns"].Content);

				if(attrMargin != null) Margin = int.Parse(nodeTileset["margin"].Content);
				if(attrSpacing != null) Spacing = int.Parse(nodeTileset["spacing"].Content);
				if(nodeImage != null) Image = ParseImage(nodeImage);

				Tiles = ParseTiles((XmlElementList)nodesTile);
				Properties = ParseProperties((XmlElementList)nodesProperty);
				Terrains = ParseTerrains((XmlElementList)nodesTerrain);
			} catch(Exception ex) {
				throw new TiledException("Unable to parse xml data, make sure the xml data represents a valid Tiled tileset", ex);
			}
		}

		private TiledImage ParseImage(XmlElement node) {
			var tiledImage = new TiledImage();
			tiledImage.source = node["source"].Content;
			tiledImage.width = int.Parse(node["width"].Content);
			tiledImage.height = int.Parse(node["height"].Content);

			return tiledImage;
		}

		private TiledTileAnimation[] ParseAnimations(XmlElementList nodeList) {
			var result = new List<TiledTileAnimation>();

			foreach(XmlElement node in nodeList) {
				var animation = new TiledTileAnimation();
				animation.tileid = int.Parse(node["tileid"].Content);
				animation.duration = int.Parse(node["duration"].Content);

				result.Add(animation);
			}

			return result.ToArray();
		}

		private TiledProperty[] ParseProperties(XmlElementList nodeList) {
			var result = new List<TiledProperty>();

			foreach(XmlElement node in nodeList) {
				var property = new TiledProperty();
				property.name = node["name"].Content;
				property.type = node["type"]?.Content;
				property.value = node["value"]?.Content;

				if(property.value == null && node.Content != null) {
					property.value = node.Content;
				}

				result.Add(property);
			}

			return result.ToArray();
		}

		private TiledTile[] ParseTiles(XmlElementList nodeList) {
			var result = new List<TiledTile>();

			foreach(XmlElement node in nodeList) {
				XmlElementList nodesProperty = (XmlElementList)node.GetChildren("properties/property");
				XmlElementList nodesAnimation = (XmlElementList)node.GetChildren("animation/frame");
				var nodeImage = node.GetChild("image");

				var tile = new TiledTile();
				tile.id = int.Parse(node["id"].Content);
				tile.type = node["type"]?.Content;
				tile.terrain = node["terrain"]?.Content.Split(',').AsIntArray();
				tile.properties = ParseProperties(nodesProperty);
				tile.animation = ParseAnimations(nodesAnimation);

				if(nodeImage != null) {
					var tileImage = new TiledImage();
					tileImage.width = int.Parse(nodeImage["width"].Content);
					tileImage.height = int.Parse(nodeImage["height"].Content);
					tileImage.source = nodeImage["source"].Content;

					tile.image = tileImage;
				}

				result.Add(tile);
			}

			return result.ToArray();
		}

		private TiledTerrain[] ParseTerrains(XmlElementList nodeList) {
			var result = new List<TiledTerrain>();

			foreach(XmlElement node in nodeList) {
				var terrain = new TiledTerrain();
				terrain.name = node["name"].Content;
				terrain.tile = int.Parse(node["tile"].Content);

				result.Add(terrain);
			}

			return result.ToArray();
		}
	}
}
