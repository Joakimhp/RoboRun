using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
Add this script to player object. Give camera "MainCamera" tag.
 */

public class CameraFollow : MonoBehaviour {

    public Transform initialCameraPosition;

	public float cameraLerp;

	private Vector3 offset;

	private void Awake() {
		offset = Camera.main.transform.position - transform.position;
        initialCameraPosition = GameObject.FindGameObjectWithTag("MainCamera").gameObject.transform;
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (Vector3.Distance(Camera.main.transform.position, transform.position + offset) > 0.01f){
			Vector3 newCamPos = Vector3.Lerp(Camera.main.transform.position, transform.position + offset, cameraLerp);
		Camera.main.transform.position = newCamPos;
		}
	}

	public void SnapCameraToPlayer(){
		Camera.main.transform.position = transform.position + offset;
	}
}
