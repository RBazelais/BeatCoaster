using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TrackTrail : MonoBehaviour
{
	public List<Person> people {get; private set;}

	private void Awake() {
		people = new List<Person>();
	}

	private void Start ()
	{
		AudioManager.instance.BeatOnUpdate += OnBeat;
	}

	[SerializeField]
	private AudioManager.TrackTypes _trackType;

	public AudioManager.TrackTypes trackType {
		get {
			return _trackType;
		}
	}

	[SerializeField]
	private SplineTrailRenderer _splineTrailRenderer;

	public SplineTrailRenderer splineTrailRenderer {
		get {
			return _splineTrailRenderer;
		}
	}

	[SerializeField]
	private SplineTrailRenderer _shadowSplineTrailRenderer;

	public SplineTrailRenderer shadowSplineTrailRenderer {
		get {
			return _shadowSplineTrailRenderer;
		}
	}

	[SerializeField]
	private SpriteRenderer _capSprite;

	public SpriteRenderer capSprite {
		get{ return _capSprite; }
	}

	private bool _active;

	public bool active {
		get {
			return _active;
		}
	}

	private Sequence _decaySequence;
	public Sequence decaySequence
	{
		get{return _decaySequence;
		}
	}

	public void SetActive ()
	{
		_active = true;
	}

	public void ActivateTrail ()
	{
		_capSprite.enabled = true;

		_splineTrailRenderer.vertexColor = ColorManager.GetColorForTrackType (trackType);
		_capSprite.color = ColorManager.GetColorForTrackType (trackType);
		AudioManager.instance.GetTrack(_trackType).volume = 1;

		_splineTrailRenderer.Clear ();
		if (_shadowSplineTrailRenderer != null)
			_shadowSplineTrailRenderer.Clear ();
		
		_splineTrailRenderer.emit = true;
		if (_shadowSplineTrailRenderer != null)
			_shadowSplineTrailRenderer.emit = true;
	}

	public void DeactivateTrail ()
	{
		Sequence peopleKillSequence = DOTween.Sequence();
		for(int i = 0; i < people.Count; i++) {
			people[i].Reset();
			people[i].SetTrail(null, Person.PersonMoveType.Follow);
			people[i].transform.parent = null;
			peopleKillSequence.Insert(0, people[i].transform.DOMove(new Vector3(Random.Range(-50,50), 10, 0), .33f).SetRelative(true));
		}

		peopleKillSequence.Play().OnComplete(() =>
			{				
				for(int i = 0; i < people.Count; i++) {
					PersonManager.instance.OnPersonLeftCollectedPeople(people[i]);
					people[i].Recycle();
				}
				people.Clear();
			});

		_capSprite.enabled = false;
		_active = false;
		_splineTrailRenderer.emit = false;
		if (_shadowSplineTrailRenderer != null)
			_shadowSplineTrailRenderer.emit = false;
	}

	public void ResetDecay() {
		_decaySequence.Kill();
		AudioManager.instance.GetTrack(_trackType).volume = 1;

		ResetTrailAppearance();

		_decaySequence = DOTween.Sequence ();
		_decaySequence.Insert (0, AudioManager.instance.GetTrack (_trackType).DOFade (0f, 5f));
		_decaySequence.Insert (0, DOTween.To (() => splineTrailRenderer.vertexColor, x => splineTrailRenderer.vertexColor = x, Color.black, 5f).SetEase(Ease.Linear));
		_decaySequence.SetDelay (5f).OnComplete(() => {
			DeactivateTrail();
			Player_Controller.instance.SetTrackOrder();
			AudioManager.instance.RestartBeats();
		});
		_decaySequence.Play();
	}

	public void ResetTrailAppearance() {
		_splineTrailRenderer.vertexColor = ColorManager.GetColorForTrackType (trackType);
		_capSprite.color = ColorManager.GetColorForTrackType (trackType);
	}

	public void AddPerson(Person person, Person.PersonMoveType moveType) 
	{
		people.Add(person);
		person.transform.SetParent(transform, true);
		person.SetTrail(this, moveType);
	}

	public void RemovePerson(Person person) {
		if (!people.Contains(person)) Debug.LogError("can't remove person who's not on track");

		people.Remove(person);
	}

	public void SetTrackType (AudioManager.TrackTypes trackType)
	{
		_trackType = trackType;
		_splineTrailRenderer.vertexColor = ColorManager.GetColorForTrackType (trackType);
		_capSprite.color = ColorManager.GetColorForTrackType (trackType);
		if (_shadowSplineTrailRenderer != null)
			_shadowSplineTrailRenderer.vertexColor = ColorManager.GetShadowColorForTrackType (trackType);

		_decaySequence = DOTween.Sequence ();
		_decaySequence.Insert (0, AudioManager.instance.GetTrack (_trackType).DOFade (0f, 5f));
		_decaySequence.Insert (0, DOTween.To (() => splineTrailRenderer.vertexColor, x => splineTrailRenderer.vertexColor = x, Color.black, 5f).SetEase(Ease.Linear));
		_decaySequence.SetDelay (5f).OnComplete(() => {
			DeactivateTrail();
			Player_Controller.instance.SetTrackOrder();
			AudioManager.instance.RestartBeats();
		});
	}


	private void OnBeat ()
	{
//		float initialHeight = splineTrailRenderer.height;
//		float targetHeight = initialHeight * 0.5f;
//
//		float beatDuration = 60f / (float)AudioManager.instance.bpm;
//
//		Sequence s = DOTween.Sequence();
//		s.Append(DOTween.To(()=>{return splineTrailRenderer.height;}, (x)=>{splineTrailRenderer.height = x;}, targetHeight, beatDuration / 2f));
//		s.Append(DOTween.To(()=>{return splineTrailRenderer.height;}, (x)=>{splineTrailRenderer.height = x;}, initialHeight, beatDuration / 2f - 0.1f));
	}
}
