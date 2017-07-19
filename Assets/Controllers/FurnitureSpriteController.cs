using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FurnitureSpriteController : MonoBehaviour {


	Dictionary<Furniture, GameObject> furnitureGameObjectMap;

	Dictionary<string, Sprite> furnitureSprites;

	World world {
		get { return WorldController.Instance.world; }
	}

	// Use this for initialization
	void Start () {
		// Fill our spite dictionary
		LoadSprites ();

		furnitureGameObjectMap = new Dictionary<Furniture, GameObject>();


		// Add callback to update graphics when tile type changes.
		world.RegisterOnFurnitureCreatedCallBack (OnFurnitureCreated);
	}

	void LoadSprites() {
		furnitureSprites = new Dictionary<string, Sprite> ();
		Sprite[] sprites = Resources.LoadAll<Sprite> ("Images/Furniture/Wall");

		foreach (Sprite s in sprites) {
			Debug.Log (s);
			furnitureSprites.Add (s.name, s);
		}
	}

	public void OnFurnitureCreated(Furniture furn) {
		// Create a VISUAL GameObject linked to the data.
		GameObject furn_go = new GameObject ();
		furnitureGameObjectMap.Add (furn, furn_go);

		//FIXME does not consider multitile objects.

		// Create new gameobject for a tile
		furn_go.name = furn.objectType + furn.tile.X + "_" + furn.tile.Y;
		furn_go.transform.position = new Vector3 (furn.tile.X, furn.tile.Y, 0);
		furn_go.transform.SetParent(this.transform, true);

		// Update visuals
		SpriteRenderer furn_sr = furn_go.AddComponent<SpriteRenderer> ();
		furn_sr.sprite = GetSpriteForFurniture (furn);
		furn_sr.sortingLayerName  = ("Furniture");


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

	public Sprite GetSpriteForFurniture(Furniture obj) {
		if (obj.linksToNeighbour == false) {
			return furnitureSprites [obj.objectType];
		}

		string spriteName = obj.objectType + "_";

		// Check for neighbours NESW
		Tile t;

		int x = obj.tile.X;
		int y = obj.tile.Y;

		t = world.GetTileAt (x, y + 1);
		if (t != null && t.furniture != null &&  t.furniture.objectType == obj.objectType) {
			spriteName += "N";
		}
		t = world.GetTileAt (x+1, y);
		if (t != null && t.furniture != null &&  t.furniture.objectType == obj.objectType) {
			spriteName += "E";
		}
		t = world.GetTileAt (x, y-1);
		if (t != null && t.furniture != null &&  t.furniture.objectType == obj.objectType) {
			spriteName += "S";
		}
		t = world.GetTileAt (x-1, y);
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

	public Sprite GetSpriteForFurniture(string objectType) {
		if (furnitureSprites.ContainsKey (objectType)) {
			return furnitureSprites [objectType];
		}
		if (furnitureSprites.ContainsKey (objectType+"_")) {
			return furnitureSprites [objectType+"_"];
		}
		Debug.LogError ("GetSpriteForFurniture -- No sprite with name: " + objectType);

		return null;
	}
}
