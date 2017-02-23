using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class SharedPanel : MonoBehaviour {
	public Action SignalActivated;
	public Action SignalWillDeactivate;
	public Action<ScreenBase> SignalInitialized;
	public Action<ScreenBase> SignalScreenGainedFocus;
	public Action<ScreenBase> SignalScreenWillLoseFocus;

	public void Activate() {
		gameObject.SetActive(true);
		if (SignalActivated != null) SignalActivated();
	}

	public void Deactivate() {
		if (SignalWillDeactivate != null) SignalWillDeactivate();
		gameObject.SetActive(false);
	}

	public virtual void OnScreenUpdate() {

	}

	public virtual void Initialize(ScreenBase screen) {
		if (SignalInitialized != null) SignalInitialized(screen);
	}

	public virtual void OnScreenGainedFocus(ScreenBase screen) {
		if (SignalScreenGainedFocus != null) SignalScreenGainedFocus(screen);
	}

	public virtual void OnScreenWillLoseFocus(ScreenBase screen) {
		if (SignalScreenWillLoseFocus != null) SignalScreenWillLoseFocus(screen);
	}

	public virtual void CleanUp() {

	}
}
