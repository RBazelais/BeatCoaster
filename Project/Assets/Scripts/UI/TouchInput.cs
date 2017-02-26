using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TouchInput : MonoBehaviour {
	private static TouchInput _instance;
	public static TouchInput instance {
		get {
			if (_instance == null) _instance = GameObject.FindObjectOfType<TouchInput>();
			return _instance;
		}
	}

	[SerializeField] private GameObject panel;

	public enum WaveButtonState {
		None,
		Up,
		Down
	}

	public Action SignalLeftButtonTapped;
	public Action SignalUp;
	public Action SignalDown;

	private WaveButtonState state = WaveButtonState.None;

	private bool upButton = false;
	private bool downButton = false;
	private bool touchIsDown = false;

	public void EnableInput() {
		panel.gameObject.SetActive(true);
	}

	public void DisableInput() {
		panel.gameObject.SetActive(false);
	}

	public void OnLeftButtonTapped() {
		if (SignalLeftButtonTapped != null) SignalLeftButtonTapped();
	}

	public void OnUpEnter() {
		if (touchIsDown) upButton = true;
	}

	public void OnUpExit() {
		upButton = false;
	}

	public void OnDownEnter() {
		if (touchIsDown) downButton = true;
	}

	public void OnDownExit() {
		downButton = false;
	}

	public void OnUpButton() {
		upButton = true;
		touchIsDown = true;
	}

	public void OnDownButton() {
		downButton = true;
		touchIsDown = true;
	}

	public void OnTouchUp() {
		upButton = false;
		downButton = false;
		touchIsDown = false;
	}

	private void Update() {
		ReconcileWaveButtons();
		SendSignals();
	}

	private void ReconcileWaveButtons() {
		if (!upButton && !downButton) state = WaveButtonState.None;
		else if (upButton && downButton) state = UnityEngine.Random.value < 0.5f ? WaveButtonState.Up : WaveButtonState.Down;
		else {
			if (upButton) state = WaveButtonState.Up;
			else if (downButton) state = WaveButtonState.Down;
		}
	}

	private void SendSignals() {
		if (state == WaveButtonState.Down) {
			if (SignalDown != null) SignalDown();
		}
		else if (state == WaveButtonState.Up) {
			if (SignalUp != null) SignalUp();
		}
	}
}
