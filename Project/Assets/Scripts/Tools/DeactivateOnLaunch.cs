using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class DeactivateOnLaunch : MonoBehaviour {
	public void Deactivate() {
		gameObject.SetActive(false);
	}
}
