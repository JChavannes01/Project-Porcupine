using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseController : MonoBehaviour {

	public GameObject circleCursor;

	Vector3 lastFramePosition;
	Vector3 currFramePostion;

	Vector3 dragStartPosition;
	List<GameObject> dragCircleCursorList;

	public float scrollSensitivity = 1f;
	// Use this for initialization
	void Start () {
		dragCircleCursorList = new List<GameObject> ();
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

	void updateDrag() {
		// If we're over a UI element, then bail out from this.
		if (EventSystem.current.IsPointerOverGameObject ()) {
			return;
		}

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
					Tile t = WorldController.Instance.world.GetTileAt (x, y);
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

			BuildModeController bmc = GameObject.FindObjectOfType<BuildModeController> ();

			for (int x = start_x; x <= end_x; x++) {
				for (int y = start_y; y <= end_y; y++) {
					Tile t = WorldController.Instance.world.GetTileAt (x, y);
					if (t != null) {
						// Call BuildModeController::DoBuild() 
						bmc.DoBuild (t);
					}
				}
			}
		}
	}

	void OnFurnitureJobComplete( string buildModeObjectType, Tile t) {
		WorldController.Instance.world.PlaceFurniture (buildModeObjectType, t);

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
}
