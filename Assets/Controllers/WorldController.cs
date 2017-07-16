using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldController : MonoBehaviour {
	public static WorldController Instance { get; protected set;}

	public World World { get; protected set; }
	public GameObject tilesContainer;

	public Sprite floorSprite;
	public Sprite emptySprite;

	Dictionary<Furniture, GameObject> furnitureGameObjectMap;
	Dictionary<string, Sprite> furnitureSprites;

	// Use this for initialization
	void Start () {
		if (Instance != null) {
			Debug.LogError ("There should only ever be one instance of WorldController.");
		}
		Instance = this;

		World = new World ();
		furnitureGameObjectMap = new Dictionary<Furniture, GameObject>();

		// Fill our spite dictionary
		furnitureSprites = new Dictionary<string, Sprite> ();
		LoadSprites ();

		// Center the camera on the world.
		Camera.main.transform.position =  new Vector3 (World.Width / 2, World.Height / 2, Camera.main.transform.position.z);

		World.RegisterOnFurnitureCreatedCallBack (OnFurnitureCreated);

		for (int x = 0; x < World.Width; x++) {
			for (int y = 0; y < World.Height; y++) {
				// Data object of the tile.
				Tile tile_data = World.GetTileAt (x, y);

				// Create new gameobject for a tile
				GameObject tile_go = new GameObject();
				tile_go.name = "Tile_" + x + "_" + y;
				tile_go.transform.position = new Vector3 (tile_data.X, tile_data.Y, 0);
				tile_go.transform.SetParent(tilesContainer.transform);
				tile_go.AddComponent<SpriteRenderer> ().sprite = emptySprite;

				// Add callback to update graphics when tile type changes.
				tile_data.RegisterTileTypeChangedCallBack ((Tile t) => { OnTileTypeChanged(t, tile_go); });
			}
		}
//		World.RandomizeTiles ();
	}

	void LoadSprites() {
		Sprite[] sprites = Resources.LoadAll<Sprite> ("Images/Furniture/Wall");

		foreach (Sprite s in sprites) {
			Debug.Log (s);
			furnitureSprites.Add (s.name, s);
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	// Changes the sprite of a tile accordingly
	void OnTileTypeChanged(Tile tile, GameObject tile_go) {
		SpriteRenderer tile_sr = tile_go.GetComponent<SpriteRenderer> ();
		if (tile.Type == Tile.TileType.Floor) {
			tile_sr.sprite = floorSprite;
		} else if (tile.Type == Tile.TileType.Empty) {
			tile_sr.sprite = null;
		} else {
			Debug.LogError ("Tiletype should be 'Floor' or 'Empty' -- Did you forget to assign a type?");
		}
	}

	public void OnFurnitureCreated(Furniture furn) {
		// Create a VISUAL GameObject linked to the data.
		GameObject obj_go = new GameObject ();
		furnitureGameObjectMap.Add (furn, obj_go);

		//FIXME does not consider multitile objects.

		// Create new gameobject for a tile
		obj_go.name = furn.objectType + furn.tile.X + "_" + furn.tile.Y;
		obj_go.transform.position = new Vector3 (furn.tile.X, furn.tile.Y, 0);
		obj_go.transform.SetParent(this.transform, true);

		// Update visuals
		SpriteRenderer obj_sr = obj_go.AddComponent<SpriteRenderer> ();
		obj_sr.sprite = GetSpriteForFurniture (furn);
		// Check neighbours to see what sprite to load.

		obj_sr.sortingLayerName  = ("TileUI");

		furn.RegisterOnChangedCallback (	OnFurnitureChanged);
	}


	public void OnFurnitureChanged(Furniture furn) {
		// Make sure the graphics are correct.
		if (furnitureGameObjectMap.ContainsKey (furn) == false) {
			Debug.LogError ("Trying to change visuals for furniture not in our map.");
		}
		GameObject furn_go = furnitureGameObjectMap [furn];
		furn_go.GetComponent<SpriteRenderer>().sprite = GetSpriteForFurniture (furn);
	}

	Sprite GetSpriteForFurniture(Furniture obj) {
		if (obj.linksToNeighbour == false) {
			return furnitureSprites [obj.objectType];
		}

		string spriteName = obj.objectType + "_";

		// Check for neighbours NESW
		Tile t;

		int x = obj.tile.X;
		int y = obj.tile.Y;

		t = World.GetTileAt (x, y + 1);
		if (t != null && t.furniture != null &&  t.furniture.objectType == obj.objectType) {
			spriteName += "N";
		}
		t = World.GetTileAt (x+1, y);
		if (t != null && t.furniture != null &&  t.furniture.objectType == obj.objectType) {
			spriteName += "E";
		}
		t = World.GetTileAt (x, y-1);
		if (t != null && t.furniture != null &&  t.furniture.objectType == obj.objectType) {
			spriteName += "S";
		}
		t = World.GetTileAt (x-1, y);
		if (t != null && t.furniture != null &&  t.furniture.objectType == obj.objectType) {
			spriteName += "W";
		}

		// Otherwise the sprite name is more complicated.
		if (furnitureSprites.ContainsKey (spriteName) == false) {
			// Our sprite can not be found.
			Debug.LogError ("GetSpriteForFurniture -- No sprite with name: " + spriteName);
			return null;
		}
		return furnitureSprites [spriteName];
	}
}
