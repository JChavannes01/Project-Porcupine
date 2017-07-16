using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour {


	// Update is called once per frame
	void LateUpdate () {
		Vector3 pos = Camera.main.transform.position;
		pos.z = 0;
		transform.position = pos;
//		transform.localScale = new Vector3 (Camera.main.orthographicSize/2, Camera.main.orthographicSize/2, 1);
	}
}
