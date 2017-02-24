using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class ScreenBase : MonoBehaviour {
	public ScreenManagerBase screenManager;
	public SharedPanel[] sharedPanels;
	public SharedPanelInsert[] sharedPanelInserts;
	public bool useSharedPanelsOfScreenUnderneath = false;

	public Rect viewportRect {get {return GetViewportRect();}}

	public virtual void OnUpdate() {
		screenManager.OnUpdate();

		OnScreenUpdateSharedPanels();
		OnScreenUpdateSharedPanelInserts();
	}

	public virtual void OnPush() {
		gameObject.SetActive(true);

		screenManager.OnInitialize();
		screenManager.OnScreenGainedFocus();

		InitializeSharedPanels();
		InitializeSharedPanelInserts();
		ActivateSharedPanels();
		ActivateSharedPanelInserts();
	}

	public virtual void OnPop() {
		screenManager.OnScreenWillLoseFocus();
		screenManager.OnCleanUp();
		CleanUpSharedPanelInserts();
		CleanUpSharedPanels();
		DeactivateSharedPanels();
		DeactivateSharedPanelInserts();
		gameObject.SetActive(false);
	}

	public bool ScreenPointIsWithinScreenRect(Vector2 screenPoint) {
		Vector2 touchViewportPoint = Camera.main.ScreenToViewportPoint(screenPoint);
		return viewportRect.Contains(touchViewportPoint);
	}

	private Rect GetViewportRect() {
		RectTransform rt = transform as RectTransform;

		Vector3[] corners = new Vector3[4];
		rt.GetWorldCorners(corners);

		Vector2 v0 = Camera.main.WorldToViewportPoint(corners[0]);
		Vector2 v1 = Camera.main.WorldToViewportPoint(corners[1]);
		Vector2 v3 = Camera.main.WorldToViewportPoint(corners[3]);

		Vector2 vOrigin = v0;
		Vector2 vSize = new Vector2(v3.x - v0.x, v1.y - v0.y);

		Rect viewportRect = new Rect(vOrigin, vSize);
		return viewportRect;
	}

	public virtual void OnScreenWillPushAbove() {
		screenManager.OnScreenWillLoseFocus();
		NotifySharedPanelsOfLoseFocus();
		DeactivateSharedPanels();
		DeactivateSharedPanelInserts();
	}

	public virtual void OnScreenAbovePopped() {
		screenManager.OnScreenGainedFocus();
		NotifySharedPanelsOfGainFocus();
		ActivateSharedPanels();
		ActivateSharedPanelInserts();
	}

	private void NotifySharedPanelsOfLoseFocus() {
		for (int i = 0; i < sharedPanels.Length; i++) {
			sharedPanels[i].OnScreenWillLoseFocus(this);
		}
	}

	private void NotifySharedPanelsOfGainFocus() {
		for (int i = 0; i < sharedPanels.Length; i++) {
			sharedPanels[i].OnScreenGainedFocus(this);
		}
	}

	private void OnScreenUpdateSharedPanels() {
		foreach (SharedPanel obj in sharedPanels) {
			obj.OnScreenUpdate();
		}
	}

	private void InitializeSharedPanels() {
		if (useSharedPanelsOfScreenUnderneath) {
			if (ScreensManager.instance.ScreenExistsUnderCurrentScreen()) {
				ScreenBase underScreen = ScreensManager.instance.GetScreenUnderCurrentScreen();
				sharedPanels = underScreen.sharedPanels;
			}
		}

		foreach (SharedPanel obj in sharedPanels) {
			obj.Initialize(this);
		}
	}

	private void InitializeSharedPanelInserts() {
		foreach (SharedPanelInsert elements in sharedPanelInserts) {
			elements.Initialize();
		}
	}

	private void CleanUpSharedPanels() {
		foreach (SharedPanel obj in sharedPanels) {
			obj.CleanUp();
		}
	}

	private void CleanUpSharedPanelInserts() {
		foreach (SharedPanelInsert elements in sharedPanelInserts) {
			elements.CleanUp();
		}
	}

	private void OnScreenUpdateSharedPanelInserts() {
		foreach (SharedPanelInsert elements in sharedPanelInserts) {
			elements.OnScreenUpdate();
		}
	}

	private void ActivateSharedPanels() {
		foreach (SharedPanel obj in sharedPanels) {
			obj.Activate();
		}
	}

	private void DeactivateSharedPanels() {
		foreach (SharedPanel obj in sharedPanels) {
			obj.Deactivate();
		}
	}

	private void ActivateSharedPanelInserts() {
		foreach (SharedPanelInsert elements in sharedPanelInserts) {
			elements.Activate();
		}
	}

	private void DeactivateSharedPanelInserts() {
		foreach (SharedPanelInsert elements in sharedPanelInserts) {
			elements.Deactivate();
		}
	}
}