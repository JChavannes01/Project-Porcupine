using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class JobQueue {

	Queue<Job> jobQueue;

	Action<Job> cbJobCreated;

	public JobQueue() {
		jobQueue = new Queue<Job>();
	}

	public void Enqueue(Job j) {
		jobQueue.Enqueue (j);

		//TODO Callbacks
		cbJobCreated(j);
	}

	public Job Dequeue() {
		if (jobQueue.Count == 0) {
			return null;
		}
		return jobQueue.Dequeue ();
	}

	public void RegisterJobCreatedCallback(Action<Job> cb) {
		cbJobCreated += cb;
	}

	public void UnregisterJobCreatedCallback(Action<Job> cb) {
		cbJobCreated -= cb;
	}


}
