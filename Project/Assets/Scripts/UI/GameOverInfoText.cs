using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameOverListenersText : MonoBehaviour {
	[SerializeField] private TextMeshProUGUI text;

	private void OnEnable() {
		UpdateText();
	}

	private void UpdateText() {
		text.text = GetString();
	}

	private string GetString() {
		return GameManager.instance.droppedListeners.ToString("N0") + " listeners";
	}
}
