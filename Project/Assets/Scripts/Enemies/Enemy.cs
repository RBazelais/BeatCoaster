using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public enum EnemyState {
	Paused,
	Entering,
	Exiting,
	Exited,
	Collected
}

public class Enemy : MonoBehaviour {
	private float enterTimer = 0;
	private float exitTimer = 0;
	private float collectedTimer = 0;
	private EnemyState state = EnemyState.Paused;
	private float enterDur;
	private float exitDur;
	private float endVerticalPos;
	private float spawnVerticalPos;
	private float sineTimer = 0;
	private float sineAmplitude;
	private float sineFrequency;

	private Vector3 _collectedPos; 
	private bool _collected = false;

	private AudioManager.TrackTypes _trackType;
	public AudioManager.TrackTypes trackType
	{
		get{return _trackType;}
	}

	[SerializeField] private TrackTrail _trail;
	public TrackTrail trail
	{
		get{return _trail;}
	}

	public void Activate() {
		sineAmplitude = Random.Range(1f, 3f);
		sineFrequency = Random.Range(3f, 10f);

		_trail.ActivateTrail();

		spawnVerticalPos = transform.position.y;
		endVerticalPos = Player_Controller.instance.yCenter + GameManager.instance.enemyVerticalRange.GetRandom();

		enterDur = EnemyManager.instance.enemyEnterDurationRange.GetRandom();
		exitDur = EnemyManager.instance.enemyExitDurationRange.GetRandom();

		state = EnemyState.Entering;
	}

	public void AddPeople(int numPeople) {
		for (int i = 0; i < numPeople; i++) {
			PersonManager.instance.AddPersonToTrail(_trail);
		}
	}

	public void Collect()
	{
		_collected = true;
		_trail.capSprite.transform.DOPunchScale(new Vector3(4f,4f,0), .33f, 1, 2);
	}

	private void Update() {
		if (state == EnemyState.Paused) return;
		UpdateTimers();
		UpdateState();
		UpdatePosition();
	}

	private void UpdatePosition() {
		if (state == EnemyState.Entering) {// || state == OpponentState.Exiting) {
			transform.position = GetPosition();
		}
		if(state == EnemyState.Collected)
			transform.position = GetPlayerPosition();
	}

	private Vector3 GetPosition() {
		float percent = GetCurrentTimerPercent();
		Vector3 pos = Vector3.Lerp(GetSpawnPos(), GetEndPos(), percent);
		Vector3 diff = GetEndPos() - GetSpawnPos();
		Vector3 dir = diff.normalized;
		Vector3 perpDir = new Vector3(dir.y, -dir.x, dir.z);
		pos += perpDir * Mathf.Sin(sineTimer) * sineAmplitude;
		return pos;
	}

	private Vector3 GetPlayerPosition() {
		float percent = GetCurrentTimerPercent();
		Vector3 pos = Vector3.Lerp(GetCollectedPos(), GetPlayerPos(), percent);
		Vector3 diff = GetPlayerPos() - GetCollectedPos();
		Vector3 dir = diff.normalized;
		Vector3 perpDir = new Vector3(dir.y, -dir.x, dir.z);
		pos += perpDir * Mathf.Sin(sineTimer) * sineAmplitude;
		return pos;
	}

	private Vector3 GetSpawnPos() {
		Vector3 v = EnemyManager.instance.spawnPoint.position;
		v.y = spawnVerticalPos;
		v.z = 0;
		return v;
	}

	private Vector3 GetCollectedPos() {
		Vector3 v = _collectedPos;
		v.y = endVerticalPos;
		v.z = 0;
		return v;
	}

	private Vector3 GetEndPos() {
		Vector3 v = EnemyManager.instance.endPoint.position;
		v.x = v.x + 5;
		v.y = endVerticalPos;
		v.z = 0;
		return v;
	}

	private Vector3 GetPlayerPos() {
		Vector3 v = Player_Controller.instance.transform.position;
		v.x = v.x + 10;
		v.y = endVerticalPos;
		v.z = 0;
		return v;
	}

	private float GetCurrentTimerPercent() {
		if (state == EnemyState.Entering) return GetEnterPercent();
		else if (state == EnemyState.Exiting) return GetExitPercent();
		else if (state == EnemyState.Collected) return GetCollectedPercent();
		else return 0;
	}

	private void UpdateTimers() {
		sineTimer += Time.deltaTime * sineFrequency;
		if (state == EnemyState.Entering) {
			enterTimer += Time.deltaTime;
		}
		else if (state == EnemyState.Exiting) {
			exitTimer += Time.deltaTime;
		}
		else if (state == EnemyState.Collected) {
			collectedTimer += Time.deltaTime;
		}
	}

	private void UpdateState() {
		if (state == EnemyState.Entering) {
			if (GetEnterPercent() >= 1) {
				OnReachedIntersectionPoint();
			}
		}
		else if (state == EnemyState.Exiting) {
			if (GetExitPercent() >= 1) {
				state = EnemyState.Exited;
				OnExited();
			}
		}
		else if (state == EnemyState.Collected) {
			if(GetCollectedPercent() >= 1) {
				Player_Controller.instance.ActivateTrack(_trackType, _trail);
				state = EnemyState.Exited;
				OnExited();
			}
		}
	}

	private void OnExited() {
		ResetAndRecycle();
	}

	private void ResetAndRecycle() {
		_trail.DeactivateTrail();
		_collected = false;
		state = EnemyState.Paused;
		enterTimer = 0;
		exitTimer = 0;
		collectedTimer = 0;
		sineTimer = 0;
		EnemyManager.instance.enemies.Remove(this);
		transform.Recycle();
	}

	private void OnReachedIntersectionPoint() {
		if(!_collected)
			state = EnemyState.Exiting;
		else
		{
			_collectedPos = transform.position;
			state = EnemyState.Collected;
			Player_Controller.instance.GetTrack(_trackType).SetActive();
		}


	}

	private float GetEnterPercent() {
		return Mathf.Clamp01(enterTimer / enterDur);
	}

	private float GetExitPercent() {
		return Mathf.Clamp01(exitTimer / exitDur);
	}

	private float GetCollectedPercent() {
		return Mathf.Clamp01(collectedTimer/ 1);
	}

	public void SetTrackType(AudioManager.TrackTypes trackType) {
		_trail.SetTrackType(trackType);
		_trackType = trackType;
	}
}
