using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GameManager : MonoBehaviour {

	[SerializeField]
	private Trail_Opponent _opponentPrefab;


	[SerializeField]
	private Transform _spawnPoint;

	private bool _spawn = false;

	void Awake() {
		_opponentPrefab.CreatePool(100);
		AudioManager.instance.Beat += SpawnOpponent;
	}

	void Update() {
		if(Input.GetKeyDown(KeyCode.Space))
		{
			_spawn = true;
		}
	}

	void SpawnOpponent() {
		Debug.Log("beat");
		if(_spawn) {
			_spawn = false;
			var opponent = _opponentPrefab.Spawn();
			opponent.transform.position = new Vector3(_spawnPoint.transform.position.x, Random.Range(-12.5f,-12.5f), 0);
			opponent.transform.DOMove(Player_Controller.instance.transform.position, 25f).SetSpeedBased(true);
		}
	}
}
