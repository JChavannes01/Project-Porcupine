using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileSpriteController : MonoBehaviour {


	public Sprite floorSprite;
	public Sprite emptySprite;

	Dictionary<Tile, GameObject> tileGameObjectMap;


	World world {
		get { return WorldController.Instance.world; }
	}

	// Use this for initialization
	void Start () {
		tileGameObjectMap = new Dictionary<Tile, GameObject>();

		for (int x = 0; x < world.Width; x++) {
			for (int y = 0; y < world.Height; y++) {
				// Data object of the tile.
				Tile tile_data = world.GetTileAt (x, y);

				// Create new gameobject for a tile
				GameObject tile_go = new GameObject();
				tile_go.name = "Tile_" + x + "_" + y;
				tile_go.transform.position = new Vector3 (tile_data.X, tile_data.Y, 0);
				tile_go.transform.SetParent(this.transform, true);
				SpriteRenderer tile_sr = tile_go.AddComponent<SpriteRenderer> ();
				tile_sr.sortingLayerName = "Tiles";
				tile_sr.sprite = emptySprite;

				tileGameObjectMap.Add (tile_data, tile_go);
			}
		}

		// Add callback to update graphics when tile type changes.
		world.RegisterOnTileChangedCallBack (OnTileChanged);
	}

	// Changes the sprite of a tile accordingly
	void OnTileChanged(Tile tile) {
		if (tileGameObjectMap.ContainsKey (tile) == false) {
			Debug.LogError ("tileGameObjectMap doesnt contain the tile_data for the given tile.");
			return;
		}

		GameObject tile_go = tileGameObjectMap [tile];

		SpriteRenderer tile_sr = tile_go.GetComponent<SpriteRenderer> ();
		if (tile.Type == Tile.TileType.Floor) {
			tile_sr.sprite = floorSprite;
		} else if (tile.Type == Tile.TileType.Empty) {
			tile_sr.sprite = emptySprite;
		} else {
			Debug.LogError ("Tiletype should be 'Floor' or 'Empty' -- Did you forget to assign a type?");
		}
	}
}
