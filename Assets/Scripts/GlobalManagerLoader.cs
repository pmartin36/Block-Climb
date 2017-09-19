using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalManagerLoader : MonoBehaviour {
	// Use this for initialization
	void Awake () {
		GlobalManager m = GlobalManager.Instance;
		Destroy(this.gameObject);
	}
}
