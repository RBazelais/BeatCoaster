using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WhitDataTypes;
using System;
using DG.Tweening;

public class PersonManager : MonoBehaviour {
	public Action SignalPersonAddedToCollection;
	public Action SignalPersonRemovedFromCollection;

	public static int listenersPerPersonUnit = 10;

	private static PersonManager _instance;
	public static PersonManager instance {
		get {
			if (_instance == null) {
				_instance = GameObject.FindObjectOfType<PersonManager>();
			}
			return _instance;
		}
	}

	public IntRange segmentDistanceRange = new IntRange(5, 40);
	[Range(0.5f, 2)] public float wobbleSpeed = 1;
	[Range(0.1f, 10)] public float wobbleIntensity = 1;
	[SerializeField] private AudioClip personCollectClip;
	[SerializeField] private Person personPrefab;

	private List<Person> people;
	private List<Person> collectedPeople;

	public int GetListenerCount() {
		return collectedPeople.Count * PersonManager.listenersPerPersonUnit;
	}

	private void Awake() {
		people = new List<Person>();
		collectedPeople = new List<Person>();
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

	public void AddPersonToTrail(TrackTrail trail) {
		Person person = CreatePerson();
		trail.AddPerson(person, Person.PersonMoveType.Follow);
	}

	public void TransferPeople(TrackTrail fromTrail, TrackTrail toTrail) {
		for (int i = 0; i < fromTrail.people.Count; i++) {
			Person person = fromTrail.people[i];
			TransferPerson(person, fromTrail, toTrail);
		}
	}

	public void OnPersonJoinedCollectedPeople(Person person) {
		AudioManager.instance.PlaySound(personCollectClip);
		collectedPeople.Add(person);
		if (SignalPersonAddedToCollection != null) SignalPersonAddedToCollection();
	}

	public void OnPersonLeftCollectedPeople(Person person) {
		collectedPeople.Remove(person);
		if (SignalPersonRemovedFromCollection != null) SignalPersonRemovedFromCollection();
	}

	private void TransferPerson(Person person, TrackTrail fromTrail, TrackTrail toTrail) {
		if (person.trail != fromTrail) Debug.LogError("can't transfer person from a trail they're not on!");
		fromTrail.RemovePerson(person);
		toTrail.AddPerson(person, Person.PersonMoveType.Transfer);
	}

	public void DropPeople() {
		var bassTrail = Player_Controller.instance.GetBassTrail();
		var clavTrail = Player_Controller.instance.GetClavTrail();
		var keyTrail = Player_Controller.instance.GetKeysTrail();
		var drumTrail = Player_Controller.instance.GetDrumTrail();
		var pizzTrail = Player_Controller.instance.GetPizzTrail();

		Sequence peopleKillSequence = DOTween.Sequence(); 
		Person[] toDrop = collectedPeople.ToArray();
		for(int i = 0; i < toDrop.Length; i++) {
			var person = toDrop[i];
			peopleKillSequence.Insert(i * 6/collectedPeople.Count, person.transform.DOMove(new Vector3(20, UnityEngine.Random.Range(-10,10), 0), .33f).SetRelative(true).OnStart(() => {
				person.Reset();
				person.SetTrail(null, Person.PersonMoveType.Follow);
				person.transform.parent = null;

				GameManager.instance.AddDroppedListeners(listenersPerPersonUnit);
				OnPersonLeftCollectedPeople(person);
			}));
		}

		peopleKillSequence.Play().OnComplete(() =>
			{	
				bassTrail.people.Clear();
				clavTrail.people.Clear();
				keyTrail.people.Clear();
				drumTrail.people.Clear();
				pizzTrail.people.Clear();

				for(int i = 0; i < collectedPeople.Count; i++) {
					collectedPeople[i].Recycle();
				}
				collectedPeople.Clear();
			});
	}

	private Person CreatePerson() {
		Person person = personPrefab.Spawn();
		person.transform.SetParent(transform);
		people.Add(person);
		return person;
	}

	private void OnBeat() {
		for (int i = 0; i < people.Count; i++) {
			people[i].OnBeat();
		}
	}
}
