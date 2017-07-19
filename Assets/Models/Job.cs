using System.Collections;
using System;
using UnityEngine;

public class Job {

	// This class holds information for a queued up job, which can include 
	// Things like placing furniture, moving stored inventory.

	public Tile tile { get; protected set;}
	float jobTime;

	//FIXME Hardcoded furn object 
	public string jobObjectType { get; protected set;}

	Action<Job> cbJobComplete;
	Action<Job> cbJobCancel;

	public Job (Tile tile, string jobObjectType, Action<Job> cbJobComplete, float jobTime = 1f) {
		this.tile = tile;
		this.jobObjectType = jobObjectType;
		this.cbJobComplete += cbJobComplete;
		this.jobTime = jobTime;
	}

	public void RegisterJobCompleteCallback (Action<Job> callback) {
		cbJobComplete += callback;
	}

	public void RegisterJobCancelCallback (Action<Job> callback) {
		cbJobCancel += callback;
	}

	public void UnregisterJobCompleteCallback (Action<Job> callback) {
		cbJobComplete -= callback;
	}

	public void UnregisterJobCancelCallback (Action<Job> callback) {
		cbJobCancel -= callback;
	}

	public void DoWork(float workTime) {
		jobTime -= workTime;
		if (jobTime <= 0) {
			if (cbJobComplete != null) {
				cbJobComplete(this);
			}
		}
	}

	public void CancelJob() {
		if (cbJobCancel != null) {
			cbJobCancel (this);
		}
	}





}
