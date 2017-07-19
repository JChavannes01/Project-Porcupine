using System.Collections.Generic;
using UnityEngine;
using System;

public class World {

	Tile[,] tiles;
	List<Character> characters;

	public int Width { get; protected set;}
	public int Height { get; protected set;}

	Dictionary<string, Furniture> furniturePrototypes;

	Action<Furniture> cbFurnitureCreated;
	Action<Character> cbCharacterCreated;
	Action<Tile> cbTileChanged;

	public JobQueue jobQueue;

	// Creates a new instance of World with a default tilegrid size of 100x100
	public World (int width=100, int height=100) {
		jobQueue = new JobQueue ();

		this.Width = width;
		this.Height = height;

		tiles = new Tile[width, height];

		for (int i = 0; i < width; i++) {
			for (int j = 0; j < height; j++) {
				tiles [i, j] = new Tile(this, i, j);
				tiles [i, j].RegisterTileTypeChangedCallBack (OnTileChanged);
			}
		}

		Debug.Log ("World created with " + (width * height) + " Tiles.");

		CreateFurniturePrototypes ();

		characters = new List<Character> ();
	}

	public void Update(float deltaTime) {
		foreach (Character c in characters) {
			c.Update (deltaTime);
		}
	}

	public Character createCharacter (Tile t) {
		Character c = new Character (t);
		characters.Add (c);

		if (cbCharacterCreated != null) {
			cbCharacterCreated (c);
		}
		return c;
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
		for (int x = 0; x < Width; x++) {
			for (int y = 0; y < Height; y++) {
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
		if (x < 0 || x >= Width || y < 0 || y >= Height) {
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

	public void RegisterOnCharacterCreatedCallBack(Action<Character> callback) {
		cbCharacterCreated += callback;
	}

	public void UnregisterOnCharacterCreatedCallBack(Action<Character> callback) {
		cbCharacterCreated -= callback;
	}

	public void RegisterOnTileChangedCallBack(Action<Tile> callback) {
		cbTileChanged += callback;
	}

	public void UnregisterOnTileChangedCallBack(Action<Tile> callback) {
		cbTileChanged -= callback;
	}

	public void OnTileChanged (Tile t) {
		if (cbTileChanged == null) {
			return;
		}

		cbTileChanged (t);
	}

	//FIXME: These functions should never be called directly.
	public bool IsFurniturePlacementValid(string furnType, Tile t) {
		if (furniturePrototypes.ContainsKey (furnType) == false) {
			return false;
		}
		return furniturePrototypes [furnType].IsValidPosition (t);
	}

	public Furniture getFurniturePrototype(string objectType) {
		if (furniturePrototypes.ContainsKey (objectType) == false) {
			Debug.LogError ("No furniture with type: " + objectType);
			return null;
		}
		return furniturePrototypes [objectType];
	}
}
