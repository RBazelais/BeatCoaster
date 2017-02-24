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
	private Player_Trail _drumTrailRenderer;

	[SerializeField]
	private Player_Trail _pizzTrailRenderer;

	[SerializeField]
	private Player_Trail _keysTrailRenderer;

	[SerializeField]
	private Player_Trail _clavTrailRenderer;

	[SerializeField]
	private Player_Trail _bassTrailRenderer;

	[SerializeField]
	private Player_Collider _col;

	public IntRange segmentDistanceRange = new IntRange(5, 40);
	[Range(0.5f, 2)] public float wobbleSpeed = 1;
	[Range(0.1f, 10)] public float wobbleIntensity = 1;

	private float _yPos = 0, _lastYPos, _yCenter;
	public float yCenter
	{
		get
		{
			return _yCenter;
		}
	}

	public Player_Trail GetDrumTrail() 
	{
		return _drumTrailRenderer;
	}

	public Player_Trail GetPizzTrail() 
	{
		return _pizzTrailRenderer;
	}

	public Player_Trail GetKeysTrail() 
	{
		return _keysTrailRenderer;
	}

	public Player_Trail GetClavTrail() 
	{
		return _clavTrailRenderer;
	}

	public Player_Trail GetBassTrail() 
	{
		return _bassTrailRenderer;
	}

	void Awake() {
		if(!instance)
			instance = this;
	}

	void Start() {
		_bassTrailRenderer.ActivateTrail();
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
		}
		else if (_playerState == PlayerState.Drop) {
			Mathf.Clamp(_yPos -= .075f, -2, 2);

			transform.position = new Vector3(_pointPos, transform.position.y + _yPos, 0);
		}


		_pointPos += 1f;
	}

	private void LateUpdate() {
		CatmullRomSpline spline = _bassTrailRenderer.splineTrailRenderer.spline;

		int segment = Mathf.Max(spline.NbSegments - 15, 0);
		float distFromStart = spline.GetSegmentDistanceFromStart(segment);

		Vector3 pos = spline.FindPositionFromDistance(distFromStart);

		Vector3 endPos = spline.FindPositionFromDistance(spline.GetSegmentDistanceFromStart(spline.NbSegments - 1));
		float diff = (endPos - pos).magnitude;
		_col.transform.position = pos;
//		_col.transform.localPosition = new Vector3(-14, _col.transform.localPosition.y, 0);
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
