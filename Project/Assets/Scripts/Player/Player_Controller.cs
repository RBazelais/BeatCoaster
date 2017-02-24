using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Controller : MonoBehaviour
{
	private float _pointPos = 0;

	[SerializeField]
	private Camera _camera;

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

	private float _yPos = 0, _lastYPos;



	void Update ()
	{
		bool input = false;
		if (transform.position.y > -4) {
			if (Input.GetKey (KeyCode.S)) {
				input = true;
				_yPos -= .05f;
			}
		}

		if (transform.position.y < 4) {
			if (Input.GetKey (KeyCode.W)) {
				input = true;
				_yPos += .05f;
			}
		}

		if (!input) { 
			if (_yPos > 0) {
				_yPos -= .025f;
			} else if (_yPos < 0) {
				_yPos += .025f;
			} else if (_yPos <= .025f && _yPos >= -.025f) {
				_yPos = 0;
			}
		}

		transform.position = new Vector3 (_pointPos, Mathf.Clamp(transform.position.y + _yPos, -7.5f, 7.5f), 0);
		_pointPos += 1f;
	}

	void LateUpdate ()
	{
		_camera.transform.position = new Vector3 (transform.position.x - 20, 0, -10);
	}
}
