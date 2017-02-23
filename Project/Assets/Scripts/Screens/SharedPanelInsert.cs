using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class SharedPanelInsert : MonoBehaviour {
	public RectTransform parentWhenInactive;
	public RectTransform sharedPanel;

	public void Activate() {
		SetAsParentOf(sharedPanel);
		gameObject.SetActive(true);
		OnActivate();
	}

	public void Deactivate() {
		OnWillDeactivate();
		SetAsParentOf(parentWhenInactive);
		gameObject.SetActive(false);
	}

	private void SetAsParentOf(RectTransform rt) {
		transform.SetParent(rt);
		transform.localScale = Vector3.one;
		transform.localPosition = Vector3.zero;
	}

	public virtual void Initialize() {

	}

	public virtual void CleanUp() {

	}

	public virtual void OnScreenUpdate() {

	}

	protected virtual void OnActivate() {

	}

	protected virtual void OnWillDeactivate() {

	}
}
