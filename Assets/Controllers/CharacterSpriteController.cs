using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSpriteController : MonoBehaviour {

	Dictionary<Character, GameObject> characterGameObjectMap;
	Dictionary<string, Sprite> characterSprites;

	World world {
		get { return WorldController.Instance.world; }
	}


	// Use this for initialization
	void Start () {
		// Fill our spite dictionary
		LoadSprites ();

		characterGameObjectMap = new Dictionary<Character, GameObject>();


		// Add callback to update graphics when tile type changes.
		world.RegisterOnCharacterCreatedCallBack (OnCharacterCreated);

		//FIXME DEBUGGING, create character somewhere else.
		Character c = world.createCharacter (world.GetTileAt (world.Width / 2, world.Height / 2));
	}
	
	void LoadSprites() {
		characterSprites = new Dictionary<string, Sprite> ();
		Sprite[] sprites = Resources.LoadAll<Sprite> ("Images/Characters/");

		foreach (Sprite s in sprites) {
			Debug.Log (s);
			characterSprites.Add (s.name, s);
		}
	}

	public void OnCharacterCreated(Character character) {
		// Create a VISUAL GameObject linked to the data.
		GameObject char_go = new GameObject ();
		characterGameObjectMap.Add (character, char_go);

		// Create new gameobject for a character
		char_go.name = "Character";
		char_go.transform.position = new Vector3 (character.X, character.Y, 0);
		char_go.transform.SetParent(this.transform, true);

		// Update visuals
		SpriteRenderer char_sr = char_go.AddComponent<SpriteRenderer> ();
		char_sr.sprite = characterSprites["p1_front"];
		char_sr.sortingLayerName  = "Characters";

		character.RegisterOnChangedCallback (OnCharacterChanged);
	}


	public void OnCharacterChanged(Character character) {
		// Make sure the graphics are correct.
		if (characterGameObjectMap.ContainsKey (character) == false) {
			Debug.LogError ("Trying to change visuals for character not in our map.");
			return;
		}
		GameObject char_go = characterGameObjectMap [character];
		char_go.transform.position = new Vector3 (character.X, character.Y, 0);
	}
}
