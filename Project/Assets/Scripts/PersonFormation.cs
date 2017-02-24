using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WhitDataTypes;
using DG.Tweening;

public class PersonFormation : MonoBehaviour {

	/*
		override segmentDistanceRange with Alpha Key
		keep wobble
		// disable ability to add more crowd people
		persons will be positioned on the grid from [back, front] = [40, 0]
		arrange limited number of persons into new positions on grid onButtonPress (Alpha9)

	*/


	/*
	//Person Manager

	public void FansGetInFormation() {
		FansGetInFormation(Player_Controller.instance.FansGetInFormation());
	}

 	//	public void FansGetInFormation() {
		FansGetInFormation(Player_Controller.instance.FansGetInFormation());
	}

	private void FansGetInFormation(Player_Trail trail) {
		//Person Move = ChangePostionInGrid();
		//trail.MovePerson(person);
	}

	private Person ChangePostionInGridn() {
		Person person = personPrefab.Spawn();
		person.transform.SetParent(transform);
		people.Add(person);
		return person;
	}
	
    // was in update if statement in Persone Manager
	else if (Input.GetKeyDown (KeyCode.Alpha9))
	FansGetInFormation ();

	// Player Controller
	[SerializeField]
	private Player_Trail _FansGetInFormation;

	public IntRange MakeFormation = new IntRange (10, 30);

	public Player_Trail FansGetInFormation() 
	{
		return _FansGetInFormation;
	}

	//Player_Trail
	public void MovePerson(Person person) 
	{
		AddPerson().disabled ;
		people.Move(person);
		person.transform.SetParent(transform);
		person.SetTrail(this);
	}
*/

}
