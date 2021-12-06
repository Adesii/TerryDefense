using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using DZetko.Xml;
using Sandbox;

namespace TiledCS {
	/// <summary>
	/// Represents a Tiled map
	/// </summary>
	public class TiledMap {
		const uint FLIPPED_HORIZONTALLY_FLAG = 0b10000000000000000000000000000000;
		const uint FLIPPED_VERTICALLY_FLAG = 0b01000000000000000000000000000000;
		const uint FLIPPED_DIAGONALLY_FLAG = 0b00100000000000000000000000000000;

		/// <summary>
		/// How many times we shift the FLIPPED flags to the right in order to store it in a byte.
		/// For example: 0b10100000000000000000000000000000 >> SHIFT_FLIP_FLAG_TO_BYTE = 0b00000101
		/// </summary>
		const int SHIFT_FLIP_FLAG_TO_BYTE = 29;

		/// <summary>
		/// Returns the Tiled version used to create this map
		/// </summary>
		public string TiledVersion { get; set; }
		/// <summary>
		/// Returns an array of properties defined in the map
		/// </summary>
		public TiledProperty[] Properties { get; set; }
		/// <summary>
		/// Returns an array of tileset definitions in the map
		/// </summary>
		public TiledMapTileset[] Tilesets { get; set; }
		/// <summary>
		/// Returns an array of layers or null if none were defined
		/// </summary>
		public TiledLayer[] Layers { get; set; }
		/// <summary>
		/// Returns an array of groups or null if none were defined
		/// </summary>
		public TiledGroup[] Groups { get; set; }
		/// <summary>
		/// Returns the defined map orientation as a string
		/// </summary>
		public string Orientation { get; set; }
		/// <summary>
		/// Returns the render order as a string
		/// </summary>
		public string RenderOrder { get; set; }
		/// <summary>
		/// The amount of horizontal tiles
		/// </summary>
		public int Width { get; set; }
		/// <summary>
		/// The amount of vertical tiles
		/// </summary>
		public int Height { get; set; }
		/// <summary>
		/// The tile width in pixels
		/// </summary>
		public int TileWidth { get; set; }
		/// <summary>
		/// The tile height in pixels
		/// </summary>
		public int TileHeight { get; set; }
		/// <summary>
		/// Returns true if the map is configured as infinite
		/// </summary>
		public bool Infinite { get; set; }
		/// <summary>
		/// Returns the defined map background color as a hex string
		/// </summary>
		public string BackgroundColor { get; set; }
		/// <summary>
		/// Returns an empty instance of TiledMap
		/// </summary>
		public TiledMap() {

		}

		/// <summary>
		/// Loads a Tiled map in TMX format and parses it
		/// </summary>
		/// <param name="path">The path to the tmx file</param>
		/// <exception cref="TiledException">Thrown when the map could not be loaded or is not in a correct format</exception>
		public TiledMap(string path) {
			var content = "";

			// Check the file
			if(!FileSystem.Mounted.FileExists(path)) {
				throw new TiledException($"{path} not found");
			} else {
				content = FileSystem.Mounted.ReadAllText(path);
			}

			if(path.EndsWith(".tmx")) {
				ParseXml(content);
			} else {
				throw new TiledException("Unsupported file format");
			}
		}

