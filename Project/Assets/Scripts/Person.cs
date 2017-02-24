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
		sprite.transform.DOLocalJump(Vector3.zero, 5, 1, 0.15f);
	}
}
