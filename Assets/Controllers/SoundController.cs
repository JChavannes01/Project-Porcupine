using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour {
	AudioClip tileAC;
	AudioClip furnAC;

	float soundCD = 0.1f;

	// Use this for initialization
	void Start () {
		WorldController.Instance.world.RegisterOnFurnitureCreatedCallBack (OnFurnitureCreated);
		WorldController.Instance.world.RegisterOnTileChangedCallBack (OnTileChanged);

		// Load soundclips
		tileAC = Resources.Load < AudioClip> ("Sounds/Floor_OnCreated");
		furnAC = Resources.Load < AudioClip> ("Sounds/Wall_OnCreated");

	}
	
	// Update is called once per frame
	void Update () {
		soundCD -= Time.deltaTime;
	}

	// Changes the sprite of a tile accordingly
	void OnTileChanged(Tile tile) {
		//FIXME
		if (soundCD > 0) {
			return;
		}
		AudioSource.PlayClipAtPoint (tileAC, Camera.main.transform.position);
		soundCD = 0.1f;
	}

	void OnFurnitureCreated(Furniture furn) {
		if (soundCD > 0) {
			return;
		}
		AudioSource.PlayClipAtPoint (furnAC, Camera.main.transform.position);
		soundCD = 0.1f;
	}
		
}
