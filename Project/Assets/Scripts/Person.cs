using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WhitDataTypes;
using DG.Tweening;

public class Person : MonoBehaviour {
	public Player_Trail trail {get; private set;}

	private SpriteRenderer sprite;

	private float wobbleOffset;
	private float wobbleVal;

	private int segmentDistance;

	private void Awake() {
		segmentDistance = Player_Controller.instance.segmentDistanceRange.GetRandom();
		Debug.Log(segmentDistance);
		wobbleOffset = UnityEngine.Random.Range(0, 360);
		sprite = GetComponentInChildren<SpriteRenderer>();
	}

	void LateUpdate () 
	{
		if (trail) 
		{
			UpdateWobble();
			UpdatePosition();
		}
	}

	private void UpdatePosition() 
	{
		int segment = Mathf.Max(trail.splineTrailRenderer.spline.NbSegments - segmentDistance, 0);
		float distFromStart = trail.splineTrailRenderer.spline.GetSegmentDistanceFromStart(segment);
		distFromStart += wobbleVal;
		Vector3 endPosition = trail.splineTrailRenderer.spline.FindPositionFromDistance(distFromStart);
		transform.position = endPosition;
	}

	public void SetTrail(Player_Trail trail)
	{
		this.trail = trail;
	}

	private void UpdateWobble() {
		wobbleVal = Player_Controller.instance.wobbleIntensity * Mathf.Sin(Time.time * Player_Controller.instance.wobbleSpeed + wobbleOffset);
	}
		
	public void OnBeat() {
		float jumpAmt = UnityEngine.Random.Range(0.4f, 0.6f);
		float dur = 60.0f / (float)AudioManager.instance.bpm;
		float buffer = dur * 0.1f;
		dur -= buffer;

		Sequence s = DOTween.Sequence();
		s.AppendInterval(buffer);
		s.Append(sprite.transform
			.DOLocalJump(Vector3.zero, jumpAmt, 1, dur)
			.SetDelay(buffer));
		s.Join(sprite.transform.DOPunchScale(new Vector3(0.2f, 0.2f, 0.2f), dur));

		
	}
}
