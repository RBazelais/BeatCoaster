using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using WhitDataTypes;

public class GameManager : MonoBehaviour {
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

	void Start() {

	}

	void Update() {

	}
}
