using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using WhitDataTypes;

public class OpponentManager : MonoBehaviour {
	private static OpponentManager _instance;
	public static OpponentManager instance {
		get {
			if (_instance == null) {
				_instance = GameObject.FindObjectOfType<OpponentManager>();
			}
			return _instance;
		}
	}

	public FloatRange enemyEnterDurationRange = new FloatRange(2, 4);
	public FloatRange enemyExitDurationRange = new FloatRange(0.5f, 1.5f);
	public Transform spawnPoint;
	public Transform endPoint;

	[SerializeField] private Trail_Opponent opponentPrefab;

	private bool songPlaying = true;

	private void Start() {
		opponentPrefab.CreatePool(10);
		AudioManager.instance.BeatExact += OnBeat;
	}

	private void OnBeat() {
		if (!songPlaying) songPlaying = true;
	}

	private void Update() {
		if (Input.GetKeyDown(KeyCode.O)) {
			SpawnOpponent();
		}
	}

	private void SpawnOpponent() {
		Trail_Opponent opponent = opponentPrefab.Spawn();
		opponent.transform.position = new Vector3(spawnPoint.position.x, GameManager.instance.enemyVerticalRange.GetRandom(), 0);
//		opponent.transform.SetParent(spawnPoint);
		opponent.Activate();
	}
}
