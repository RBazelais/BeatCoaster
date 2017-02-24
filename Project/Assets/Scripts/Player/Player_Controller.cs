﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WhitDataTypes;
using DG.Tweening;

public class Player_Controller : MonoBehaviour
{
	public enum PlayerState
	{
		None,
		Idle,
		Active,
		Drop
	}

	public static Player_Controller instance;

	[SerializeField]
	private PlayerState _playerState;

	public PlayerState playerState {
		get { return _playerState; }
	}

	private float _pointPos = 0;

	[SerializeField]
	private TrackTrail _drumTrailRenderer;

	[SerializeField]
	private TrackTrail _pizzTrailRenderer;

	[SerializeField]
	private TrackTrail _keysTrailRenderer;

	[SerializeField]
	private TrackTrail _clavTrailRenderer;

	[SerializeField]
	private TrackTrail _bassTrailRenderer;

	[SerializeField]
	private Player_Collider _col;

	private float _yPos = 0, _lastYPos, _yCenter;

	public float yCenter {
		get {
			return _yCenter;
		}
	}

	public TrackTrail GetDrumTrail ()
	{
		return _drumTrailRenderer;
	}

	public TrackTrail GetPizzTrail ()
	{
		return _pizzTrailRenderer;
	}

	public TrackTrail GetKeysTrail ()
	{
		return _keysTrailRenderer;
	}

	public TrackTrail GetClavTrail ()
	{
		return _clavTrailRenderer;
	}

	public TrackTrail GetBassTrail ()
	{
		return _bassTrailRenderer;
	}

	void Awake ()
	{
		if (!instance)
			instance = this;
	}

	public void StartPlaying ()
	{
		_bassTrailRenderer.SetActive ();
		_bassTrailRenderer.ActivateTrail ();
	}

	public void SetTrails ()
	{
		_bassTrailRenderer.SetTrackType (AudioManager.TrackTypes.Bass);
		_clavTrailRenderer.SetTrackType (AudioManager.TrackTypes.Clav);
		_drumTrailRenderer.SetTrackType (AudioManager.TrackTypes.Drums);
		_keysTrailRenderer.SetTrackType (AudioManager.TrackTypes.Keys);
		_pizzTrailRenderer.SetTrackType (AudioManager.TrackTypes.Pizz);
	}

	private void ActivateAllTrails ()
	{
		_bassTrailRenderer.ActivateTrail ();
		_clavTrailRenderer.ActivateTrail ();
		_drumTrailRenderer.ActivateTrail ();
		_keysTrailRenderer.ActivateTrail ();
		_pizzTrailRenderer.ActivateTrail ();
	}

	private void DeactivateAllTrails ()
	{
		_bassTrailRenderer.DeactivateTrail ();
		_clavTrailRenderer.DeactivateTrail ();
		_drumTrailRenderer.DeactivateTrail ();
		_keysTrailRenderer.DeactivateTrail ();
		_pizzTrailRenderer.DeactivateTrail ();
	}

	public void OnTitleEnterState ()
	{
		ActivateAllTrails ();
	}

	public void OnTitleUpdateState ()
	{

	}

	public void OnPlayingEnterState ()
	{
		DeactivateAllTrails ();
		_bassTrailRenderer.ActivateTrail ();
	}

	public void OnPlayingUpdateState ()
	{
		if (Input.GetKeyDown (KeyCode.Q) || Input.GetKeyDown (KeyCode.DownArrow)) {
			SetState (PlayerState.Drop);
		}
		if (Input.GetKeyDown (KeyCode.E) || Input.GetKeyDown (KeyCode.UpArrow)) {
			SetState (PlayerState.Active);
		}

		if (_playerState == PlayerState.Active) {
			bool input = false;
			if (transform.position.y > _yCenter - 4) {
				if (Input.GetKey (KeyCode.S)) {
					input = true;
					_yPos -= .05f;
				}
			}

			if (transform.position.y < _yCenter + 11.5f - (1 * ActiveTrails ())) {
				if (Input.GetKey (KeyCode.W)) {
					input = true;
					_yPos += .05f;
				}
			}
		} else if (_playerState == PlayerState.Drop) {
			Mathf.Clamp (_yPos -= .075f, -2, 2);

			transform.position = new Vector3 (_pointPos, transform.position.y + _yPos, 0);
		}
	}

	public void OnGameOverEnterState ()
	{

	}

	public void OnGameOverUpdateState ()
	{

	}

	public void SetState (PlayerState state)
	{
		switch (state) {
		case PlayerState.Active:
			_yCenter = transform.position.y;
			break;
		case PlayerState.Drop:
			break;
		case PlayerState.Idle:
			break;

		}
		_playerState = state;
	}

	void Update ()
	{

	}

	public void OnPreUpdateState ()
	{
		
	}

