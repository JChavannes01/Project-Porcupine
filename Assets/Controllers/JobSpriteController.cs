using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JobSpriteController : MonoBehaviour {
	// This barebones controller is mostly jsut going to piggyback on furniturespritecontroller
	// because we dont yet fully know what our job system is going to look like in the future

	FurnitureSpriteController fsc;
	Dictionary<Job, GameObject> jobGameObjectMap;

	void Start() {
		jobGameObjectMap = new Dictionary<Job, GameObject> ();
		fsc = GameObject.FindObjectOfType<FurnitureSpriteController> ();

		//FIXME no such object yet.
		WorldController.Instance.world.jobQueue.RegisterJobCreatedCallback (OnJobCreated);	
	}

	void OnJobCreated(Job job) {
		//FIXME we can only do furniture building jobs

		//TODO sprite
		GameObject job_go = new GameObject ();

		jobGameObjectMap.Add (job, job_go);

		// Create new gameobject for a tile
		job_go.name = "JOB_"+ job.jobObjectType +"_"+  job.tile.X+ "_" + job.tile.Y;
		job_go.transform.position = new Vector3 (job.tile.X, job.tile.Y, 0);
		job_go.transform.SetParent(this.transform, true);

		// Update visuals
		SpriteRenderer job_sr = job_go.AddComponent<SpriteRenderer> ();
		job_sr.sortingLayerName = "Jobs";
		job_sr.sprite = fsc.GetSpriteForFurniture (job.jobObjectType);
		job_sr.color = new Color (0.5f, 1f, 0.5f, 0.3f);


		job.RegisterJobCompleteCallback (OnJobEnded);
		job.RegisterJobCancelCallback (OnJobEnded);
	}

	void OnJobEnded(Job job) {
		//FIXME we can only do furniture building jobs
		//TODO delete sprite
		GameObject job_go = jobGameObjectMap[job];
		job.UnregisterJobCancelCallback (OnJobEnded);
		job.UnregisterJobCompleteCallback (OnJobEnded);

		Destroy (job_go);
	}

}
