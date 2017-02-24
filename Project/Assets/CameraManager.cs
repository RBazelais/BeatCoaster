﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour {

	[SerializeField]
	private Camera _mainCamera;

	void LateUpdate ()
	{
		if(Player_Controller.instance.playerState == Player_Controller.PlayerState.Active)
			UpdateActivePosition();
		else if (Player_Controller.instance.playerState == Player_Controller.PlayerState.Drop) {
			UpdateDropPosition();
		}
	}

	private void UpdateActivePosition() {
		Vector3 targetPos = new Vector3(Player_Controller.instance.transform.position.x - 18, Player_Controller.instance.yCenter, -10);
		_mainCamera.transform.position = targetPos; // Vector3.Lerp(_mainCamera.transform.position, targetPos, 3 * Time.deltaTime);
	}

	private void UpdateDropPosition() {
		Vector3 targetPos = new Vector3 (Player_Controller.instance.transform.position.x, Player_Controller.instance.transform.position.y - 5, -10);
		_mainCamera.transform.position = targetPos; // Vector3.Lerp(_mainCamera.transform.position, targetPos, 3 * Time.deltaTime);
	}
}