		/// <summary>
		/// Can be used to parse the content of a TMX map manually instead of loading it using the constructor
		/// </summary>
		/// <param name="xml">The tmx file content as string</param>
		/// <exception cref="TiledException"></exception>
		public void ParseXml(string xml) {
			try {
				// Load the xml document
				XmlParser idk = new(XmlParser.InputType.Text, xml);
				var document = idk.Parse();

				var nodeMap = document.RootNode;
				var nodesProperty = nodeMap.GetChild("properties")?.GetChildren("property");
				var nodesLayer = nodeMap.GetChildren("layer");
				var nodesImageLayer = nodeMap.GetChildren("imagelayer");
				var nodesObjectGroup = nodeMap.GetChildren("objectgroup");
				var nodesTileset = nodeMap.GetChildren("tileset");
				var nodesGroup = nodeMap.GetChildren("group");


				this.TiledVersion = nodeMap["tiledversion"].Content;
				this.Orientation = nodeMap["orientation"].Content;
				this.RenderOrder = nodeMap["renderorder"].Content;
				this.BackgroundColor = nodeMap["backgroundcolor"]?.Content;
				this.Infinite = nodeMap["infinite"].Content == "1";

				this.Width = int.Parse(nodeMap["width"].Content);
				this.Height = int.Parse(nodeMap["height"].Content);
				this.TileWidth = int.Parse(nodeMap["tilewidth"].Content);
				this.TileHeight = int.Parse(nodeMap["tileheight"].Content);

				if(nodesProperty != null) Properties = ParseProperties(nodesProperty);
				if(nodesTileset != null) Tilesets = ParseTilesets(nodesTileset);
				if(nodesLayer != null) Layers = ParseLayers(nodesLayer, nodesObjectGroup, nodesImageLayer);
				if(nodesGroup != null) Groups = ParseGroups(nodesGroup);
			} catch(Exception ex) {
				throw new TiledException("Unable to parse xml data, make sure the xml data represents a valid Tiled map", ex);
			}
		}

