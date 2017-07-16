using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// Things attached to a tile, e.g. walls, doors or installed furniture
public class Furniture {

	// BASE Tile where the object is placed on. Large objects may occupy multiple tiles.
	public Tile tile{ get; protected set;} 

	// This type will be queried by the visual system to know which sprite to render..
	public string objectType { get; protected set;}

	// This is a multiplier, a value of 2 means you move at half speed.
	// NOTE: If 0, the tile is impassable (e.g. a wall).
	float groundResistance; 

	int width;
	int height;
	public bool linksToNeighbour { get; protected set; }

	Action<Furniture> cbOnChanged;
	Func<Tile, bool> funcPositionValidation;


	protected Furniture() {
		
	}

	// Used by our object factory to create a prototype.
	// Note that it doesnt ask for a tile.
	public static Furniture CreatePrototype(string objectType, float groundResistance=1f, int width=1, int height=1, bool linksToNeighbour=false) {
		Furniture obj = new Furniture ();
		obj.objectType = objectType;
		obj.groundResistance = groundResistance;
		obj.width = width;
		obj.height = height;
		obj.linksToNeighbour = linksToNeighbour;

		// Default the positionvalidation funciton to check whether the tile has floors.
		obj.funcPositionValidation = obj.isValidPosition;

		return obj;
	}

	public static Furniture PlaceInstance(Furniture prototype, Tile tile, bool linksToNeighbour=false) {
		if (prototype.funcPositionValidation(tile) == false) {
			Debug.LogError ("PlaceInstance -- Position validity function returned FALSE.");
			return null;
		}

		// we know our placement destination is valid.

		Furniture obj = new Furniture ();
		obj.objectType = prototype.objectType;
		obj.groundResistance = prototype.groundResistance;
		obj.width = prototype.width;
		obj.height = prototype.height;
		obj.tile = tile;
		obj.linksToNeighbour = prototype.linksToNeighbour;

		//FIXME: This assumes we are 1x1
		if (tile.PlaceFurniture (obj) == false) {
			// For some reason we weren't able to place our object in this tile.
			// Probably it was already occupied.

			// Do NOT return our newly instantiated object.
			return null;
		}

		if (obj.linksToNeighbour) {
			// This furniture links to neighbours, so we need to inform our neighbours
			// That they have a new buddy, and need to update their graphics.
			// Just trigger their OnChangedCallback.

			Tile t;
			int x = tile.X;
			int y = tile.Y;

			t = tile.world.GetTileAt (x, y + 1);
			if (t != null && t.furniture != null &&  t.furniture.objectType == obj.objectType) {
				// We have a nneighbour with the same object type as us so tell it it has to be changed by calling its callback.
				t.furniture.cbOnChanged (t.furniture);
			}
			t = tile.world.GetTileAt (x+1, y);
			if (t != null && t.furniture != null &&  t.furniture.objectType == obj.objectType) {
				t.furniture.cbOnChanged (t.furniture);
			}
			t = tile.world.GetTileAt (x, y-1);
			if (t != null && t.furniture != null &&  t.furniture.objectType == obj.objectType) {
				t.furniture.cbOnChanged (t.furniture);
			}
			t = tile.world.GetTileAt (x-1, y);
			if (t != null && t.furniture != null &&  t.furniture.objectType == obj.objectType) {
				t.furniture.cbOnChanged (t.furniture);
			}


		}

		return obj;
	}

	public void RegisterOnChangedCallback(Action<Furniture> callbackFunc) {
		this.cbOnChanged += callbackFunc;
	}

	public void UnregisterOnChangedCallback(Action<Furniture> callbackFunc) {
		this.cbOnChanged -= callbackFunc;
	}

	public bool isValidPosition(Tile t) {
		// Make sure the tile we're trying to build on is FLOOR
		if (t.Type != Tile.TileType.Floor) {
			return false;
		}

		// Make sure tile doesn't already have furniture.
		if (t.furniture != null) {
			return false;
		}


		return true;
	}

}
