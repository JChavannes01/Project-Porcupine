using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Object pooler is the static class that manages object pools for different prefabs.
/// </summary>
public static class ObjectPooler {
	const int DEFAULT_POOL_SIZE = 3;
	static Dictionary<GameObject, Pool> pools; // Keep track of different pools for every prefab.

	/// <summary>
	/// The Pool class holds a pool of objects for a single prefab.
	/// </summary>
	public class Pool {
		public GameObject prefab; // The game object that will be pooled.
		Stack<GameObject> inactive; // Track all inactive GameObjects of this pool

		int objectID; // Purely cosmetic. The ID will be appended to a GameObject to differentiate between the various objects in the pool.

		public Pool(GameObject prefab, int size) {
			this.prefab = prefab;
			inactive = new Stack<GameObject>();
		}

		// Spawns a new GameObject from the pool. First check all inactive GameObjects to see if they are still alive.
		// If none of them are alive, then create a new copy and store it in the pool.
		public GameObject Spawn(Vector3 pos, Quaternion rot) {
			GameObject instance;
			if (inactive.Count == 0) {
				instance = GameObject.Instantiate (prefab, pos, rot);
				instance.name = prefab.name + "_" + objectID;
				objectID++;

				// Add Poolmember component so we know which pool this object belongs to.
				instance.AddComponent<PoolMember>().myPool = this;
			} else {
				instance = inactive.Pop ();

				if (instance == null) {
					return Spawn (pos, rot);
				}
			} 

			instance.transform.position = pos;
			instance.transform.rotation = rot;
			instance.SetActive (true);
			return instance;
		}

		// If we no longer need the item, just disable it and add it to the inactive stack.
		public void Despawn(GameObject obj) {
			obj.SetActive (false);
			inactive.Push (obj);
		}
	}

	// Simple component class to track which pool this gameobject belongs to.
	public class PoolMember : MonoBehaviour {
		public Pool myPool;
	}

	// Add a dictionary entry for the given prefab, (creating the dictionary if it's not yet instantiated).
	static public void Init(GameObject prefab, int qty = DEFAULT_POOL_SIZE) {
		if (pools == null) {
			pools = new Dictionary<GameObject, Pool>();
		} 
		if (prefab != null && !pools.ContainsKey(prefab)) {
			pools[prefab] = new Pool(prefab,qty);
		}
	}

	/// <summary>
	/// Spawns a copy of the given prefab. (Will instantiate a new one if required.)
	/// NOTE: Start() and Awake() will only get called when the object is initialised.
	/// Remember to set member variables to defaults.	
	/// </summary>
	static public GameObject Spawn(GameObject prefab, Vector3 pos, Quaternion rot) {
		// Get the right pool
		Init (prefab);

		return pools [prefab].Spawn (pos, rot);
	}

	// Despawn the given object back into its pool.
	static public void Despawn(GameObject instance) {
		// Get right pool
		PoolMember pm = instance.GetComponent<PoolMember> ();
		if (pm == null) {
			Debug.Log ("Object " + instance.name + " was not spawned from a pool. Destroying it instead.");
		}
		pm.myPool.Despawn (instance);
	}

	// Preloads a bunch of objects before they are needed.
	public static void Preload(GameObject prefab, int amount) {
		Init (prefab, amount);

		// Create a new obj and immediately disable it.
		GameObject[] obs = new GameObject[amount];
		for (int i = 0; i < amount; i++) {
			obs[i] = Spawn (prefab, Vector3.zero, Quaternion.identity);
		}

		// Now disable all of them
		foreach (GameObject o in obs) {
			Despawn (o);
		}
	}
}
