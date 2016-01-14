﻿using UnityEngine;
using System.Collections;

public class webcam : MonoBehaviour {

	public MeshRenderer UseWebcamTexture;
	public Camera POVCamera;
	private WebCamTexture camTex;
	private float dimLevel = 1;
	private	bool dimmed = false;
	private bool headtrackingOn = true;
	private float dimRate = 0.08f;
	public float range = 20;
	public float zoom = 18;
	private int cameraID = 0;
	private float tiltAngle = 0;
	public float width, height; 
	// Use this for initialization
	void Start () {
		//Debug.Log("Device:" + devices[i].name + " | IS FRONT FACING:" + devices[i].isFrontFacing);
		setCameraID (cameraID);
		width = 1920;
		height = 1080;

	}

	public void setDimmed() {		
		dimmed = !dimmed;
	}

	public bool isHeadtrackingOn() {
		return headtrackingOn;
	}

	private void setDimLevel() {		
		float next;
		if (dimmed) next = 1;
		else next = 0;
		dimLevel += dimRate * (next - dimLevel);	
		Color c = new Color (dimLevel*range, dimLevel*range, dimLevel*range);
		UseWebcamTexture.material.SetColor("_Color", c);
	}

	public void setCameraOrientation(){
		tiltAngle += 90;
	}				
		
	public void setZoom(float value) {
		zoom = value;
	}		

	public void setCameraID(int id) {
		WebCamDevice[] devices = WebCamTexture.devices;
		//cameraID = id;
		if (devices.Length >= id - 1) {
			camTex = new WebCamTexture (devices [id].name, (int) width, (int) height, 60);
			camTex.requestedWidth = (int) width;
			camTex.requestedHeight = (int) height;
			UseWebcamTexture.material.mainTexture = camTex;
			UseWebcamTexture.material.shader = Shader.Find ("Sprites/Default");
			camTex.Play ();
		}
	}		

	public void recenterPose(){
		UnityEngine.VR.InputTracking.Recenter();
	}

	public void switchHeadtracking() {
		headtrackingOn = !headtrackingOn;
	}

	// Update is called once per frame
	void Update () {		
		transform.position = POVCamera.transform.position + POVCamera.transform.forward * 15; //keep webcam at a certain distance from head.
		transform.rotation = POVCamera.transform.rotation; //keep webcam feed aligned with head
		transform.rotation *= Quaternion.Euler (0, 0, tiltAngle) * Quaternion.AngleAxis(camTex.videoRotationAngle, Vector3.up); //to adjust for webcam physical orientation
		transform.localScale = new Vector3 (width/height*zoom, height/width*zoom, 0);
		setDimLevel ();
	}
}
