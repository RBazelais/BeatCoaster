using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trail_Opponent : MonoBehaviour {

	[SerializeField]
	private SplineTrailRenderer _splineTrailRenderer, _bgSplineTrailRenderer;

	public void Activate() {
		_splineTrailRenderer.gameObject.SetActive(true);
		_bgSplineTrailRenderer.gameObject.SetActive(true);

		_splineTrailRenderer.emit = true;
		_bgSplineTrailRenderer.emit = true;
	}
}
