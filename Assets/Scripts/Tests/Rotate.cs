using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 e = transform.localRotation.eulerAngles + Vector3.forward * 3;
		transform.localRotation = Quaternion.Euler(e);
	}
}
