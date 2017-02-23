using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

interface IScreenCallbacks {
	void OnInitialize();
	void OnCleanUp();
	void OnScreenWillLoseFocus();
	void OnScreenGainedFocus();
	void OnUpdate();
}
