using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GameManager : MonoBehaviour {

	[SerializeField]
	private Trail_Opponent _opponentPrefab;


	[SerializeField]
	private Transform _spawnPoint;

	private bool _spawn = false, _readyToSpawn = false;

	void Start() {
		_opponentPrefab.CreatePool(100);
		AudioManager.instance.BeatExact += SpawnOpponent;
	}

	void SpawnOpponent() {
		_readyToSpawn = true;
	}

	void Update() {

		if(Input.GetKeyDown(KeyCode.Space))
		{
			_spawn = true;
		}

		if(_readyToSpawn && _spawn) {
			_readyToSpawn = false;
			_spawn = false;
			Trail_Opponent opponent = _opponentPrefab.Spawn();
			opponent.Activate();
			opponent.transform.position = new Vector3(_spawnPoint.transform.position.x, Random.Range(-12.5f,-12.5f), 0);
			opponent.transform.DOMove(new Vector3(Player_Controller.instance.transform.position.x + 50, Player_Controller.instance.transform.position.y, 0), 30f).SetSpeedBased(true).OnComplete(() => opponent.Recycle());
		}
	}
}
