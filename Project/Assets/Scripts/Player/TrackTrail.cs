using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TrackTrail : MonoBehaviour {
	private List<Person> people;

	private void Awake() {
		people = new List<Person>();
	}

	private void Start() {
		AudioManager.instance.BeatOnUpdate += OnBeat;
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

	[SerializeField]
	private SplineTrailRenderer _shadowSplineTrailRenderer;
	public SplineTrailRenderer shadowSplineTrailRenderer
	{
		get
		{
			return _shadowSplineTrailRenderer;
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
		_splineTrailRenderer.Clear();
		_splineTrailRenderer.emit = true;
		if (_shadowSplineTrailRenderer != null) _shadowSplineTrailRenderer.emit = true;
	}

	public void DeactivateTrail()
	{
		_active = false;
		_splineTrailRenderer.emit = false;
		if (_shadowSplineTrailRenderer != null) _shadowSplineTrailRenderer.emit = false;
	}

	public void AddPerson(Person person) 
	{
		people.Add(person);
		person.transform.SetParent(transform);
		person.SetTrail(this);
	}

	public void SetTrackType(AudioManager.TrackTypes trackType) {
		_trackType = trackType;
		_splineTrailRenderer.vertexColor = ColorManager.GetColorForTrackType(trackType);
		if (_shadowSplineTrailRenderer != null) _shadowSplineTrailRenderer.vertexColor = ColorManager.GetShadowColorForTrackType(trackType);
	}

	private void OnBeat() {
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
