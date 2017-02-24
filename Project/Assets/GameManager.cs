using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using WhitDataTypes;

public class GameManager : MonoBehaviour {
	public enum GameState {
		Title,
		Playing,
		GameOver
	}

	public static Color DrumColor = new Color(1, 0, 0);
	public static Color PizzColor = new Color(254f/255f, 118f/255f, 15f/255f);
	public static Color KeysColor = new Color(247f/255f, 205f/255f, 0f/255f);
	public static Color ClavColor = new Color(0f/255f, 137f/255f, 209f/255f);
	public static Color BassColor = new Color(0f/255f, 196f/255f, 71f/255f);

	public static Color DrumShadowColor {get {return GetShadowColorFromColor(DrumColor);}}
	public static Color PizzShadowColor {get {return GetShadowColorFromColor(PizzColor);}}
	public static Color KeysShadowColor {get {return GetShadowColorFromColor(KeysColor);}}
	public static Color ClavShadowColor {get {return GetShadowColorFromColor(ClavColor);}}
	public static Color BassShadowColor {get {return GetShadowColorFromColor(BassColor);}}

	public static Color GetColorForTrackType(AudioManager.TrackTypes trackType) {
		switch (trackType) {
			case AudioManager.TrackTypes.Drums:
				return DrumColor;
			case AudioManager.TrackTypes.Pizz:
				return PizzColor;
			case AudioManager.TrackTypes.Keys:
				return KeysColor;
			case AudioManager.TrackTypes.Clav:
				return ClavColor;
			case AudioManager.TrackTypes.Bass:
				return BassColor;
			default:
				Debug.LogError("invalid track type: " + trackType.ToString());
				return Color.magenta;
		}
	}

	public static Color GetShadowColorForTrackType(AudioManager.TrackTypes trackType) {
		switch (trackType) {
			case AudioManager.TrackTypes.Drums:
				return DrumShadowColor;
			case AudioManager.TrackTypes.Pizz:
				return PizzShadowColor;
			case AudioManager.TrackTypes.Keys:
				return KeysShadowColor;
			case AudioManager.TrackTypes.Clav:
				return ClavShadowColor;
			case AudioManager.TrackTypes.Bass:
				return BassShadowColor;
			default:
				Debug.LogError("invalid track type: " + trackType.ToString());
				return Color.magenta;
		}
	}

	public static Color GetShadowColorFromColor(Color color) {
		HSVColor hsvColor = WadeUtils.RGBToHSV(color);
		hsvColor.v -= 0.26f;
		Color c = WadeUtils.HSVToRGB(hsvColor);
		return c;
	}

	private static GameManager _instance;
	public static GameManager instance {
		get {
			if (_instance == null) {
				_instance = GameObject.FindObjectOfType<GameManager>();
			}
			return _instance;
		}
	}

	public FloatRange enemyVerticalRange = new FloatRange(-12.5f, 12.5f);

	public GameState state {get; private set;}

	public void SetState(GameState state) {
		this.state = state;
	}

	void Start() {

	}

	void Update() {

	}
}
