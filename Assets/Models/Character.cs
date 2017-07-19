using System.Collections;
using UnityEngine;
using System;

public class Character {
	public float X {
		get {
			return Mathf.Lerp (currTile.X, destTile.X, movementPercentage);
		}
	}

	public float Y {
		get {
			return Mathf.Lerp (currTile.Y, destTile.Y, movementPercentage);
		}
	}
	public Tile currTile { get; protected set;}
	
	Tile destTile; // If we aren't moving then destTile = currTile
	float movementPercentage; // Goes from 0 to 1, as the character moves from currTile to destTile
	float moveSpeed = 2f; // speed in tiles/sec

	Action<Character> cbCharacterChanged;

	Job myJob;

	public Character(Tile tile) {
		currTile = destTile = tile;
	}

	// Used to update the position of the character realtime.
	// Call this from a controller
	public void Update(float deltaTime) {
		
		// Do I have a job??
		if (myJob == null) {
			myJob = currTile.world.jobQueue.Dequeue ();

			if (myJob != null) {
				destTile = myJob.tile;

				myJob.RegisterJobCompleteCallback (OnJobEnded);
				myJob.RegisterJobCancelCallback (OnJobEnded);
			}
		}

		// Are we already at our destination??
		if (currTile == destTile) {
			if (myJob != null) {
				myJob.DoWork (deltaTime);
			}
			return;
		}


		//Total distance from a to b?
		float distToTravel = Mathf.Sqrt (Mathf.Pow(currTile.X-destTile.X, 2) + Mathf.Pow(currTile.Y-destTile.Y, 2));

		// How much distance can we travel in this frame?
		float distThisFrame = moveSpeed * deltaTime;

		// How much relative distance did we cover?
		float percThisFrame = distThisFrame / distToTravel;

		// Add that to overall travel distance.
		movementPercentage += percThisFrame;
		if (movementPercentage >= 1) {
			// Reached our destination
			currTile = destTile;
			movementPercentage = 0;

			//FIXME retain overshot movement?
		}

		if (cbCharacterChanged != null) {
			cbCharacterChanged (this);
		}
	}


	public void SetDestination(Tile tile) {
		if (currTile.IsNeighbour (tile, true) == false) {
			Debug.Log ("Character::SetDestination -- destination tile is not a neighbour to currTile.");
		}
		destTile = tile;
	}

	public void RegisterOnChangedCallback(Action<Character> cb) {
		cbCharacterChanged += cb;
	}

	public void UnregisterOnChangedCallback(Action<Character> cb) {
		cbCharacterChanged -= cb;
	}

	void OnJobEnded(Job j) {
		// Job completed or was cancelled

		if (j != myJob) {
			Debug.LogError ("Character being told about job that isnt his. You forgot to unregister something.");
			return;
		}
		myJob = null;
	}

}
