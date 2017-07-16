using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AutomaticVerticalSize : MonoBehaviour {

	public float childSize = 35f;

	// Use this for initialization
	void Start () {
		adjustSize ();
	}
	
	public void adjustSize() {
		Vector2 size = this.GetComponent<RectTransform> ().sizeDelta;
		int childCount = this.transform.childCount;
		float spacing = GetComponent<VerticalLayoutGroup> ().spacing;
		float vertPad = GetComponent<VerticalLayoutGroup> ().padding.vertical;

		size.y = childCount * childSize + (childCount - 1) * spacing + vertPad;
		this.GetComponent<RectTransform> ().sizeDelta = size;
	}
}
