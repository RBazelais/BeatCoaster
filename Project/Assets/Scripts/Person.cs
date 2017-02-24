using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Person : MonoBehaviour {
	[SerializeField] private SplineTrailRenderer trail;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (trail) 
		{
			UpdatePosition();
		}
	}

	private void UpdatePosition() 
	{
		Vector3 pos = trail.transform.position;

	}
}
