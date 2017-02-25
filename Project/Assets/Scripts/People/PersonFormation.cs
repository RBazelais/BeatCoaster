using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonFormation : MonoBehaviour {

	// Use this for initialization
	void Start () {
		//FansGetInFormation ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	/*
		override segmentDistanceRange with Alpha Key
		keep wobble
		// disable ability to add more crowd people
		persons will be positioned on the grid from [back, front] = [40, 0]
		arrange limited number of persons into new positions on grid onButtonPress (Alpha9)

	*/

	///	psuedo Code
	///
	/// public void FansGetInFormation(){
	/// 	if (Input.GetKeyDown (KeyCode.Alpha9)) {
	/// 		MakeAFace();
	/// 	}
	///
	/// 	void MakeAFace(){
	/// 		Get Position of all Person Clones or Get all Person Clones on screen
	/// 	then collect them into an array
	/// 		var ArrayName = New Arr [ with all the clones in it];
	///
	/// 	Put them in a new public IntRange from Range (10, 30)
	/// 	for ( Loop through Array ){
	/// 		var row;
	/// 		for ( draw in rows based on IntRange  )
	/// 			distribute within the grid with that range (no i did not maths yet)
	/// 			var MapGroupOrwWatever = [][];
	/// 				        0 0 0 0 0 0 0 0
	/// 					  0         	    0
	/// 				     0                   0
	///         		    0                     0
	/// 				   0     00        00      0
	/// 				  0      00        00       0
	///                   0                         0
	///                   0                         0
	///                    0     0           0      0
	///                     0     0         0      0
	///                      0      0  0  0       0
	///                        0               0
	///                           0 0 0 0 0 0
	///
	/// 	}
	///
	///
	/// 	}

	/// }


}
