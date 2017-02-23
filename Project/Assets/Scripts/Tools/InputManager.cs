using UnityEngine;
using System.Collections;
using System;

public enum SwipeDirection {
	Up,
	Right,
	Down,
	Left
}

public class InputManager : MonoBehaviour {
	public static InputManager instance {get; private set;}

	public bool inputEnabled {get; private set;}

	public Action<Vector2> SignalTap;
	public Action<Vector2> SignalRightSwipe;
	public Action<Vector2> SignalLeftSwipe;
	public Action<Vector2> SignalUpSwipe;
	public Action<Vector2> SignalDownSwipe;
	public Action<Vector2, Vector2, float> SignalSwipe;
	public Action<Vector2> SignalLongPress;
	public Action<Vector2> SignalTouchDown;
	public Action<Vector2> SignalTouchHold;
	public Action<Vector2> SignalTouchUp;
	public Action SignalAnyInput;
	public Action SignalCancel;
	public Action<SwipeDirection> SignalHoldAfterSwipe;

	private float minSwipeLength = 50;
	private float longPressDuration = 0.3f;

	private ScreenBase startScreen;
	private Vector2 startPos;
	private Vector2 curPos;
	private float startTime;
	private int leftTouchID;
	private int rightTouchID;
	private bool longPressHappened = false;
	private bool swipeHappened = false;
	private bool touchInSession = false;
	private bool sentInputSignalThisFrame = false;
	private SwipeDirection lastSwipeDirection = SwipeDirection.Up;

	public static void Create() {
		if (instance != null) Debug.LogError("already created!");

		InputManager.instance = WhitTools.CreateGameObjectWithComponent<InputManager>("Input Manager");
		InputManager.instance.Initialize();
	}

	public void CancelInput() {
		EndTouchSession();
		if (SignalCancel != null) SignalCancel();
	}

	public float GetLongPressDuration() {
		return longPressDuration;
	}

	private void EndTouchSession() {
		touchInSession = false;
		longPressHappened = false;
		swipeHappened = false;
		sentInputSignalThisFrame = false;
		lastSwipeDirection = SwipeDirection.Up;
	}

	private void Initialize() {
		inputEnabled = true;
	}

	private void Update() {
		if (!inputEnabled) return;
		if (!CurrentScreenExists()) return;

		if (PlatformInfo.IsEditor() || PlatformInfo.IsStandalone()) {
			DetectMouseInput();
		}
		else if (PlatformInfo.IsMobile()) {
			DetectTouchInput();
		}
	}






	public void DisableInput() {
		inputEnabled = false;
	}

	public void EnableInput() {
		inputEnabled = true;
	}




	private bool GetTouchDown(Touch touch) {
		return touch.phase == TouchPhase.Began;
	}

	private bool GetTouchHold(Touch touch) {
		return touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary;
	}

	private bool GetTouchUp(Touch touch) {
		return touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled;
	}

	private bool GetMouseDown() {
		return Input.GetMouseButtonDown(0);
	}

	private bool GetMouseHold() {
		return Input.GetMouseButton(0);
	}

	private bool GetMouseUp() {
		return Input.GetMouseButtonUp(0);
	}

	private float GetTouchDuration() {
		return Time.time - startTime;
	}

	private Vector2 GetTouchVector() {
		return curPos - startPos;
	}

	private bool CurrentScreenExists() {
		return ScreensManager.instance.HasCurrentScreen();
	}

	private ScreenBase GetCurrentScreen() {
		return ScreensManager.instance.GetCurrentScreen();
	}

	private bool CurrentScreenIsSameAsStartScreen() {
		return startScreen == GetCurrentScreen();
	}




	private void RecordStart(Vector2 startPos) {
		touchInSession = true;
		this.startPos = startPos;
		RecordCur(startPos);
		this.startTime = Time.time;
		this.startScreen = ScreensManager.instance.GetCurrentScreen();
	}

	private void RecordCur(Vector2 curPos) {
		this.curPos = curPos;
	}


	private bool IsSwipe() {
		float magnitude = GetTouchVector().magnitude;
		bool isOverMinLength = magnitude >= minSwipeLength;
		return isOverMinLength;
	}

	private bool IsLongPress() {
		return touchInSession && GetTouchDuration() >= longPressDuration;
	}


	private void DetectTouchInput() {
		DetectTouchDown();
		DetectTouchHold();
		DetectSwipes();
		DetectHoldAfterSwipe();
		DetectLongPress();
		DetectTouchUp();
		DetectAnyInput();
	}

	private void DetectAnyInput() {
		if (sentInputSignalThisFrame) {
			SendAnyInputSignal();
			sentInputSignalThisFrame = false;
		}
	}

	private void DetectSwipes() {
		if (!touchInSession) return;
		if (Input.touches.Length == 0) return;
		Touch touch = Input.GetTouch(0);
		if (!CurrentScreenIsSameAsStartScreen()) CancelInput();
		if (!swipeHappened) {
			if (IsSwipe()) {
				SendSwipeSignals(touch.position);
			}
		}
	}

	private void DetectTouchHold() {
		if (!touchInSession) return;
		if (Input.touches.Length == 0) return;
		Touch touch = Input.GetTouch(0);
		if (GetTouchHold(touch)) {
			RecordCur(touch.position);
			SendTouchHoldSignal(touch.position);
		}
	}

	private void DetectTouchDown() {
		if (Input.touches.Length == 0) return;
		Touch touch = Input.GetTouch(0);
		if (GetTouchDown(touch)) {
			RecordStart(touch.position);
			SendTouchDownSignal(touch.position);
		}
	}

