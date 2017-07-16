using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseController : MonoBehaviour {

	public GameObject circleCursor;

	bool buildModeObjects = false;
	string buildModeObjectType;
	Tile.TileType buildModeTile = Tile.TileType.Floor;
	Vector3 lastFramePosition;
	Vector3 currFramePostion;
	Vector3 dragStartPosition;
	List<GameObject> dragCircleCursorList;
	bool isDragging;

	public float scrollSensitivity = 1f;
	// Use this for initialization
	void Start () {
		dragCircleCursorList = new List<GameObject> ();
//		ObjectPooler.Preload (circleCursor, 900);
	}
	
	// Update is called once per frame
	void LateUpdate () {
		
		currFramePostion = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		currFramePostion.z = 0; // Set to 0 to make sure the circle does not get too close 
								// to the camera to be rendered.
		updateDrag ();
		updateCamera ();

		// Save mouse position from this frame.
		// Dont use currFramePosition because we may have moved the camera.
		lastFramePosition = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		lastFramePosition.z = 0;
	}

	void updateCursor() {
		// Update cursorcircle position -- Snap to tile.
		Tile tileUnderMouse = this.GetTileAtWorldCoord(currFramePostion);
		if (tileUnderMouse != null) {
			circleCursor.SetActive (true);
			Vector3 cursorPosition = new Vector3 (tileUnderMouse.X, tileUnderMouse.Y, 0);
			circleCursor.transform.position = cursorPosition;
		} else {
			circleCursor.SetActive (false);
		}
	}

	void updateDrag() {
		// Clear old selection
		foreach (GameObject cc in dragCircleCursorList) {
			ObjectPooler.Despawn (cc);
		};
		dragCircleCursorList.Clear ();

		// Start drag
		if (Input.GetMouseButtonDown(0)) {
			dragStartPosition = currFramePostion; 
		}

		// Set start and end points for x and y. Used during and at end of drag.
		int start_x = Mathf.FloorToInt (dragStartPosition.x);
		int end_x = Mathf.FloorToInt (currFramePostion.x);
		int start_y = Mathf.FloorToInt (dragStartPosition.y);
		int end_y = Mathf.FloorToInt (currFramePostion.y);
		// If we are dragging in the "wrong" direction, invert the start and end points.
		if (end_x < start_x) {
			int temp_x = end_x;
			end_x = start_x;
			start_x = temp_x;
		}
		if (end_y < start_y) {
			int temp_y = end_y;
			end_y = start_y;
			start_y = temp_y;
		}

		// While dragging
		if (Input.GetMouseButton(0)) {
			for (int x = start_x; x <= end_x; x++) {
				for (int y = start_y; y <= end_y; y++) {
					Tile t = WorldController.Instance.World.GetTileAt (x, y);
					if (t != null) {
						GameObject go = ObjectPooler.Spawn (circleCursor, new Vector3 (x, y, 0), Quaternion.identity);
						go.transform.SetParent (this.transform, true);
						dragCircleCursorList.Add (go);
					}
				}
			}
		}

		// End drag
		if (Input.GetMouseButtonUp (0)) {
			for (int x = start_x; x <= end_x; x++) {
				for (int y = start_y; y <= end_y; y++) {

					Tile t = WorldController.Instance.World.GetTileAt (x, y);
					if (t != null) {
						if (buildModeObjects) {
							// Create installed object and assign to tile

							//FIXME: right now we are just doing walls;
							WorldController.Instance.World.PlaceFurniture (buildModeObjectType, t);

						} else {
							// We are in tile changing mode;
							t.Type = buildModeTile;
						}
					}
				}
			}
		}
	}

	void updateCamera() {
		// Zooming in and out;
		Camera.main.orthographicSize -= Camera.main.orthographicSize * Input.GetAxis("Mouse ScrollWheel") * scrollSensitivity;
		Camera.main.orthographicSize = Mathf.Clamp (Camera.main.orthographicSize, 3f, 25f);

		// Right or middle mouse button to pan
		if (Input.GetMouseButton(2) || Input.GetMouseButton(1)) {
			Camera.main.transform.Translate(lastFramePosition - currFramePostion);
		}


	}

	Tile GetTileAtWorldCoord(Vector3 pos) {
		int x = Mathf.FloorToInt (pos.x);
		int y = Mathf.FloorToInt (pos.y);

		// Get world instance
		return GameObject.FindObjectOfType<WorldController>().World.GetTileAt(x,y);
	}


	public void SetMode_BuildFloor() {
		buildModeObjects = false;
		buildModeTile = Tile.TileType.Floor;
	}

	public void SetMode_EraseFloor() {
		buildModeObjects = false;
		buildModeTile = Tile.TileType.Empty;
	}

	public void SetMode_BuildFurniture(string objType) {
		buildModeObjects = true;
		buildModeObjectType = objType;
	}
}
