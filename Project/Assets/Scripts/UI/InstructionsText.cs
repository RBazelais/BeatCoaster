using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InstructionsText : MonoBehaviour {
	[SerializeField] private TextMeshProUGUI text;

	private void OnEnable() {
		if (SystemInfo.deviceType == DeviceType.Handheld) {
			text.text = "Grow your listener count by tapping on the left side. Tap the right to move the blue track up and down. Tap the screen to check them in when you have 5 colors. Check in the most listeners before running out of energy.\n\nTap to play";
		} else {
			text.text = "Grow your listener count by using shift or clicking the left side. Use arrows, W & S, or tap the right to move the blue track up and down. Press spacebar to check them in when you have 5 colors. Check in the most listeners before running out of energy.\n\nClick or press spacebar to play";
		}
	}
}
