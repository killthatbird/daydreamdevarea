﻿using UnityEngine;
using System.Collections;
using Gvr.Internal;
using System.Collections.Generic;

public class playerinteraction : MonoBehaviour {

	public	IGvrGazePointer gazer;

	//start orientation
	private Quaternion startcontrollerOR;
	//player orientation
	private Quaternion controllerOR;
	//the touchpad
	private Vector2 touchpadxy;
	//acellerometer
	private Vector3 controlleraccel;
	//app button pressed
	public bool appbuttonpress = false;
	// touchpad touch
	public bool touchpaddown = false;

	//recticle
	public Transform rec;
	// rec material
	public Material currentrecmat;
	//raycast rec material
	public Material newrecmat;
	//raycast start material
	public Material startmat;

	//headmovement
	public Transform headobj;

	//mainraycast hit
	public RaycastHit rhit;
	//raycast distance
	public float raydistance = 100f;
	//distance from hit
	public float hitdistance=0;
	//hit value
	Vector3? point;
	//the pointer obj
	public GameObject raybox;
	//object collected
	public static GameObject lookedatobj;
	// player state system
	public static int playerstate =0;
	//hit transform
	private Transform hit;
	//list of gameobjects collected
	public List<GameObject> elementcolection = new List<GameObject>();
	//game objects collected count
	public int elementintcount = 0;
	//collect point
	public Transform collectpoint;


	//movement variables
	private	float direction =0;
	private float speed = 1f;

	void Awake()
	{
		startcontrollerOR = transform.rotation;
		currentrecmat = rec.GetComponent<MeshRenderer>().material;
		startmat = currentrecmat;
		startmat.color = currentrecmat.color;
	}

	void OnEnable()
	{
		gazer = GazeInputModule.gazePointer;
	}


	
	// Update is called once per frame
	void Update () {

		controllerOR = GvrController.Orientation;
		transform.rotation = controllerOR;


		appbuttonpress = GvrController.AppButtonDown;
		touchpaddown = GvrController.TouchDown;

		direction = touchpadxy.y * 10;
	
		//touchpad
		touchpadxy = GvrController.TouchPos;
		//movement
		controlleraccel = GvrController.Accel;

		//raycast 
		Debug.DrawRay(transform.position,transform.forward * raydistance,Color.red,.5f);

		if(Physics.Raycast(transform.position,transform.forward,out rhit,Mathf.Infinity,1 << LayerMask.NameToLayer("incell")))
		{
				hit = rhit.transform;
				raybox.SetActive(true);
				hitdistance = rhit.distance;
				lookedatobj = hit.gameObject;
				point = rhit.point;
				raybox.transform.position = (transform.position + point.Value) / 2f;
				raybox.transform.localScale = new Vector3 (raybox.transform.localScale.x, raybox.transform.localScale.y, Vector3.Distance(transform.position,point.Value));

			//testing with gaze input
			gazer.OnGazeStart(GetComponent<Camera>(),hit.gameObject,rhit.point,true);


		}
	 else 
		{
			raybox.SetActive (false);
			point = null;
		}
		//raycast

		if(appbuttonpress)
		{
			switch (hit.tag)
			{
			case "element":
				elementcollector(hit);
				break;
			
			case "corecenter":
				elementcenter(hit);
				break;
			
			
			}
		}

		if(touchpaddown)
		{

			switch(hit.tag)
			{
			case "element":
				break;
			case "corecenter":
				touchpadactioncenterobj();
				break;
			}

		}


	}

	public void touchpadactioncenterobj()
	{
		hit.GetComponent<centralaction>().centralstateaction();
	}


	public void elementcollector(Transform obj)
	{
	//	print("collected");
		elementcolection.Add(obj.gameObject);
	//	print(obj.GetComponent<elementaction>().acklook);
		obj.GetComponent<elementaction>().collected(collectpoint);
		elementintcount++;
	}
		

	public void elementcenter(Transform hit)
	{
		if(elementintcount>0)
		{
			elementcolection[elementintcount-1].GetComponent<elementaction>().letloose(hit,rhit.point);
			elementcolection.Remove(elementcolection[elementintcount-1]);
			elementintcount--;

		}
	}
}
