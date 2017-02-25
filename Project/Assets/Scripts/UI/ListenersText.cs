using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ListenersText : MonoBehaviour {
	[SerializeField] private TextMeshProUGUI text;

	private void OnEnable() {
		PersonManager.instance.SignalPersonAddedToCollection += OnListenerAdded;
		PersonManager.instance.SignalPersonRemovedFromCollection += OnListenerRemoved;
		GameManager.instance.SignalPlayingStateEnter += OnPlayingStateEnter;
	}

	private void OnDisable() {
		if (PersonManager.instance != null) {
			PersonManager.instance.SignalPersonAddedToCollection -= OnListenerAdded;
			PersonManager.instance.SignalPersonRemovedFromCollection -= OnListenerRemoved;
		}
		if (GameManager.instance != null) {
			GameManager.instance.SignalPlayingStateEnter -= OnPlayingStateEnter;
		}
	}

	private void OnPlayingStateEnter() {
		UpdateText();
	}

	private void OnListenerAdded() {
		UpdateText();
	}

	private void OnListenerRemoved() {
		UpdateText();

	}

	private void UpdateText() {
		text.text = GetString();
	}

	private string GetString() {
		return GameManager.instance.droppedListeners.ToString("N0") + " checked-in listeners";
	}
}
