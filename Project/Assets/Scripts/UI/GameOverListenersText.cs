using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameOverInfoText : MonoBehaviour {
	[SerializeField] private TextMeshProUGUI text;
	private string infoString;
	private string venueName;
	private string venueLocation;
	private string devicePromptText;

	private void OnEnable() {
		if (SystemInfo.deviceType == DeviceType.Handheld) {
			devicePromptText = "Tap to continue";
		} else {
			devicePromptText = "Press spacebar to continue";
		}
		UpdateText ();
	}

	private void UpdateText() {
		text.text = GetString();
	}

	private string GetString() {

		if (GameManager.instance.droppedListeners == 0) {
			infoString = "That's not enough to fill anything. Yikes.\n\n";
		} else if (GameManager.instance.droppedListeners <= 300) {
			venueName = "the historic, now-closed CBGB";
			venueLocation = "New York, New York";
		} else if (GameManager.instance.droppedListeners <= 600) {
			venueName = "the Music Hall of Williamsburg";
			venueLocation = "New York, New York";
		} else if (GameManager.instance.droppedListeners <= 1000) {
			venueName = "the Great American Music Hall";
			venueLocation = "San Francisco, California";
		} else if (GameManager.instance.droppedListeners <= 2000) {
			venueName = "the Paramount theatre";
			venueLocation = "Denver, Colorado";
		} else if (GameManager.instance.droppedListeners <= 2500) {
			venueName = "Webster Hall";
			venueLocation = "New York, New York";
		} else if (GameManager.instance.droppedListeners <= 5000) {
			venueName = "Saratoga Performing Arts Center";
			venueLocation = "Saratoga Springs, New York";
		} else if (GameManager.instance.droppedListeners <= 10000) {
			venueName = "Ford Idaho Center";
			venueLocation = "Nampa, Idaho";
		} else if (GameManager.instance.droppedListeners <= 20000) {
			venueName = "Madison Square Garden";
			venueLocation = "New York, New York";
		} else if (GameManager.instance.droppedListeners <= 40000) {
			venueName = "Angel Stadium";
			venueLocation = "Anaheim, California";
		} else if (GameManager.instance.droppedListeners > 40000) {
			venueName = "Levi's Stadium";
			venueLocation = "Santa Clara, California";
		}

		if (GameManager.instance.droppedListeners != 0) {
			infoString = "That's enough to fill " + venueName + " in " + venueLocation + ".\n\n";
		}

		infoString += devicePromptText;

		return infoString;
	}
}
