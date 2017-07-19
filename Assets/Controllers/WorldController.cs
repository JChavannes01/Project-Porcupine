using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldController : MonoBehaviour {
	public static WorldController Instance { get; protected set;}

	public World world { get; protected set; }

	// Use this for initialization
	void Awake () {
		if (Instance != null) {
			Debug.LogError ("There should only ever be one instance of WorldController.");
		}
		Instance = this;

		// Create a new world with empty tiles
		world = new World ();

		// Center the camera on the world.
		Camera.main.transform.position =  new Vector3 (world.Width / 2, world.Height / 2, Camera.main.transform.position.z);
	}

	void Update() {
		//TODO add pause/unpause, speed control
		world.Update (Time.deltaTime);
	}

	public Tile GetTileAtWorldCoord(Vector3 pos) {
		int x = Mathf.FloorToInt (pos.x);
		int y = Mathf.FloorToInt (pos.y);

		// Get world instance
		return GameObject.FindObjectOfType<WorldController>().world.GetTileAt(x,y);
	}
}