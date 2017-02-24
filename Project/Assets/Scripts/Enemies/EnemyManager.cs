using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using WhitDataTypes;

public class EnemyManager : MonoBehaviour {
	private static EnemyManager _instance;
	public static EnemyManager instance {
		get {
			if (_instance == null) {
				_instance = GameObject.FindObjectOfType<EnemyManager>();
			}
			return _instance;
		}
	}

	public FloatRange enemyEnterDurationRange = new FloatRange(2, 4);
	public FloatRange enemyExitDurationRange = new FloatRange(0.5f, 1.5f);
	public Transform spawnPoint;
	public Transform endPoint;

	[SerializeField] private Enemy enemyPrefab;

	private bool songPlaying = true;

	private void Start() {
		enemyPrefab.CreatePool(10);
		AudioManager.instance.BeatExact += OnBeat;
	}

	private void OnBeat() {
		if (!songPlaying) songPlaying = true;
	}

	private void Update() {
		if (Input.GetKeyDown(KeyCode.O)) {
			SpawnEnemy();
		}
	}

	private void SpawnEnemy() {
		Enemy enemy = enemyPrefab.Spawn();
		enemy.transform.position = new Vector3(spawnPoint.position.x,Player_Controller.instance.yCenter + GameManager.instance.enemyVerticalRange.GetRandom(), 0);
		AudioManager.TrackTypes trackType = (AudioManager.TrackTypes)Random.Range(0, 5);
		enemy.SetTrackType(trackType);
		enemy.Activate();
	}
}
