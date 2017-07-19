using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuildModeController : MonoBehaviour {

	bool buildModeObjects = false;
	string buildModeObjectType;
	Tile.TileType buildModeTile = Tile.TileType.Floor;

	void Start () {
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

	public void DoBuild(Tile t) {
		if (buildModeObjects) {
			// Create installed object and assign to tile

			//FIXME: This instantly builds the furniture.
//										WorldController.Instance.World.PlaceFurniture (buildModeObjectType, t);

			// Can we build on the selected tile?
			string furnitureType = buildModeObjectType;

			if (WorldController.Instance.world.IsFurniturePlacementValid (furnitureType, t) &&
				t.pendingFurnitureJob == null) {
				//This tile isnt valid for this furniture, we will continue building

				Job j = new Job (t, furnitureType, (theJob) => {
					WorldController.Instance.world.PlaceFurniture (furnitureType, theJob.tile);
					t.pendingFurnitureJob = null;
				});

				// FIXME: We dont want manual flags to set to prevent conflicts
				t.pendingFurnitureJob = j;

				j.RegisterJobCancelCallback ((theJob) => {
					theJob.tile.pendingFurnitureJob = null;
				});

				WorldController.Instance.world.jobQueue.Enqueue (j);
			}

		} else {
			// We are in tile changing mode;
			t.Type = buildModeTile;
		}
	}
}
