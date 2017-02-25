using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraManager : MonoBehaviour {

	[SerializeField]
	private Camera _mainCamera;

	[SerializeField]
	private Transform _cameraParent;

	private float _shakeIntensity = 1f;

	void Start() {
		AudioManager.instance.BeatOnUpdate += ShakeCamera;
	}

	void LateUpdate ()
	{
		if(Player_Controller.instance.playerState == Player_Controller.PlayerState.Active || Player_Controller.instance.playerState == Player_Controller.PlayerState.Idle)
			UpdateActivePosition();
		else if (Player_Controller.instance.playerState == Player_Controller.PlayerState.Drop) {
			UpdateDropPosition();
		}
	}

	private void UpdateActivePosition() {
		Vector3 targetPos = new Vector3(Player_Controller.instance.transform.position.x - 19, Player_Controller.instance.yCenter, -10);
		_cameraParent.position = targetPos; // Vector3.Lerp(_mainCamera.transform.position, targetPos, 3 * Time.deltaTime);
	}

	private void UpdateDropPosition() {
		Vector3 targetPos = new Vector3 (Player_Controller.instance.transform.position.x - 20, Player_Controller.instance.transform.position.y + 17, -10);
		_cameraParent.position = targetPos; // Vector3.Lerp(_mainCamera.transform.position, targetPos, 3 * Time.deltaTime);
	}

	void ShakeCamera() {
		if(Player_Controller.instance.playerState == Player_Controller.PlayerState.Idle){
			_mainCamera.DOShakePosition(.15f, Mathf.Clamp(_shakeIntensity, 1, 2f), 10);
			_shakeIntensity += .05f;
		}
		else if (Player_Controller.instance.playerState == Player_Controller.PlayerState.Drop) {
			_shakeIntensity = 1f;
			_mainCamera.DOShakePosition(.15f, _shakeIntensity, 90);
		}
		else {
			_shakeIntensity = 1f;
		}
	}
}
