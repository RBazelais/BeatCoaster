using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WhitDataTypes;
using DG.Tweening;

public class Player_Controller : MonoBehaviour
{
	public enum PlayerState
	{
		None,
		Idle,
		Active,
		Drop, 
		GameOver
	}

	public static Player_Controller instance;

	[SerializeField]
	private PlayerState _playerState;

	public PlayerState playerState {
		get { return _playerState; }
	}

	private float _pointPos = 0;

	[SerializeField]
	private Transform _dropReminder;

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

	[SerializeField] private ParticleSystem dropParticles;

	[SerializeField]
	private Player_Collider _col;

	private float _yPos = 0, _lastYPos, _yCenter;

	private bool _hitSpaceForDrop, _dropHit;
	public bool dropHit
	{
		get{ return _dropHit; }
	}

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

	private void OnUpTouch() {
		_yPos += .065f;
	}

	private void OnDownTouch() {
		_yPos -= .065f;
	}

	public void OnEnemyCollected(Enemy enemy) {
		_col.OnEnemyCollected(enemy);
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


	public void OnDropButtonPressed() {
		Debug.Log("bha");
		if(_playerState == PlayerState.Idle)
		{
			if(_hitSpaceForDrop)
			{
				_dropHit = true;
			}
		}
	}

	public void OnTitleUpdateState ()
	{

	}

	public void OnPlayingEnterState ()
	{
		TouchInput.instance.EnableInput();

		DeactivateAllTrails ();
		_bassTrailRenderer.ActivateTrail ();
		TouchInput.instance.SignalUp += OnUpTouch;
		TouchInput.instance.SignalDown += OnDownTouch;
	}

	public void OnPlayingExitState() {
		TouchInput.instance.DisableInput();

		TouchInput.instance.SignalUp -= OnUpTouch;
		TouchInput.instance.SignalDown -= OnDownTouch;
	}

	public void OnPlayingUpdateState ()
	{
		if (ActiveTrails () == 5 && _playerState == PlayerState.Active && PersonManager.instance.GetListenerCount() > 0) {
				TriggerBuildUp ();
		}

		if(_playerState == PlayerState.Idle)
		{
			if(_hitSpaceForDrop)
			{
				if(Input.GetKeyDown(KeyCode.Space))
					_dropHit = true;
			}
		}

		if (_playerState == PlayerState.Active || _playerState == PlayerState.Idle) {
			if (transform.position.y > _yCenter - 4) {
				if (Input.GetKey (KeyCode.S) || Input.GetKeyDown (KeyCode.DownArrow)) {
					_yPos -= .065f;
				}
			}

			if (transform.position.y < _yCenter + 11.5f - (1 * ActiveTrails ())) {
				if (Input.GetKey (KeyCode.W) || Input.GetKeyDown (KeyCode.UpArrow)) {
					_yPos += .065f;
				}
			}
		} else if (_playerState == PlayerState.Drop) {
			if (Input.GetKey (KeyCode.S) || Input.GetKeyDown (KeyCode.DownArrow)) {
				_yPos -= .065f;
			}

			if (Input.GetKey (KeyCode.W) || Input.GetKeyDown (KeyCode.UpArrow)) {
				_yPos += .065f;
			}
		}
	}

	void TriggerBuildUp ()
	{
		_playerState = PlayerState.Idle;
		AudioManager.instance.PlaySound(AudioManager.instance.whiteNoiseClip, 1);
		AudioManager.instance.TriggerDrop ();

		_bassTrailRenderer.decaySequence.Kill ();
		_clavTrailRenderer.decaySequence.Kill ();
		_drumTrailRenderer.decaySequence.Kill ();
		_keysTrailRenderer.decaySequence.Kill ();
		_pizzTrailRenderer.decaySequence.Kill ();

		_bassTrailRenderer.ResetTrailAppearance ();
		_clavTrailRenderer.ResetTrailAppearance ();
		_drumTrailRenderer.ResetTrailAppearance ();
		_keysTrailRenderer.ResetTrailAppearance ();
		_pizzTrailRenderer.ResetTrailAppearance ();

		for (int i = 0; i < EnemyManager.instance.enemies.Count; i++) {
			EnemyManager.instance.enemies[i].Recycle ();
		}
		EnemyManager.instance.enemies.Clear ();
	}

	void EnableDropNotification ()
	{
		_dropReminder.gameObject.SetActive (true);
		//AudioManager.instance.sfxWhiteNoise.Play ();
	}

	void DisableDropNotification() {
		_dropReminder.gameObject.SetActive(false);
		//AudioManager.instance.sfxWhiteNoise.Stop();
	}

	public IEnumerator YieldToDropEnable() {
		yield return new WaitForSeconds(11.5f);
		_hitSpaceForDrop = true;
		EnableDropNotification();
	}

	public IEnumerator PlayDrop() {
		SetState (PlayerState.Drop);
		DisableDropNotification();

		PersonManager.instance.DropPeople();
		GameManager.instance.SpeedFadeTimes();

		yield return new WaitForSeconds(7.5f);

		ResetToBass();
	}

	public void ResetToBass() {
		_dropHit = false;
		_hitSpaceForDrop = false;

		DisableDropNotification();

		_clavTrailRenderer.DeactivateTrail();
		_drumTrailRenderer.DeactivateTrail();
		_keysTrailRenderer.DeactivateTrail();
		_pizzTrailRenderer.DeactivateTrail();

		SetTrackOrder();

		SetState(PlayerState.Active);
		AudioManager.instance.RestartBeats();
	}

	public void OnGameOverEnterState ()
	{

	}

	public void OnGameOverUpdateState ()
	{
		if (_playerState == PlayerState.GameOver) {
			DeactivateAllTrails ();
			GameManager.instance.SetState (GameManager.GameState.End);
			//Play Game Over Menu animation
		}
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
		case PlayerState.GameOver:
			break;

		}

		if (state == PlayerState.Drop) dropParticles.Play();
		else dropParticles.Stop();

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
			_yPos = Mathf.Clamp (_yPos, -.6f, .6f);
			transform.position = new Vector3 (_pointPos, Mathf.Clamp (transform.position.y + _yPos, _yCenter - 7.5f, _yCenter + 12.5f - (1 * ActiveTrails ())), 0);
		} else if (_playerState == PlayerState.Drop) {
			_yPos = Mathf.Clamp (_yPos, -1.5f, -.75f);
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

		StartBassDecaySequenceIfNeeded();
	}

	private void StartBassDecaySequenceIfNeeded() {
		if (!OnlyActiveTrackIsBass()) return;

		TrackTrail bass = GetTrack(AudioManager.TrackTypes.Bass);
		if (bass.decaySequence.IsPlaying()) return;

		bass.decaySequence.Play();
	}

	private bool OnlyActiveTrackIsBass() {
		return
			GetTrack(AudioManager.TrackTypes.Bass).active &&
			!GetTrack(AudioManager.TrackTypes.Clav).active &&
			!GetTrack(AudioManager.TrackTypes.Drums).active &&
			!GetTrack(AudioManager.TrackTypes.Keys).active &&
			!GetTrack(AudioManager.TrackTypes.Pizz).active;
	}

	public void ActivateTrack (AudioManager.TrackTypes type, TrackTrail fromTrail)
	{
		TrackTrail bassTrack = GetTrack(AudioManager.TrackTypes.Bass);
		bassTrack.ResetDecay(false);

		TrackTrail track = GetTrack (type);
		if(!track.active) {
			track.SetActive ();
			SetTrackOrder ();
			track.ActivateTrail ();

			track.ResetDecay(type != AudioManager.TrackTypes.Bass);

			PersonManager.instance.TransferPeople (fromTrail, track);
		}
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
