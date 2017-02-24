using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour {

	[SerializeField]
	private Camera _mainCamera;

	void LateUpdate ()
	{
		if(Player_Controller.instance.playerState == Player_Controller.PlayerState.Active)
		_mainCamera.transform.position = new Vector3 (Player_Controller.instance.transform.position.x - 20, 0, -10);
		else if (Player_Controller.instance.playerState == Player_Controller.PlayerState.Drop) {
			_mainCamera.transform.position = new Vector3 (Player_Controller.instance.transform.position.x - 20, Player_Controller.instance.transform.position.y, -10);
		}
	}
}
