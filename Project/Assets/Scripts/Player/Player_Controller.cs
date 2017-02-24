using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Controller : MonoBehaviour
{
	public static Player_Controller instance;

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

	void Awake() {
		if(!instance)
			instance = this;
	}

	void Start() {
		_bassTrailRenderer.ActivateTrail();
	}

	void Update ()
	{
		bool input = false;
		if (transform.position.y > -4) {
			if (Input.GetKey (KeyCode.S)) {
				input = true;
				_yPos -= .05f;
			}
		}

		if (transform.position.y < 11.5f - (1 * ActiveTrails ())) {
			if (Input.GetKey (KeyCode.W)) {
				input = true;
				_yPos += .05f;
			}
		}

		if (_yPos <= .025f && _yPos >= -.025f) {
			_yPos = 0;
		}
		else if (_yPos > 0) {
			_yPos -= .025f;
		} else if (_yPos < 0) {
			_yPos += .025f;
		}

		_yPos = Mathf.Clamp(_yPos, -.4f, .4f);
		transform.position = new Vector3 (_pointPos, Mathf.Clamp (transform.position.y + _yPos, -7.5f, 12.5f - (1 * ActiveTrails ())), 0);
		_pointPos += 1f;
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

	void LateUpdate ()
	{
		_camera.transform.position = new Vector3 (transform.position.x - 20, 0, -10);
	}
}
