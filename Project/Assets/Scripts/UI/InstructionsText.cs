using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InstructionsText : MonoBehaviour {
	[SerializeField] private TextMeshProUGUI text;

	private void OnEnable() {
		if (SystemInfo.deviceType == DeviceType.Handheld) {
			text.text = "Swipe the right side up and down to move the blue track. Grow your listener count by tapping on the left side. Tap the screen to check them in when you have 5 colors. Check in most listeners before running out of energy.\n\nTap to play";
		} else {
			text.text = "Grow your listener count using arrows/W+S & shift. Press spacebar to check them in when you have 5 colors. Check in most listeners before running out of energy.\n\nPress spacebar to play";
		}
	}
}