	private void DetectTouchUp() {
		if (!touchInSession) return;
		if (Input.touches.Length == 0) return;
		Touch touch = Input.GetTouch(0);
		if (!CurrentScreenIsSameAsStartScreen()) CancelInput();
		if (GetTouchUp(touch)) {
			SendTouchUpSignal(touch.position);

			if (!longPressHappened && !swipeHappened) {
				SendTapSignal(touch.position);
			}

			EndTouchSession();
		}
	}

	private void DetectLongPress() {
		if (!touchInSession) return;
		if (Input.touches.Length == 0) return;
		Touch touch = Input.GetTouch(0);
		if (!CurrentScreenIsSameAsStartScreen()) CancelInput();
		if (!longPressHappened) {
			if (IsLongPress()) {
				SendLongPressSignal(touch.position);
			}
		}
	}

	private void DetectHoldAfterSwipe() {
		if (!touchInSession) return;
		if (!swipeHappened) return;
		if (longPressHappened) return;
		if (Input.touches.Length == 0) return;
		Touch touch = Input.GetTouch(0);
		if (!CurrentScreenIsSameAsStartScreen()) CancelInput();
		if (GetTouchHold(touch)) {
			SendHoldAfterSwipeSignal();
		}
	}




	private void DetectMouseInput() {
		DetectMouseDown();
		DetectMouseHold();
		DetectMouseSwipes();
		DetectMouseHoldAfterSwipe();
		DetectMouseLongPress();
		DetectMouseUp();
		DetectAnyInput();
	}

	private void DetectMouseDown() {
		if (GetMouseDown()) {
			RecordStart(Input.mousePosition);
			SendTouchDownSignal(Input.mousePosition);
		}
	}

	private void DetectMouseUp() {
		if (!touchInSession) return;
		if (!CurrentScreenIsSameAsStartScreen()) CancelInput();
		if (GetMouseUp()) {
			RecordCur(Input.mousePosition);
			SendTouchUpSignal(Input.mousePosition);

			if (!longPressHappened && !swipeHappened) {
				SendTapSignal(Input.mousePosition);
			}

			EndTouchSession();
		}
	}

	private void DetectMouseSwipes() {
		if (!touchInSession) return;
		if (!CurrentScreenIsSameAsStartScreen()) CancelInput();
		if (!swipeHappened) {
			if (IsSwipe()) {
				SendSwipeSignals(Input.mousePosition);
			}
		}
	}

	private void DetectMouseLongPress() {
		if (!touchInSession) return;
		if (!CurrentScreenIsSameAsStartScreen()) CancelInput();
		if (!longPressHappened) {
			if (IsLongPress()) {
				SendLongPressSignal(Input.mousePosition);
			}
		}
	}

	private void DetectMouseHoldAfterSwipe() {
		if (!touchInSession) return;
		if (!swipeHappened) return;
		if (longPressHappened) return;
		if (!CurrentScreenIsSameAsStartScreen()) CancelInput();
		if (GetMouseHold()) {
			SendHoldAfterSwipeSignal();
		}
	}

	private void DetectMouseHold() {
		if (!touchInSession) return;
		if (GetMouseHold()) {
			RecordCur(Input.mousePosition);
			SendTouchHoldSignal(Input.mousePosition);
		}
	}





	private void SendTapSignal(Vector2 position) {
		if (SignalTap != null) SignalTap(position);
		OnSentInputSignal();
	}

	private void SendTouchUpSignal(Vector2 position) {
		if (SignalTouchUp != null) SignalTouchUp(position);
		OnSentInputSignal();
	}

	private void SendLongPressSignal(Vector2 position) {
		longPressHappened = true;
		if (SignalLongPress != null) SignalLongPress(position);
		OnSentInputSignal();
	}

	private void SendTouchDownSignal(Vector2 position) {
		if (SignalTouchDown != null) SignalTouchDown(position);
		OnSentInputSignal();
	}

	private void SendTouchHoldSignal(Vector2 position) {
		if (SignalTouchHold != null) SignalTouchHold(position);
		OnSentInputSignal();
	}

	private void SendSwipeSignals(Vector2 endPosition) {
		Vector2 vector = GetTouchVector();
		Vector2 direction = vector.normalized;
		float magnitude = vector.magnitude;

		if (WhitTools.IsLeft(direction)) {
			lastSwipeDirection = SwipeDirection.Left;
			if (SignalLeftSwipe != null) SignalLeftSwipe(endPosition);
		}
		else if (WhitTools.IsRight(direction)) {
			lastSwipeDirection = SwipeDirection.Right;
			if (SignalRightSwipe != null) SignalRightSwipe(endPosition);
		}
		else if (WhitTools.IsUp(direction)) {
			lastSwipeDirection = SwipeDirection.Up;
			if (SignalUpSwipe != null) SignalUpSwipe(endPosition);
		}
		else if (WhitTools.IsDown(direction)) {
			lastSwipeDirection = SwipeDirection.Down;
			if (SignalDownSwipe != null) SignalDownSwipe(endPosition);
		}
			
		if (SignalSwipe != null) SignalSwipe(endPosition, direction, magnitude);

		swipeHappened = true;
		OnSentInputSignal();
	}

	private void SendHoldAfterSwipeSignal() {
		if (SignalHoldAfterSwipe != null) SignalHoldAfterSwipe(lastSwipeDirection);
		OnSentInputSignal();
	}

	private void SendAnyInputSignal() {
		if (SignalAnyInput != null) SignalAnyInput();
	}

	private void OnSentInputSignal() {
		sentInputSignalThisFrame = true;
	}
}