	public void OnPostUpdateState ()
	{
		if (_yPos <= .025f && _yPos >= -.025f) {
			_yPos = 0;
		} else if (_yPos > 0) {
			_yPos -= .025f;
		} else if (_yPos < 0) {
			_yPos += .025f;
		}

		if (_playerState == PlayerState.Active || _playerState == PlayerState.Idle) {
			_yPos = Mathf.Clamp (_yPos, -.4f, .4f);
			transform.position = new Vector3 (_pointPos, Mathf.Clamp (transform.position.y + _yPos, _yCenter - 7.5f, _yCenter + 12.5f - (1 * ActiveTrails ())), 0);
		} else if (_playerState == PlayerState.Drop) {
			_yPos = Mathf.Clamp (_yPos, -2f, 2f);
			transform.position = new Vector3 (_pointPos, transform.position.y + _yPos, 0);
		}

		_pointPos += 1f;

		CatmullRomSpline spline = _bassTrailRenderer.splineTrailRenderer.spline;

		int segment = Mathf.Max (spline.NbSegments - 15, 0);
		float distFromStart = spline.GetSegmentDistanceFromStart (segment);

		Vector3 pos = spline.FindPositionFromDistance (distFromStart);

		Vector3 endPos = spline.FindPositionFromDistance (spline.GetSegmentDistanceFromStart (spline.NbSegments - 1));
		float diff = (endPos - pos).magnitude;
		_col.transform.position = pos;
	}

	private void LateUpdate ()
	{
		
//		_col.transform.localPosition = new Vector3(-14, _col.transform.localPosition.y, 0);
	}

	public void ActivateTrack (AudioManager.TrackTypes type, TrackTrail fromTrail)
	{
		TrackTrail trail = GetTrack (type);
		SetTrackOrder ();
		trail.ActivateTrail ();

		if (type != AudioManager.TrackTypes.Bass)
			trail.decaySequence.Play ();
		
		PersonManager.instance.TransferPeople (fromTrail, trail);
	}

	public void SetTrackOrder ()
	{

		int val = 0;
		for (int i = 0; i < 5; i++) {
			AudioManager.TrackTypes type = (AudioManager.TrackTypes)i;
			TrackTrail track = GetTrack (type);
			if (track.active) {
				track.transform.localPosition = new Vector3 (0, -2 + val, 0);
				val++;
			}
		}
	}

	public TrackTrail GetTrack (AudioManager.TrackTypes type)
	{
		TrackTrail track = null;
		switch (type) {
		case AudioManager.TrackTypes.Bass:
			track = _bassTrailRenderer;
			break;
		case AudioManager.TrackTypes.Clav:
			track = _clavTrailRenderer;
			break;
		case AudioManager.TrackTypes.Drums:
			track = _drumTrailRenderer;
			break;
		case AudioManager.TrackTypes.Keys:
			track = _keysTrailRenderer;
			break;
		case AudioManager.TrackTypes.Pizz:
			track = _pizzTrailRenderer;
			break;
		}
		return track;
	}

	public AudioManager.TrackTypes GetLongestDuration ()
	{
		
		AudioManager.TrackTypes type = AudioManager.TrackTypes.Clav;
		float duration = 0;
		if (_bassTrailRenderer.decaySequence.IsPlaying ())
			type = AudioManager.TrackTypes.Bass;
		
		if (_clavTrailRenderer.decaySequence.IsPlaying ())
		if (_clavTrailRenderer.decaySequence.Elapsed() > duration) {
			type = AudioManager.TrackTypes.Clav;
			duration = _clavTrailRenderer.decaySequence.Elapsed();
		}

		if (_drumTrailRenderer.decaySequence.IsPlaying ())
		if (_drumTrailRenderer.decaySequence.Elapsed() > duration) {
			type = AudioManager.TrackTypes.Drums;
			duration = _drumTrailRenderer.decaySequence.Elapsed();
		}

		if (_keysTrailRenderer.decaySequence.IsPlaying ())
		if (_keysTrailRenderer.decaySequence.Elapsed() > duration) {
			type = AudioManager.TrackTypes.Keys;
			duration = _keysTrailRenderer.decaySequence.Elapsed();
		}

		if (_pizzTrailRenderer.decaySequence.IsPlaying ())
		if (_pizzTrailRenderer.decaySequence.Elapsed() > duration) {
			type = AudioManager.TrackTypes.Pizz;
			duration = _pizzTrailRenderer.decaySequence.Elapsed();
		}

		return type;
	}

	int ActiveTrails ()
	{
		int active = 0;
		if (_drumTrailRenderer.active)
			active++;
		if (_pizzTrailRenderer.active)
			active++;
		if (_keysTrailRenderer.active)
			active++;
		if (_clavTrailRenderer.active)
			active++;
		if (_bassTrailRenderer.active)
			active++;

		return active;
	}
}
