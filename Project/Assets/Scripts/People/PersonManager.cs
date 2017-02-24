using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WhitDataTypes;

public class PersonManager : MonoBehaviour {
	[SerializeField] private Person personPrefab;

	private List<Person> people;

	private void Awake() {
		people = new List<Person>();
	}

	private void Start() {
		personPrefab.CreatePool(50);
		AudioManager.instance.BeatOnUpdate += OnBeat;
	}

	public void AddPersonToDrumTrail() {
		AddPersonToTrail(Player_Controller.instance.GetDrumTrail());
	}

	public void AddPersonToPizzTrail() {
		AddPersonToTrail(Player_Controller.instance.GetPizzTrail());
	}

	public void AddPersonToKeysTrail() {
		AddPersonToTrail(Player_Controller.instance.GetKeysTrail());
	}

	public void AddPersonToClavTrail() {
		AddPersonToTrail(Player_Controller.instance.GetClavTrail());
	}

	public void AddPersonToBassTrail() {
		AddPersonToTrail(Player_Controller.instance.GetBassTrail());
	}

	private void AddPersonToTrail(Player_Trail trail) {
		Person person = CreatePerson();
		trail.AddPerson(person);
	}

	private Person CreatePerson() {
		Person person = personPrefab.Spawn();
		person.transform.SetParent(transform);
		people.Add(person);
		return person;
	}

	private void Update() {
		if (Input.GetKeyDown(KeyCode.Alpha1)) AddPersonToDrumTrail();
		else if (Input.GetKeyDown(KeyCode.Alpha2)) AddPersonToPizzTrail();
		else if (Input.GetKeyDown(KeyCode.Alpha3)) AddPersonToKeysTrail();
		else if (Input.GetKeyDown(KeyCode.Alpha4)) AddPersonToClavTrail();
		else if (Input.GetKeyDown(KeyCode.Alpha5)) AddPersonToBassTrail();
	}

	private void OnBeat() {
		for (int i = 0; i < people.Count; i++) {
			people[i].OnBeat();
		}
	}
}
