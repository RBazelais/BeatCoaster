using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Trail : MonoBehaviour {

	[SerializeField]
	private AudioManager.TrackTypes _trackType;
	public AudioManager.TrackTypes trackType
	{
		get
		{
			return _trackType;
		}
	}

	[SerializeField]
	private SplineTrailRenderer _splineTrailRenderer;
	public SplineTrailRenderer splineTrailRenderer
	{
		get
		{
			return _splineTrailRenderer;
		}
	}

	private bool _active;
	public bool active
	{
		get
		{
			return _active;
		}
	}

	public void ActivateTrail()
	{
		_active = true;
		_splineTrailRenderer.emit = true;
	}

	public void DeactivateTrail()
	{
		_active = false;
		_splineTrailRenderer.emit = false;
	}
}
