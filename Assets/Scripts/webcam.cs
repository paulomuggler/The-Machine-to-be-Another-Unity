﻿using UnityEngine;
using System.Collections;

public class webcam : MonoBehaviour {

	public MeshRenderer UseWebcamTexture;
	public Camera POVCamera;
	public float range = 20;
	public float zoom = 39.5f;
	public float widthHeightRatio = 1.3f;
	public int cameraID = 1;
	public float width, height; 
	public Quaternion otherPose;
	public Vector3 otherPosition;

	private float tiltAngle = 0;
	private WebCamTexture camTex;
	private float dimLevel = 1;
	private	bool dimmed = false;
	private bool headtrackingOn = true;
	private float dimRate = 0.08f;
	private bool twoWaySwap = true;

	void Start () {
		width = 1920;
		height = 1080;
		getPlayerPrefs();
		startCamera(PlayerPrefs.GetInt("cameraID"));
		otherPose = new Quaternion ();
		otherPosition = new Vector3 ();
	}

	void getPlayerPrefs() {
		tiltAngle = PlayerPrefs.GetFloat("tiltAngle");
		zoom = PlayerPrefs.GetFloat ("zoom");
        if (zoom == 0)
        {
            zoom = 39.5f;
            PlayerPrefs.SetFloat("zoom", 39.5f);
        }
		cameraID = PlayerPrefs.GetInt ("cameraID");
	}

	public void setDimmed() {		
		dimmed = !dimmed;
	}

	public void setDimmed(bool dim) {		
		dimmed = dim;
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
		PlayerPrefs.SetFloat ("tiltAngle", tiltAngle);
	}				
		
	public void setZoom(float value) {
		zoom = value;
		PlayerPrefs.SetFloat ("zoom", zoom);
	}		

	public void setCameraID(int id) {
		camTex.Pause ();
		startCamera (id);
		PlayerPrefs.SetInt ("cameraID", id);
	}		

	public void startCamera (int id){
		WebCamDevice[] devices = WebCamTexture.devices;
		if (devices.Length == 1)
			id = 0;
 		if (id < devices.Length) {
			camTex = new WebCamTexture (devices [id].name, (int)width, (int)height, 60);
			camTex.requestedWidth = (int)width;
			camTex.requestedHeight = (int)height;
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
		if (!twoWaySwap) {
			transform.position = POVCamera.transform.position + POVCamera.transform.forward * 15; //keep webcam at a certain distance from head.
			transform.rotation = POVCamera.transform.rotation; //keep webcam feed aligned with head
			transform.rotation *= Quaternion.Euler (0, 0, 1) * Quaternion.AngleAxis (-utilities.toEulerAngles (POVCamera.transform.rotation).x, Vector3.forward); //compensate for absence of roll servo
			transform.rotation *= Quaternion.Euler (0, 0, tiltAngle) * Quaternion.AngleAxis (camTex.videoRotationAngle, Vector3.up); //to adjust for webcam physical orientation
			//transform.localScale = new Vector3 (width/height*zoom, height/width*zoom, 0);
			transform.localScale = new Vector3 (widthHeightRatio * zoom, 1 / widthHeightRatio * zoom, 0);
		} else {
			transform.position = otherPosition + otherPose * Vector3.forward * 15; //keep webcam at a certain distance from head.
			transform.rotation = otherPose; //keep webcam feed aligned with head
			//transform.rotation *= Quaternion.Euler (0, 0, 1) * Quaternion.AngleAxis (-utilities.toEulerAngles (POVCamera.transform.rotation).x, Vector3.forward); //compensate for absence of roll servo
			transform.rotation *= Quaternion.Euler (0, 0, tiltAngle) * Quaternion.AngleAxis (camTex.videoRotationAngle, Vector3.up); //to adjust for webcam physical orientation
			//transform.localScale = new Vector3 (width/height*zoom, height/width*zoom, 0);
			transform.localScale = new Vector3 (widthHeightRatio * zoom, 1 / widthHeightRatio * zoom, 0);
		}
		setDimLevel ();
	}
}
