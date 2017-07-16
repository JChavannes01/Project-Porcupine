using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class World {

	Tile[,] tiles;

	int width;
	int height;
	public int Width { get { return width;}}
	public int Height { get { return height;}}

	Dictionary<string, Furniture> furniturePrototypes;

	Action<Furniture> cbFurnitureCreated;

	// Creates a new instance of World with a default tilegrid size of 100x100
	public World (int width=100, int height=100) {
		this.width = width;
		this.height = height;
		tiles = new Tile[width, height];

		for (int i = 0; i < width; i++) {
			for (int j = 0; j < height; j++) {
				tiles [i, j] = new Tile(this, i, j);
			}
		}

		Debug.Log ("World created with " + (width * height) + " Tiles.");

		CreateFurniturePrototypes ();
	}

	void CreateFurniturePrototypes() {
		furniturePrototypes = new Dictionary<string, Furniture> ();

			furniturePrototypes.Add ("Wall", Furniture.CreatePrototype (
										"Wall", // Internal ID
										0, 		 // Movement resistance
										1, 		 // Width
										1,		 // Height
										true)	 // Links to neighbour.
									);
	}


	public void RandomizeTiles() {
		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				if (UnityEngine.Random.Range(0, 2) == 0) {
					tiles [x, y].Type = Tile.TileType.Empty;
				} else {
					tiles [x, y].Type = Tile.TileType.Floor;
				}
			}
		}
	}

	public void PlaceFurniture(string type, Tile tile) {
		// TODO: this assumes 1x1 objects. Change for larger objects
		if (furniturePrototypes.ContainsKey (type) == false) {
			Debug.LogError ("furniturePrototypes doesn't contain a prototype for key: " + type);
			return;
		}
		Furniture obj = Furniture.PlaceInstance (furniturePrototypes [type], tile);

		if (obj == null) {
			// Failed to place object, most likely there was already something there.
			return;
		}

		if (cbFurnitureCreated != null) {
			cbFurnitureCreated (obj);
		}
	}

	public Tile GetTileAt(int x, int y) {
		if (x < 0 || x >= width || y < 0 || y >= height) {
//			Debug.LogError ("Tile ("+x+","+y+") is out of range.");
			return null;
		}
		return tiles [x, y];
	}

	public void RegisterOnFurnitureCreatedCallBack(Action<Furniture> callback) {
		cbFurnitureCreated += callback;
	}

	public void UnregisterOnFurnitureCreatedCallBack(Action<Furniture> callback) {
		cbFurnitureCreated -= callback;
	}
}
