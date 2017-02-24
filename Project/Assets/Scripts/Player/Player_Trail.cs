using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Trail : MonoBehaviour {
	private List<Person> people;

	private void Awake() {
		people = new List<Person>();
	}

	[SerializeField]
	private AudioManager.TrackTypes _trackType;
	public AudioManager.TrackTypes trackType
	{
		get
		{
			return _trackType;
		}
	}

	[SerializeField]
	private SplineTrailRenderer _splineTrailRenderer;
	public SplineTrailRenderer splineTrailRenderer
	{
		get
		{
			return _splineTrailRenderer;
		}
	}

	private bool _active;
	public bool active
	{
		get
		{
			return _active;
		}
	}

	public void ActivateTrail()
	{
		_active = true;
		_splineTrailRenderer.emit = true;
	}

	public void DeactivateTrail()
	{
		_active = false;
		_splineTrailRenderer.emit = false;
	}

	public void AddPerson(Person person) 
	{
		people.Add(person);
		person.transform.SetParent(transform);
		person.SetTrail(this);
	}
}
