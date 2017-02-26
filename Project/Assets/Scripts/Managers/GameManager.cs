using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using WhitDataTypes;
using TMPro;
using System;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
	public Action SignalPlayingStateEnter;

	public enum GameState {
		Title,
		Playing,
		End
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

	public GameState state {
		get {
			return (GameState)stateMachine.currentState;
		}
	}

	private int _droppedListeners;
	public int droppedListeners
	{
		get { return _droppedListeners; }
	}

	public FloatRange enemyVerticalRange = new FloatRange(-4f, 4f);

	[SerializeField] private Player_Controller playerController;
	public GameObject mainMenu;
	public GameObject inGameData;
	public Camera mainCamera;
	public GameObject GameOverMenu;

	private StateMachine stateMachine;

	private float _fadeDelay = 5, _fadeDuration = 5;
	public float fadeDelay
	{
		get { return _fadeDelay;}
	}
	public float fadeDuration
	{
		get{return _fadeDuration;}
	}

	public void SetState(GameState state) {
		stateMachine.SetState(state);
	}

	void Start() {
		stateMachine = new StateMachine();
		stateMachine.Initialize(this, PreUpdateState, PostUpdateState);
		playerController.SetTrails();
		SetState(GameState.Title);
	}

	void Update() {
		stateMachine.Update();
	}

	public void AddDroppedListeners(int amt) {
		_droppedListeners += amt;
	}

	public void SpeedFadeTimes() {
		_fadeDelay = Mathf.Clamp(_fadeDelay -= .25f, 2.5f, 5f);
		_fadeDuration = Mathf.Clamp(_fadeDuration -= .25f, 3f, 5f);
	}

	protected void PreUpdateState() {
		playerController.OnPreUpdateState();
	}

	protected void PostUpdateState() {
		playerController.OnPostUpdateState();
	}

	private void Title_EnterState() {
		//Show the title
		mainMenu.gameObject.GetComponent<Animator>().SetBool("isActive", true);
		inGameData.gameObject.GetComponent<Animator>().SetBool("isActive", false);
		GameOverMenu.gameObject.GetComponent<Animator>().SetBool("isActive", false);
		playerController.OnTitleEnterState();
		mainCamera.DOOrthoSize(2f, 1f);
	}

	public void OnMainMenuTapped() {
		AudioManager.instance.PlayTracks();
		SetState(GameState.Playing);
		playerController.SetState(Player_Controller.PlayerState.Active);
		playerController.StartPlaying();
	}

	private void Title_UpdateState() {
		// Wait for player to press spacebar
		playerController.OnTitleUpdateState();
		if (Input.GetKeyDown(KeyCode.Space)) {
			AudioManager.instance.PlayTracks();
			SetState(GameState.Playing);
			playerController.SetState(Player_Controller.PlayerState.Active);
			playerController.StartPlaying();
		}
	}

	private void Title_ExitState() {
		//Hide the title, start the game
		inGameData.gameObject.SetActive (true);
		mainMenu.gameObject.GetComponent<Animator>().SetBool("isActive", false);
		inGameData.gameObject.GetComponent<Animator>().SetBool("isActive", true);
		GameOverMenu.gameObject.GetComponent<Animator>().SetBool("isActive", false);
		mainCamera.DOOrthoSize(10f, 1f);
	}

	public void End_EnterState() {
		//Show game over menu
		GameOverMenu.gameObject.SetActive (true);
		mainMenu.gameObject.GetComponent<Animator> ().SetBool ("isActive", false);
		inGameData.gameObject.GetComponent<Animator> ().SetBool ("isActive", false);
		GameOverMenu.gameObject.GetComponent<Animator> ().SetBool ("isActive", true);
		playerController.OnGameOverEnterState ();
		mainCamera.DOOrthoSize (2f, 1f);
	}

	private void End_ExitState() {
		//Hide game over menu and show the title
		mainMenu.gameObject.GetComponent<Animator>().SetBool("isActive", true);
		inGameData.gameObject.GetComponent<Animator> ().SetBool ("isActive", false);
		GameOverMenu.gameObject.GetComponent<Animator>().SetBool("isActive", false);
		mainCamera.DOOrthoSize(10f, 1f);

	}

	public void OnGameOverMenuTapped() {
		SceneManager.LoadScene("Main");
	}

	private void End_UpdateState() {
		playerController.OnGameOverUpdateState();
		if (Input.GetKeyDown(KeyCode.Space)) {
			SceneManager.LoadScene("Main");
		}
	}

	private void Playing_EnterState() {
		playerController.OnPlayingEnterState();
		if (SignalPlayingStateEnter != null) SignalPlayingStateEnter();
	}

	private void Playing_ExitState() {
		playerController.OnPlayingExitState();
	}

	private void Playing_UpdateState() {
		playerController.OnPlayingUpdateState();
	}

	private void GameOver_EnterState() {
		playerController.OnGameOverEnterState ();
	}

	private void GameOver_ExitState() {

	}

	private void GameOver_UpdateState() {
		playerController.OnGameOverUpdateState();
	}
}
