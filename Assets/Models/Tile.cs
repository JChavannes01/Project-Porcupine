using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Tile {

	public enum TileType {Empty, Floor};

	Action<Tile> cbTileTypeChanged;

	// Every tile can have 1 installed piece of furniture and 1 inventory item.
	public Furniture furniture { get; protected set;}
	Inventory inventory;

	TileType _type = TileType.Empty;
	public TileType Type {
		get { return _type; }
		set {
			TileType oldType = _type;
			_type = value;

			// Call the callback
			if (cbTileTypeChanged != null && oldType != _type) {
				cbTileTypeChanged(this);
			}
		}
	}

	public Job pendingFurnitureJob;

 // Default to empty floor

	// Self awareness.
	public World world { get; protected set;}
	int x;
	int y;
	public int X { get { return x; }}
	public int Y { get { return y; }}


	public Tile(World world, int x, int y) {
		this.world = world;
		this.x = x;
		this.y = y;
	}

	public void RegisterTileTypeChangedCallBack(Action<Tile> callback) {
		cbTileTypeChanged += callback;
	}

	public void UnregisterTileTypeChangedCallBack(Action<Tile> callback) {
		cbTileTypeChanged -= callback;
	}


	public bool PlaceFurniture(Furniture instance) {
		if (instance == null) {
			// Uninstall the object
			furniture = null;
			return true;
		}

		if (Type == TileType.Empty) {
			// Can't place furniture in space ..
			Debug.LogError ("Furniture can only be built on floors, not in empty space...");
			return false;
		}

		if (furniture != null) {
			Debug.LogError ("Tile already has a piece of furniture!");
			return false;
		}

		// Everything is fine
		furniture = instance;
		return true;
	}

	// Tells us if the given tile is adjacent
	public bool IsNeighbour(Tile tile, bool checkDiag) {
		if (tile.x == x && Mathf.Abs (tile.y - y) == 1) {
			return true;
		}

		if (tile.y == y && Mathf.Abs (tile.x - x) == 1) {
			return true;
		}

		if (checkDiag) {
			// True if both x and y differ by 1
			if ( Mathf.Abs (tile.x - x) == 1 && Mathf.Abs (tile.y - y) == 1) {
				return true;
			}

		}

		return false;
		
	}
}
