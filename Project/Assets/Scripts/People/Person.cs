using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WhitDataTypes;
using DG.Tweening;

public class Person : MonoBehaviour {
	public enum PersonMoveType {
		Follow,
		Transfer
	}

	public TrackTrail trail {get; private set;}

	private SpriteRenderer sprite;

	private float wobbleOffset;
	private float wobbleVal;
	private float transferTimer = 0;
	private float transferDur = 2f;
	private int rotateDirSign = 1;
	private bool readyForNewBeat = true;
	private float decayVal = 1;

	[SerializeField] private Animator animator;

	private int segmentDistance;
	private PersonMoveType moveType = PersonMoveType.Follow;

	private void Awake() {
		transferDur += Random.Range(-0.5f, 0.5f);
		rotateDirSign = WhitTools.GetRandomSign();
		segmentDistance = PersonManager.instance.segmentDistanceRange.GetRandom();
		wobbleOffset = UnityEngine.Random.Range(0, 360);
		sprite = GetComponentInChildren<SpriteRenderer>();
	}

	public void Reset() {
		transferTimer = 0;
		moveType = PersonMoveType.Follow;
	}

	void LateUpdate () 
	{
		if (trail) 
		{
			UpdateWobble();
			UpdatePosition();

			animator.SetFloat("Track Decay", decayVal);
		}
	}



	private void UpdatePosition() 
	{
		int segment = Mathf.Max(trail.splineTrailRenderer.spline.NbSegments - segmentDistance, 0);
		float distFromStart = trail.splineTrailRenderer.spline.GetSegmentDistanceFromStart(segment);
		distFromStart += wobbleVal;
		Vector3 targetPos = trail.splineTrailRenderer.spline.FindPositionFromDistance(distFromStart);

		if (moveType == PersonMoveType.Follow) {
			transform.position = targetPos;
		}
		else if (moveType == PersonMoveType.Transfer) {
			transferTimer += Time.deltaTime;
			float percent = Mathf.Clamp01(transferTimer / transferDur);
			Vector3 lerpedPos = Vector3.Lerp(transform.position, targetPos, percent);
			transform.position = lerpedPos;

			if (percent >= 0.3f) {
				moveType = PersonMoveType.Follow;
				animator.SetBool("isJumping", false);
				OnJoinedCollectedPeople();
			}
		}
	}

	private void OnJoinedCollectedPeople() {
		PersonManager.instance.OnPersonJoinedCollectedPeople(this);
	}

	public void SetTrail(TrackTrail trail, PersonMoveType moveType)
	{
		this.trail = trail;
		this.moveType = moveType;

		if (moveType == PersonMoveType.Transfer) {
			animator.SetBool("isJumping", true);
			transferTimer = 0;
		}
	}

	public void OnTrackDecayChange(float decay) {
		decayVal = decay;
	}

	private void UpdateWobble() {
		wobbleVal = PersonManager.instance.wobbleIntensity * Mathf.Sin(Time.time * PersonManager.instance.wobbleSpeed + wobbleOffset);
	}
		
	public void OnBeat() {
		if (!readyForNewBeat) return;

		readyForNewBeat = false;
		float jumpAmt = UnityEngine.Random.Range(0.5f, 0.7f);

		float dur = 60.0f / (float)AudioManager.instance.bpm - 0.1f;
		float buffer = dur * 0.1f;
		dur -= buffer;

		Vector3 punchRotation = new Vector3(0, 0, Random.Range(10, 40) * rotateDirSign);

		Sequence s = DOTween.Sequence();
		s.AppendInterval(buffer);
		s.Append(sprite.transform
			.DOLocalJump(Vector3.zero, jumpAmt, 1, dur)
			.SetDelay(buffer));
		s.Join(sprite.transform.DOPunchScale(new Vector3(0.2f, 0.2f, 0.2f), dur));
		s.Join(sprite.transform.DOPunchRotation(punchRotation, dur, 5, 0.5f));
		s.OnComplete(()=>{
			rotateDirSign *= -1;
			readyForNewBeat = true;
		});
		s.Play();
	}
}