		private TiledProperty[] ParseProperties(List<XmlElement> nodeList) {
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

		private TiledMapTileset[] ParseTilesets(List<XmlElement> nodeList) {
			var result = new List<TiledMapTileset>();

			foreach(XmlElement node in nodeList) {
				var tileset = new TiledMapTileset();
				tileset.firstgid = int.Parse(node["firstgid"].Content);
				tileset.source = node["source"].Content;

				result.Add(tileset);
			}

			return result.ToArray();
		}

		private TiledGroup[] ParseGroups(List<XmlElement> nodeListGroups) {
			var result = new List<TiledGroup>();

			foreach(XmlElement node in nodeListGroups) {
				var nodesProperty = node.GetChildren("properties/property");
				var nodesGroup = node.GetChildren("group");
				var nodesLayer = node.GetChildren("layer");
				var nodesObjectGroup = node.GetChildren("objectgroup");
				var nodesImageLayer = node.GetChildren("imagelayer");
				var attrVisible = node["visible"];
				var attrLocked = node["locked"];

				var tiledGroup = new TiledGroup();
				tiledGroup.id = int.Parse(node["id"].Content);
				tiledGroup.name = node["name"].Content;

				if(attrVisible != null) tiledGroup.visible = attrVisible.Content == "1";
				if(attrLocked != null) tiledGroup.locked = attrLocked.Content == "1";
				if(nodesProperty != null) tiledGroup.properties = ParseProperties(nodesProperty);
				if(nodesGroup != null) tiledGroup.groups = ParseGroups(nodesGroup);
				if(nodesLayer != null) tiledGroup.layers = ParseLayers(nodesLayer, nodesObjectGroup, nodesImageLayer);

				result.Add(tiledGroup);
			}

			return result.ToArray();
		}
		private TiledLayer[] ParseLayers(List<XmlElement> nodesLayer, List<XmlElement> nodesObjectGroup, List<XmlElement> nodesImageLayer) {
			var result = new List<TiledLayer>();

			foreach(XmlElement node in nodesLayer) {
				var nodeData = node.GetChild("data");
				var nodesProperty = node.GetChild("properties").GetChildren("property");
				var encoding = nodeData["encoding"].Content;
				var attrVisible = node["visible"];
				var attrLocked = node["locked"];
				var attrTint = node["tintcolor"];
				var attrOffsetX = node["offsetx"];
				var attrOffsetY = node["offsety"];


				var tiledLayer = new TiledLayer();
				tiledLayer.id = int.Parse(node["id"].Content);
				tiledLayer.name = node["name"].Content;
				tiledLayer.height = int.Parse(node["height"].Content);
				tiledLayer.width = int.Parse(node["width"].Content);
				tiledLayer.type = "tilelayer";
				tiledLayer.visible = true;

				if(attrVisible != null) tiledLayer.visible = attrVisible.Content == "1";
				if(attrLocked != null) tiledLayer.locked = attrLocked.Content == "1";
				if(attrTint != null) tiledLayer.tintcolor = attrTint.Content;
				if(attrOffsetX != null) tiledLayer.offsetX = int.Parse(attrOffsetX.Content);
				if(attrOffsetY != null) tiledLayer.offsetY = int.Parse(attrOffsetY.Content);
				if(nodesProperty != null) tiledLayer.properties = ParseProperties(nodesProperty);

				if(encoding == "csv") {
					var csvs = nodeData.Content.Split(',');

					tiledLayer.data = new int[csvs.Length];
					tiledLayer.dataRotationFlags = new byte[csvs.Length];

					// Parse the comma separated csv string and update the inner data as well as the data rotation flags
					for(var i = 0; i < csvs.Length; i++) {
						var rawID = uint.Parse(csvs[i]);
						var hor = ((rawID & FLIPPED_HORIZONTALLY_FLAG));
						var ver = ((rawID & FLIPPED_VERTICALLY_FLAG));
						var dia = ((rawID & FLIPPED_DIAGONALLY_FLAG));
						tiledLayer.dataRotationFlags[i] = (byte)((hor | ver | dia) >> SHIFT_FLIP_FLAG_TO_BYTE);

						// assign data to rawID with the rotation flags cleared
						tiledLayer.data[i] = (int)(rawID & ~(FLIPPED_HORIZONTALLY_FLAG | FLIPPED_VERTICALLY_FLAG | FLIPPED_DIAGONALLY_FLAG));
					}
				} else if(encoding == "base64") {
					var compression = nodeData["compression"]?.Content;

					using(var base64DataStream = new MemoryStream(Convert.FromBase64String(nodeData.Content))) {
						if(compression == null) {
							// Parse the decoded bytes and update the inner data as well as the data rotation flags
							var rawBytes = new byte[4];
							tiledLayer.data = new int[base64DataStream.Length];
							tiledLayer.dataRotationFlags = new byte[base64DataStream.Length];

							for(var i = 0; i < base64DataStream.Length; i++) {
								base64DataStream.Read(rawBytes, 0, rawBytes.Length);
								var rawID = BitConverter.ToUInt32(rawBytes, 0);
								var hor = ((rawID & FLIPPED_HORIZONTALLY_FLAG));
								var ver = ((rawID & FLIPPED_VERTICALLY_FLAG));
								var dia = ((rawID & FLIPPED_DIAGONALLY_FLAG));
								tiledLayer.dataRotationFlags[i] = (byte)((hor | ver | dia) >> SHIFT_FLIP_FLAG_TO_BYTE);

								// assign data to rawID with the rotation flags cleared
								tiledLayer.data[i] = (int)(rawID & ~(FLIPPED_HORIZONTALLY_FLAG | FLIPPED_VERTICALLY_FLAG | FLIPPED_DIAGONALLY_FLAG));
							}
						} else if(compression == "zlib") {
							// .NET doesn't play well with the headered zlib data that Tiled produces,
							// so we have to manually skip the 2-byte header to get what DeflateStream's looking for
							// Should an external library be used instead of this hack?
							base64DataStream.ReadByte();
							base64DataStream.ReadByte();

							using(var decompressionStream = new DeflateStream(base64DataStream, CompressionMode.Decompress)) {
								// Parse the raw decompressed bytes and update the inner data as well as the data rotation flags
								var decompressedDataBuffer = new byte[4]; // size of each tile
								var dataRotationFlagsList = new List<byte>();
								var layerDataList = new List<int>();

								while(decompressionStream.Read(decompressedDataBuffer, 0, decompressedDataBuffer.Length) == decompressedDataBuffer.Length) {
									var rawID = BitConverter.ToUInt32(decompressedDataBuffer, 0);
									var hor = ((rawID & FLIPPED_HORIZONTALLY_FLAG));
									var ver = ((rawID & FLIPPED_VERTICALLY_FLAG));
									var dia = ((rawID & FLIPPED_DIAGONALLY_FLAG));
									dataRotationFlagsList.Add((byte)((hor | ver | dia) >> SHIFT_FLIP_FLAG_TO_BYTE));

									// assign data to rawID with the rotation flags cleared
									layerDataList.Add((int)(rawID & ~(FLIPPED_HORIZONTALLY_FLAG | FLIPPED_VERTICALLY_FLAG | FLIPPED_DIAGONALLY_FLAG)));
								}

								tiledLayer.data = layerDataList.ToArray();
								tiledLayer.dataRotationFlags = dataRotationFlagsList.ToArray();
							}
						} else if(compression == "gzip") {
							using(var decompressionStream = new GZipStream(base64DataStream, CompressionMode.Decompress)) {
								// Parse the raw decompressed bytes and update the inner data as well as the data rotation flags
								var decompressedDataBuffer = new byte[4]; // size of each tile
								var dataRotationFlagsList = new List<byte>();
								var layerDataList = new List<int>();

								while(decompressionStream.Read(decompressedDataBuffer, 0, decompressedDataBuffer.Length) == decompressedDataBuffer.Length) {
									var rawID = BitConverter.ToUInt32(decompressedDataBuffer, 0);
									var hor = ((rawID & FLIPPED_HORIZONTALLY_FLAG));
									var ver = ((rawID & FLIPPED_VERTICALLY_FLAG));
									var dia = ((rawID & FLIPPED_DIAGONALLY_FLAG));

									dataRotationFlagsList.Add((byte)((hor | ver | dia) >> SHIFT_FLIP_FLAG_TO_BYTE));

									// assign data to rawID with the rotation flags cleared
									layerDataList.Add((int)(rawID & ~(FLIPPED_HORIZONTALLY_FLAG | FLIPPED_VERTICALLY_FLAG | FLIPPED_DIAGONALLY_FLAG)));
								}

								tiledLayer.data = layerDataList.ToArray();
								tiledLayer.dataRotationFlags = dataRotationFlagsList.ToArray();
							}
						} else {
							throw new TiledException("Zstandard compression is currently not supported");
						}
					}
				} else {
					throw new TiledException("Only CSV and Base64 encodings are currently supported");
				}

				result.Add(tiledLayer);
			}

			foreach(XmlElement node in nodesObjectGroup) {
				var nodesProperty = node.GetChild("properties")?.GetChildren("property");
				var nodesObject = node.GetChildren("object");
				var attrVisible = node["visible"];
				var attrLocked = node["locked"];
				var attrTint = node["tintcolor"];
				var attrOffsetX = node["offsetx"];
				var attrOffsetY = node["offsety"];

				var tiledLayer = new TiledLayer();
				tiledLayer.id = int.Parse(node["id"].Content);
				tiledLayer.name = node["name"].Content;
				tiledLayer.objects = ParseObjects(nodesObject);
				tiledLayer.type = "objectgroup";
				tiledLayer.visible = true;

				if(attrVisible != null) tiledLayer.visible = attrVisible.Content == "1";
				if(attrLocked != null) tiledLayer.locked = attrLocked.Content == "1";
				if(attrTint != null) tiledLayer.tintcolor = attrTint.Content;
				if(attrOffsetX != null) tiledLayer.offsetX = int.Parse(attrOffsetX.Content);
				if(attrOffsetY != null) tiledLayer.offsetY = int.Parse(attrOffsetY.Content);
				if(nodesProperty != null) tiledLayer.properties = ParseProperties(nodesProperty);

				result.Add(tiledLayer);
			}

			foreach(XmlElement node in nodesImageLayer) {
				var nodesProperty = node.GetChild("properties")?.GetChildren("property");
				var nodeImage = node.GetChild("image");
				var attrVisible = node["visible"];
				var attrLocked = node["locked"];
				var attrTint = node["tintcolor"];
				var attrOffsetX = node["offsetx"];
				var attrOffsetY = node["offsety"];

				var tiledLayer = new TiledLayer();
				tiledLayer.id = int.Parse(node["id"].Content);
				tiledLayer.name = node["name"].Content;
				tiledLayer.type = "imagelayer";
				tiledLayer.visible = true;

				if(attrVisible != null) tiledLayer.visible = attrVisible.Content == "1";
				if(attrLocked != null) tiledLayer.locked = attrLocked.Content == "1";
				if(attrTint != null) tiledLayer.tintcolor = attrTint.Content;
				if(attrOffsetX != null) tiledLayer.offsetX = int.Parse(attrOffsetX.Content);
				if(attrOffsetY != null) tiledLayer.offsetY = int.Parse(attrOffsetY.Content);
				if(nodesProperty != null) tiledLayer.properties = ParseProperties(nodesProperty);
				if(nodeImage != null) tiledLayer.image = ParseImage(nodeImage);

				result.Add(tiledLayer);
			}

			return result.ToArray();
		}

		private TiledImage ParseImage(XmlElement node) {
			var tiledImage = new TiledImage();
			tiledImage.source = node["source"].Content;
			tiledImage.width = int.Parse(node["width"].Content);
			tiledImage.height = int.Parse(node["height"].Content);

			return tiledImage;
		}

		private TiledObject[] ParseObjects(List<XmlElement> nodeList) {
			var result = new List<TiledObject>();

			foreach(XmlElement node in nodeList) {
				var nodesProperty = node.GetChild("properties")?.GetChildren("property");
				var nodePolygon = node.GetChild("polygon");
				var nodePoint = node.GetChild("point");
				var nodeEllipse = node.GetChild("ellipse");

				var obj = new TiledObject();
				obj.id = int.Parse(node["id"].Content);
				obj.name = node["name"]?.Content;
				obj.type = node["type"]?.Content;
				obj.gid = int.Parse(node["gid"]?.Content ?? "0");
				obj.x = float.Parse(node["x"].Content, CultureInfo.InvariantCulture);
				obj.y = float.Parse(node["y"].Content, CultureInfo.InvariantCulture);

				if(nodesProperty != null) {
					obj.properties = ParseProperties(nodesProperty);
				}

				if(nodePolygon != null) {
					var points = nodePolygon["points"].Content;
					var vertices = points.Split(' ');

					var polygon = new TiledPolygon();
					polygon.points = new float[vertices.Length * 2];

					for(var i = 0; i < vertices.Length; i++) {
						polygon.points[(i * 2) + 0] = float.Parse(vertices[i].Split(',')[0], CultureInfo.InvariantCulture);
						polygon.points[(i * 2) + 1] = float.Parse(vertices[i].Split(',')[1], CultureInfo.InvariantCulture);
					}

					obj.polygon = polygon;
				}

				if(nodeEllipse != null) {
					obj.ellipse = new TiledEllipse();
				}

				if(nodePoint != null) {
					obj.point = new TiledPoint();
				}

				if(node["width"] != null) {
					obj.width = float.Parse(node["width"].Content, CultureInfo.InvariantCulture);
				}

				if(node["height"] != null) {
					obj.height = float.Parse(node["height"].Content, CultureInfo.InvariantCulture);
				}

				if(node["rotation"] != null) {
					obj.rotation = int.Parse(node["rotation"].Content);
				}

				result.Add(obj);
			}

			return result.ToArray();
		}

		/* HELPER METHODS */
		/// <summary>
		/// Locates the right TiledMapTileset object for you within the Tilesets array
		/// </summary>
		/// <param name="gid">A value from the TiledLayer.data array</param>
		/// <returns>An element within the Tilesets array or null if no match was found</returns>
		public TiledMapTileset GetTiledMapTileset(int gid) {
			if(Tilesets == null) {
				return null;
			}

			for(var i = 0; i < Tilesets.Length; i++) {
				if(i < Tilesets.Length - 1) {
					int gid1 = Tilesets[i + 0].firstgid;
					int gid2 = Tilesets[i + 1].firstgid;

					if(gid >= gid1 && gid < gid2) {
						return Tilesets[i];
					}
				} else {
					return Tilesets[i];
				}
			}

			return new TiledMapTileset();
		}
		/* /// <summary>
		/// Loads external tilesets and matches them to firstGids from elements within the Tilesets array
		/// </summary>
		/// <param name="src">The folder where the TiledMap file is located</param>
		/// <returns>A dictionary where the key represents the firstGid of the associated TiledMapTileset and the value the TiledTileset object</returns>
		public Dictionary<int, TiledTileset> GetTiledTilesets(string src) {
			var tilesets = new Dictionary<int, TiledTileset>();
			var info = new FileInfo(src);
			var srcFolder = info.Directory;

			if(Tilesets == null) {
				return tilesets;
			}

			foreach(var mapTileset in Tilesets) {
				var path = $"{srcFolder}/{mapTileset.source}";

				if(FileSystem.Mounted.FileExists(path)) {
					tilesets.Add(mapTileset.firstgid, new TiledTileset(path));
				}
			}

			return tilesets;
		} */
		/// <summary>
		/// Locates a specific TiledTile object
		/// </summary>
		/// <param name="mapTileset">An element within the Tilesets array</param>
		/// <param name="tileset">An instance of the TiledTileset class</param>
		/// <param name="gid">An element from within a TiledLayer.data array</param>
		/// <returns>An entry of the TiledTileset.tiles array or null if none of the tile id's matches the gid</returns>
		/// <remarks>Tip: Use the GetTiledMapTileset and GetTiledTilesets methods for retrieving the correct TiledMapTileset and TiledTileset objects</remarks>
		public TiledTile GetTiledTile(TiledMapTileset mapTileset, TiledTileset tileset, int gid) {
			foreach(var tile in tileset.Tiles) {
				if(tile.id == gid - mapTileset.firstgid) {
					return tile;
				}
			}

			return null;
		}
		/// <summary>
		/// This method can be used to figure out the x and y position on a Tileset image for rendering tiles. 
		/// </summary>
		/// <param name="mapTileset">An element of the Tilesets array</param>
		/// <param name="tileset">An instance of the TiledTileset class</param>
		/// <param name="gid">An element within a TiledLayer.data array</param>
		/// <returns>An int array of length 2 containing the x and y position of the source rect of the tileset image. Multiply the values by the tile width and height in pixels to get the actual x and y position. Returns null if the gid was not found</returns>
		/// <remarks>This method currently doesn't take margin into account</remarks>
		[Obsolete("Please use GetSourceRect instead because with future versions of Tiled this method may no longer be sufficient")]
		public int[] GetSourceVector(TiledMapTileset mapTileset, TiledTileset tileset, int gid) {
			var tileHor = 0;
			var tileVert = 0;

			for(var i = 0; i < tileset.TileCount; i++) {
				if(i == gid - mapTileset.firstgid) {
					return new[] { tileHor, tileVert };
				}

				// Update x and y position
				tileHor++;

				if(tileHor == tileset.Image.width / tileset.TileWidth) {
					tileHor = 0;
					tileVert++;
				}
			}

			return null;
		}

		/// <summary>
		/// This method can be used to figure out the source rect on a Tileset image for rendering tiles.
		/// </summary>
		/// <param name="mapTileset"></param>
		/// <param name="tileset"></param>
		/// <param name="gid"></param>
		/// <returns>An instance of the class TiledSourceRect that represents a rectangle. Returns null if the provided gid was not found within the tileset.</returns>
		public TiledSourceRect GetSourceRect(TiledMapTileset mapTileset, TiledTileset tileset, int gid) {
			var tileHor = 0;
			var tileVert = 0;

			for(var i = 0; i < tileset.TileCount; i++) {
				if(i == gid - mapTileset.firstgid) {
					var result = new TiledSourceRect();
					result.x = tileHor * tileset.TileWidth;
					result.y = tileVert * tileset.TileHeight;
					result.width = tileset.TileWidth;
					result.height = tileset.TileHeight;

					return result;
				}

				// Update x and y position
				tileHor++;

				if(tileHor == tileset.Image.width / tileset.TileWidth) {
					tileHor = 0;
					tileVert++;
				}
			}

			return null;
		}

		/// <summary>
		/// Checks is a tile is flipped horizontally
		/// </summary>
		/// <param name="layer">An entry of the TiledMap.layers array</param>
		/// <param name="tileHor">The tile's horizontal position</param>
		/// <param name="tileVert">The tile's vertical position</param>
		/// <returns>True if the tile was flipped horizontally or False if not</returns>
		public bool IsTileFlippedHorizontal(TiledLayer layer, int tileHor, int tileVert) {
			return IsTileFlippedHorizontal(layer, tileHor + (tileVert * layer.width));
		}
		/// <summary>
		/// Checks is a tile is flipped horizontally
		/// </summary>
		/// <param name="layer">An entry of the TiledMap.layers array</param>
		/// <param name="dataIndex">An index of the TiledLayer.data array</param>
		/// <returns>True if the tile was flipped horizontally or False if not</returns>
		public bool IsTileFlippedHorizontal(TiledLayer layer, int dataIndex) {
			return (layer.dataRotationFlags[dataIndex] & (FLIPPED_HORIZONTALLY_FLAG >> SHIFT_FLIP_FLAG_TO_BYTE)) > 0;
		}
		/// <summary>
		/// Checks is a tile is flipped vertically
		/// </summary>
		/// <param name="layer">An entry of the TiledMap.layers array</param>
		/// <param name="tileHor">The tile's horizontal position</param>
		/// <param name="tileVert">The tile's vertical position</param>
		/// <returns>True if the tile was flipped vertically or False if not</returns>
		public bool IsTileFlippedVertical(TiledLayer layer, int tileHor, int tileVert) {
			return IsTileFlippedVertical(layer, tileHor + (tileVert * layer.width));
		}
		/// <summary>
		/// Checks is a tile is flipped vertically
		/// </summary>
		/// <param name="layer">An entry of the TiledMap.layers array</param>
		/// <param name="dataIndex">An index of the TiledLayer.data array</param>
		/// <returns>True if the tile was flipped vertically or False if not</returns>
		public bool IsTileFlippedVertical(TiledLayer layer, int dataIndex) {
			return (layer.dataRotationFlags[dataIndex] & (FLIPPED_VERTICALLY_FLAG >> SHIFT_FLIP_FLAG_TO_BYTE)) > 0;
		}
		/// <summary>
		/// Checks is a tile is flipped diagonally
		/// </summary>
		/// <param name="layer">An entry of the TiledMap.layers array</param>
		/// <param name="tileHor">The tile's horizontal position</param>
		/// <param name="tileVert">The tile's vertical position</param>
		/// <returns>True if the tile was flipped diagonally or False if not</returns>
		public bool IsTileFlippedDiagonal(TiledLayer layer, int tileHor, int tileVert) {
			return IsTileFlippedDiagonal(layer, tileHor + (tileVert * layer.width));
		}
		/// <summary>
		/// Checks is a tile is flipped diagonally
		/// </summary>
		/// <param name="layer">An entry of the TiledMap.layers array</param>
		/// <param name="dataIndex">An index of the TiledLayer.data array</param>
		/// <returns>True if the tile was flipped diagonally or False if not</returns>
		public bool IsTileFlippedDiagonal(TiledLayer layer, int dataIndex) {
			return (layer.dataRotationFlags[dataIndex] & (FLIPPED_DIAGONALLY_FLAG >> SHIFT_FLIP_FLAG_TO_BYTE)) > 0;
		}
	}
}
