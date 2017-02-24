using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyState {
	Paused,
	Entering,
	Exiting,
	Exited
}

public class Enemy : MonoBehaviour {
	private float enterTimer = 0;
	private float exitTimer = 0;
	private EnemyState state = EnemyState.Paused;
	private float enterDur;
	private float exitDur;
	private float endVerticalPos;
	private float spawnVerticalPos;
	private float sineTimer = 0;
	private float sineAmplitude;
	private float sineFrequency;

	[SerializeField] private TrackTrail trail;

	public void Activate() {
		sineAmplitude = Random.Range(1f, 3f);
		sineFrequency = Random.Range(3f, 10f);

		trail.ActivateTrail();

		spawnVerticalPos = transform.position.y;
		endVerticalPos = GameManager.instance.enemyVerticalRange.GetRandom();

		enterDur = EnemyManager.instance.enemyEnterDurationRange.GetRandom();
		exitDur = EnemyManager.instance.enemyExitDurationRange.GetRandom();

		state = EnemyState.Entering;
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

	private Vector3 GetSpawnPos() {
		Vector3 v = EnemyManager.instance.spawnPoint.position;
		v.y = spawnVerticalPos;
		v.z = 0;
		return v;
	}

	private Vector3 GetEndPos() {
		Vector3 v = EnemyManager.instance.endPoint.position;
		v.y = endVerticalPos;
		v.z = 0;
		return v;
	}

	private float GetCurrentTimerPercent() {
		if (state == EnemyState.Entering) return GetEnterPercent();
		else if (state == EnemyState.Exiting) return GetExitPercent();
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
	}

	private void UpdateState() {
		if (state == EnemyState.Entering) {
			if (GetEnterPercent() >= 1) {
				state = EnemyState.Exiting;
				OnReachedIntersectionPoint();
			}
		}
		else if (state == EnemyState.Exiting) {
			if (GetExitPercent() >= 1) {
				state = EnemyState.Exited;
				OnExited();
			}
		}
	}

	private void OnExited() {
		ResetAndRecycle();
	}

	private void ResetAndRecycle() {
		trail.DeactivateTrail();
		state = EnemyState.Paused;
		enterTimer = 0;
		exitTimer = 0;
		sineTimer = 0;
		transform.Recycle();
	}

	private void OnReachedIntersectionPoint() {
		
	}

	private float GetEnterPercent() {
		return Mathf.Clamp01(enterTimer / enterDur);
	}

	private float GetExitPercent() {
		return Mathf.Clamp01(exitTimer / exitDur);
	}

	public void SetTrackType(AudioManager.TrackTypes trackType) {
		trail.SetTrackType(trackType);
	}
}
