using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using WhitDataTypes;
using TMPro;

public class GameManager : MonoBehaviour {
	public enum GameState {
		Title,
		Playing,
		GameOver
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

	public FloatRange enemyVerticalRange = new FloatRange(-4f, 4f);

	[SerializeField] private Player_Controller playerController;
	[SerializeField] private TextMeshProUGUI titleText;
	public GameObject mainMenu;
	public Camera mainCamera;

	private StateMachine stateMachine;

	public void SetState(GameState state) {
		stateMachine.SetState(state);
	}

	void Start() {
		stateMachine = new StateMachine();
		stateMachine.Initialize(this, PreUpdateState, PostUpdateState);
		SetState(GameState.Title);
		//playerController.SetState(Player_Controller.PlayerState.Idle);
	}

	void Update() {
		stateMachine.Update();
	}

	protected void PreUpdateState() {
		playerController.OnPreUpdateState();
	}

	protected void PostUpdateState() {
		playerController.OnPostUpdateState();
	}

	private void Title_EnterState() {
		mainMenu.gameObject.GetComponent<Animator>().SetBool("isActive", true);
		playerController.OnTitleEnterState();
		mainCamera.DOOrthoSize(2f, 1f);
	}

	private void Title_ExitState() {
		mainMenu.gameObject.GetComponent<Animator>().SetBool("isActive", false);
		mainCamera.DOOrthoSize(10f, 1f);
	}

	private void Title_UpdateState() {
		playerController.OnTitleUpdateState();
		if (Input.GetKeyDown(KeyCode.Space)) {
			SetState(GameState.Playing);
			playerController.SetState(Player_Controller.PlayerState.Active);
			playerController.StartPlaying();
		}
	}

	private void Playing_EnterState() {
		playerController.OnPlayingEnterState();
	}

	private void Playing_ExitState() {

	}

	private void Playing_UpdateState() {
		playerController.OnPlayingUpdateState();
	}

	private void GameOver_EnterState() {
		playerController.OnGameOverEnterState();
	}

	private void GameOver_ExitState() {

	}

	private void GameOver_UpdateState() {
		playerController.OnGameOverUpdateState();
	}
}
