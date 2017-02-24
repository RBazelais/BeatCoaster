using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum OpponentState {
	Paused,
	Entering,
	Exiting,
	Exited
}

public class Trail_Opponent : MonoBehaviour {
	private float enterTimer = 0;
	private float exitTimer = 0;
	private OpponentState state = OpponentState.Paused;
	private float enterDur;
	private float exitDur;
	private float endVerticalPos;
	private float spawnVerticalPos;
	private float sineTimer = 0;
	private float sineAmplitude;
	private float sineFrequency;

	[SerializeField]
	private SplineTrailRenderer _splineTrailRenderer, _bgSplineTrailRenderer;

	public void Activate() {
		sineAmplitude = Random.Range(1f, 3f);
		sineFrequency = Random.Range(3f, 10f);

		_splineTrailRenderer.gameObject.SetActive(true);
		_bgSplineTrailRenderer.gameObject.SetActive(true);

		_splineTrailRenderer.emit = true;
		_bgSplineTrailRenderer.emit = true;

		spawnVerticalPos = transform.position.y;
		endVerticalPos = GameManager.instance.enemyVerticalRange.GetRandom();

		enterDur = OpponentManager.instance.enemyEnterDurationRange.GetRandom();
		exitDur = OpponentManager.instance.enemyExitDurationRange.GetRandom();

		state = OpponentState.Entering;
	}

	private void Update() {
		if (state == OpponentState.Paused) return;
		UpdateTimers();
		UpdateState();
		UpdatePosition();
	}

	private void UpdatePosition() {
		if (state == OpponentState.Entering) {// || state == OpponentState.Exiting) {
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
		Vector3 v = OpponentManager.instance.spawnPoint.position;
		v.y = spawnVerticalPos;
		v.z = 0;
		return v;
	}

	private Vector3 GetEndPos() {
		Vector3 v = OpponentManager.instance.endPoint.position;
		v.y = endVerticalPos;
		v.z = 0;
		return v;
	}

	private float GetCurrentTimerPercent() {
		if (state == OpponentState.Entering) return GetEnterPercent();
		else if (state == OpponentState.Exiting) return GetExitPercent();
		else return 0;
	}

	private void UpdateTimers() {
		sineTimer += Time.deltaTime * sineFrequency;
		if (state == OpponentState.Entering) {
			enterTimer += Time.deltaTime;
		}
		else if (state == OpponentState.Exiting) {
			exitTimer += Time.deltaTime;
		}
	}

	private void UpdateState() {
		if (state == OpponentState.Entering) {
			if (GetEnterPercent() >= 1) {
				state = OpponentState.Exiting;
				OnReachedIntersectionPoint();
			}
		}
		else if (state == OpponentState.Exiting) {
			if (GetExitPercent() >= 1) {
				state = OpponentState.Exited;
				OnExited();
			}
		}
	}

	private void OnExited() {
		ResetAndRecycle();
	}

	private void ResetAndRecycle() {
		_bgSplineTrailRenderer.emit = false;
		_splineTrailRenderer.emit = false;
		state = OpponentState.Paused;
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
}
