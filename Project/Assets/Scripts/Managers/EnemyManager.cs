using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using WhitDataTypes;

public class EnemyManager : MonoBehaviour
{
	private static EnemyManager _instance;

	public static EnemyManager instance {
		get {
			if (_instance == null) {
				_instance = GameObject.FindObjectOfType<EnemyManager> ();
			}
			return _instance;
		}
	}

	public FloatRange enemyEnterDurationRange = new FloatRange (2, 4);
	public FloatRange enemyExitDurationRange = new FloatRange (0.5f, 1.5f);
	public Transform spawnPoint;
	public Transform endPoint;

	private AudioManager.TrackTypes _lastType;

	[SerializeField] private Enemy enemyPrefab;

	private bool songPlaying = true, _readytoSpawn = false;

	private List<Enemy> _enemies = new List<Enemy> ();

	public List<Enemy> enemies {
		get { return _enemies; }
	}

	private int _maxEnemies = 2;

	private void Start ()
	{
		enemyPrefab.CreatePool (10);
		AudioManager.instance.BeatExact += OnBeat;
	}

	private void OnBeat ()
	{
		if (!songPlaying)
			songPlaying = true;
		_readytoSpawn = true;
	}

	private void Update ()
	{
		if (_readytoSpawn && Player_Controller.instance.playerState == Player_Controller.PlayerState.Active) {
			_readytoSpawn = false;
			StartSpawnEnemyCoroutine ();
		}
	}

	void StartSpawnEnemyCoroutine ()
	{
		StartCoroutine (SpawnEnemy ());
	}

	private IEnumerator SpawnEnemy ()
	{
		if (_enemies.Count < _maxEnemies) {
			Enemy enemy = enemyPrefab.Spawn ();
			_enemies.Add (enemy);
			enemy.transform.position = new Vector3 (spawnPoint.position.x, Player_Controller.instance.yCenter + GameManager.instance.enemyVerticalRange.GetRandom (), 0);
		
			AudioManager.TrackTypes trackType = Player_Controller.instance.GetLongestDuration();
			while (trackType == _lastType) {
				trackType = (AudioManager.TrackTypes)Random.Range (0, 5);
				yield return null;
			}

			enemy.SetTrackType (trackType);
			enemy.AddPeople (Random.Range (3, 10));
			enemy.Activate ();

			_lastType = trackType;
		}
	}
}
