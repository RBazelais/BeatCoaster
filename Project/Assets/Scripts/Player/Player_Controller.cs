using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WhitDataTypes;

public class Player_Controller : MonoBehaviour
{
	public enum PlayerState
	{
		Idle,
		Active,
		Drop
	}

	public static Player_Controller instance;

	[SerializeField]
	private PlayerState _playerState;
	public PlayerState playerState
	{
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
	public float yCenter
	{
		get
		{
			return _yCenter;
		}
	}

	public TrackTrail GetDrumTrail() 
	{
		return _drumTrailRenderer;
	}

	public TrackTrail GetPizzTrail() 
	{
		return _pizzTrailRenderer;
	}

	public TrackTrail GetKeysTrail() 
	{
		return _keysTrailRenderer;
	}

	public TrackTrail GetClavTrail() 
	{
		return _clavTrailRenderer;
	}

	public TrackTrail GetBassTrail() 
	{
		return _bassTrailRenderer;
	}

	void Awake() {
		if(!instance)
			instance = this;
	}

	public void StartPlaying() {
		_bassTrailRenderer.ActivateTrail();
	}

	private void ActivateAllTrails() {
		_bassTrailRenderer.ActivateTrail();
		_clavTrailRenderer.ActivateTrail();
		_drumTrailRenderer.ActivateTrail();
		_keysTrailRenderer.ActivateTrail();
		_pizzTrailRenderer.ActivateTrail();
	}

	private void DeactivateAllTrails() {
		_bassTrailRenderer.DeactivateTrail();
		_clavTrailRenderer.DeactivateTrail();
		_drumTrailRenderer.DeactivateTrail();
		_keysTrailRenderer.DeactivateTrail();
		_pizzTrailRenderer.DeactivateTrail();
	}

	public void OnTitleEnterState() {
		ActivateAllTrails();
	}

	public void OnTitleUpdateState() {

	}

	public void OnPlayingEnterState() {
		DeactivateAllTrails();
		_bassTrailRenderer.ActivateTrail();
	}

	public void OnPlayingUpdateState() {
		if(Input.GetKeyDown(KeyCode.Q))
		{
			SetState(PlayerState.Drop);
		}
		if(Input.GetKeyDown(KeyCode.E))
		{
			SetState(PlayerState.Active);
		}

		if(_playerState == PlayerState.Active) {
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
		}
		else if (_playerState == PlayerState.Drop) {
			Mathf.Clamp(_yPos -= .075f, -2, 2);

			transform.position = new Vector3(_pointPos, transform.position.y + _yPos, 0);
		}
	}

	public void OnGameOverEnterState() {

	}

	public void OnGameOverUpdateState() {

	}

	void SetState(PlayerState state) {
		switch(state) {
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

	public void OnPreUpdateState() {
		
	}

	public void OnPostUpdateState() {
		if (_yPos <= .025f && _yPos >= -.025f) {
			_yPos = 0;
		}
		else if (_yPos > 0) {
			_yPos -= .025f;
		} else if (_yPos < 0) {
			_yPos += .025f;
		}

		_yPos = Mathf.Clamp(_yPos, -.4f, .4f);

		transform.position = new Vector3 (_pointPos, Mathf.Clamp (transform.position.y + _yPos, _yCenter -7.5f, _yCenter + 12.5f - (1 * ActiveTrails ())), 0);

		_pointPos += 1f;

		CatmullRomSpline spline = _bassTrailRenderer.splineTrailRenderer.spline;

		int segment = Mathf.Max(spline.NbSegments - 15, 0);
		float distFromStart = spline.GetSegmentDistanceFromStart(segment);

		Vector3 pos = spline.FindPositionFromDistance(distFromStart);

		Vector3 endPos = spline.FindPositionFromDistance(spline.GetSegmentDistanceFromStart(spline.NbSegments - 1));
		float diff = (endPos - pos).magnitude;
		_col.transform.position = pos;
	}

	private void LateUpdate() {
		
//		_col.transform.localPosition = new Vector3(-14, _col.transform.localPosition.y, 0);
	}

	public void ActivateTrack(AudioManager.TrackTypes type)
	{
		switch(type) {
			case AudioManager.TrackTypes.Bass:
				if(!_bassTrailRenderer.active)
					_bassTrailRenderer.ActivateTrail();
				break;
			case AudioManager.TrackTypes.Clav:
				if(!_clavTrailRenderer.active)
					_clavTrailRenderer.ActivateTrail();
				break;
			case AudioManager.TrackTypes.Drums:
				if(!_drumTrailRenderer.active)
					_drumTrailRenderer.ActivateTrail();
				break;
			case AudioManager.TrackTypes.Keys:
				if(!_keysTrailRenderer.active)
					_keysTrailRenderer.ActivateTrail();
				break;
			case AudioManager.TrackTypes.Pizz:
				if(!_pizzTrailRenderer.active)
					_pizzTrailRenderer.ActivateTrail();
				break;
		}
	}

	public TrackTrail GetTrack(AudioManager.TrackTypes type)
	{
		TrackTrail track = null;
		switch(type) {
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
