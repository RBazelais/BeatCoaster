using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
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

	private float _decayVal = 1;
	public float decayVal {
		get {
			return _decayVal;
		}
		set {
			_decayVal = value;
			OnDecayChange(_decayVal);
		}
	}

	private void OnDecayChange(float decay) {
		for (int i = 0; i < people.Count; i++) {
			people[i].OnTrackDecayChange(decay);
		}
	}

	public void SetActive ()
	{
		_active = true;
	}

	public void InitTrail() {
		_capSprite.enabled = true;

		_splineTrailRenderer.vertexColor = ColorManager.GetColorForTrackType (trackType);
		_capSprite.color = ColorManager.GetColorForTrackType (trackType);

		_splineTrailRenderer.Clear ();
		if (_shadowSplineTrailRenderer != null)
			_shadowSplineTrailRenderer.Clear ();

		_splineTrailRenderer.emit = true;
		if (_shadowSplineTrailRenderer != null)
			_shadowSplineTrailRenderer.emit = true;
	}

	public void ActivateTrail ()
	{
		AudioManager.instance.GetTrack(_trackType).volume = 1f;
		InitTrail();
	}

	public void DeactivateTrail ()
	{
		Sequence peopleKillSequence = DOTween.Sequence();
		for(int i = 0; i < people.Count; i++) {
			people[i].Reset();
			people[i].SetTrail(null, Person.PersonMoveType.Follow);
			people[i].transform.parent = null;
			peopleKillSequence.Insert(0, people[i].transform.DOMove(new Vector3(UnityEngine.Random.Range(-50,50), 10, 0), .33f).SetRelative(true));
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

	public void ResetDecay(bool playAgain = true) {
		_decaySequence.Kill();
		AudioManager.instance.GetTrack(_trackType).volume = 1f;

		decayVal = 1;
		ResetTrailAppearance();
		Debug.Log(trackType.ToString() + " reset track sequence");

		_decaySequence = DOTween.Sequence ();
		_decaySequence.Insert (0, AudioManager.instance.GetTrack (_trackType).DOFade (0f, GameManager.instance.fadeDuration));
		_decaySequence.Insert (0, DOTween.To (() => splineTrailRenderer.vertexColor, x => splineTrailRenderer.vertexColor = x, Color.black, GameManager.instance.fadeDuration).SetEase(Ease.Linear));
		if (shadowSplineTrailRenderer) _decaySequence.Insert (0, DOTween.To (() => shadowSplineTrailRenderer.vertexColor, x => shadowSplineTrailRenderer.vertexColor = x, Color.black, GameManager.instance.fadeDuration).SetEase(Ease.Linear));
		_decaySequence.Insert (0, DOTween.To (() => decayVal, x => decayVal = x, 0, GameManager.instance.fadeDuration));
		_decaySequence.SetDelay (GameManager.instance.fadeDelay).OnComplete(() => {
			DeactivateTrail();
			Player_Controller.instance.SetTrackOrder();
			AudioManager.instance.RestartBeats();
			Debug.Log(trackType.ToString() + " decay sequence finished");
			if (trackType == AudioManager.TrackTypes.Bass) {
				GameManager.instance.SetState(GameManager.GameState.End);
			}
		});
		if (playAgain) _decaySequence.Play();
	}

	public void SetTrackType (AudioManager.TrackTypes trackType)
	{
		_trackType = trackType;

		_splineTrailRenderer.vertexColor = ColorManager.GetColorForTrackType (trackType);
		_capSprite.color = ColorManager.GetColorForTrackType (trackType);
		if (_shadowSplineTrailRenderer != null)
			_shadowSplineTrailRenderer.vertexColor = ColorManager.GetShadowColorForTrackType (trackType);

		_decaySequence = DOTween.Sequence ();
		_decaySequence.Insert (0, AudioManager.instance.GetTrack (_trackType).DOFade (0f, GameManager.instance.fadeDuration));
		_decaySequence.Insert (0, DOTween.To (() => splineTrailRenderer.vertexColor, x => splineTrailRenderer.vertexColor = x, Color.black, GameManager.instance.fadeDuration).SetEase(Ease.Linear));
		if (shadowSplineTrailRenderer) _decaySequence.Insert (0, DOTween.To (() => shadowSplineTrailRenderer.vertexColor, x => shadowSplineTrailRenderer.vertexColor = x, Color.black, GameManager.instance.fadeDuration).SetEase(Ease.Linear));
		_decaySequence.Insert (0, DOTween.To (() => decayVal, x => decayVal = x, 0, GameManager.instance.fadeDuration));
		_decaySequence.SetDelay (GameManager.instance.fadeDelay).OnComplete(() => {
			DeactivateTrail();
			Player_Controller.instance.SetTrackOrder();
			AudioManager.instance.RestartBeats();
			Debug.Log(trackType.ToString() + " decay sequence finished");
			if (trackType == AudioManager.TrackTypes.Bass) {
				GameManager.instance.SetState(GameManager.GameState.End);
			}
		});
	}

	public void ResetTrailAppearance() {
		_splineTrailRenderer.vertexColor = ColorManager.GetColorForTrackType (trackType);
		if (_shadowSplineTrailRenderer) _shadowSplineTrailRenderer.vertexColor = ColorManager.GetShadowColorForTrackType(trackType);
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
