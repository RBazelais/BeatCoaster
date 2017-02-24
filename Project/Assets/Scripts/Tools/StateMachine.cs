using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class StateMachine {
	public Enum lastState {get; private set;}

	public class State {
		public Action UpdateState = DoNothing;
		public Action EnterState = DoNothing;
		public Action ExitState = DoNothing;

		public Action<Vector2> LeftSwipe = DoNothingVector2;
		public Action<Vector2> RightSwipe = DoNothingVector2;
		public Action<Vector2> UpSwipe = DoNothingVector2;
		public Action<Vector2> DownSwipe = DoNothingVector2;
		public Action<Vector2, Vector2, float> Swipe = DoNothingSwipe;
		public Action<Vector2> Tap = DoNothingVector2;
		public Action<Vector2> TouchUp = DoNothingVector2;
		public Action<Vector2> TouchHold = DoNothingVector2;
		public Action<Vector2> TouchDown = DoNothingVector2;
		public Action LeftTouchDown = DoNothing;
		public Action LeftTouchUp = DoNothing;
		public Action RightTouchDown = DoNothing;
		public Action RightTouchUp = DoNothing;
		public Action AnyInput = DoNothing;
		public Action<Vector2> LongPress = DoNothingVector2;
		public Action<SwipeDirection> HoldAfterSwipe = DoNothingHoldAfterSwipe;

		public Enum currentState;
	}

	public Enum currentState {get {return state.currentState;}}	

	protected float timeEnteredState;

	private State state = new State();
	private Dictionary<Enum, Dictionary<string, Delegate>> allStateDelegateDicts = new Dictionary<Enum, Dictionary<string, Delegate>>();
	private Dictionary<Enum, string> enumToStringDict = new Dictionary<Enum, string>();
	private Action preUpdateStateDelegate;
	private Action postUpdateStateDelegate;
	private object target;

	public void Initialize(object target, Action preUpdateStateDelegate, Action postUpdateStateDelegate) {
		this.target = target;
		this.preUpdateStateDelegate = preUpdateStateDelegate;
		this.postUpdateStateDelegate = postUpdateStateDelegate;

		if (InputManager.instance) EnableInput();
	}

	public void Update() {
		PreUpdateState();
		state.UpdateState();
		PostUpdateState();
	}

	public void DisableInput() {
		UnsubscribeFromInputCallbacks();
	}

	public void EnableInput() {
		SubscribeToInputCallbacks();
	}

	public void SetState(Enum state) {
		if (this.state.currentState == state) return;

		OnStateWillChange();
		this.state.currentState = state;
		OnStateChanged();
	}

	private void OnStateWillChange() {
		lastState = state.currentState;
		timeEnteredState = Time.time;
	}

	private void OnStateChanged() {
		if (state.ExitState != null) state.ExitState();

		state.UpdateState = 		GetOrCreateCurrentStateDelegate<Action>("UpdateState", target, DoNothing);
		state.EnterState = 			GetOrCreateCurrentStateDelegate<Action>("EnterState", target, DoNothing);
		state.ExitState = 			GetOrCreateCurrentStateDelegate<Action>("ExitState", target, DoNothing);

		state.LeftSwipe = 			GetOrCreateCurrentStateDelegate<Action<Vector2>>("LeftSwipe", target, DoNothingVector2);
		state.RightSwipe = 			GetOrCreateCurrentStateDelegate<Action<Vector2>>("RightSwipe", target, DoNothingVector2);
		state.UpSwipe = 			GetOrCreateCurrentStateDelegate<Action<Vector2>>("UpSwipe", target, DoNothingVector2);
		state.DownSwipe = 			GetOrCreateCurrentStateDelegate<Action<Vector2>>("DownSwipe", target, DoNothingVector2);
		state.Swipe = 				GetOrCreateCurrentStateDelegate<Action<Vector2, Vector2, float>>("Swipe", target, DoNothingSwipe);
		state.Tap = 				GetOrCreateCurrentStateDelegate<Action<Vector2>>("Tap", target, DoNothingVector2);
		state.TouchUp = 			GetOrCreateCurrentStateDelegate<Action<Vector2>>("TouchUp", target, DoNothingVector2);
		state.TouchHold = 			GetOrCreateCurrentStateDelegate<Action<Vector2>>("TouchHold", target, DoNothingVector2);
		state.TouchDown = 			GetOrCreateCurrentStateDelegate<Action<Vector2>>("TouchDown", target, DoNothingVector2);
		state.LeftTouchDown = 		GetOrCreateCurrentStateDelegate<Action>("LeftTouchDown", target, DoNothing);
		state.LeftTouchUp = 		GetOrCreateCurrentStateDelegate<Action>("LeftTouchUp", target, DoNothing);
		state.RightTouchDown = 		GetOrCreateCurrentStateDelegate<Action>("RightTouchDown", target, DoNothing);
		state.RightTouchUp = 		GetOrCreateCurrentStateDelegate<Action>("RightTouchUp", target, DoNothing);
		state.LongPress = 			GetOrCreateCurrentStateDelegate<Action<Vector2>>("LongPress", target, DoNothingVector2);
		state.HoldAfterSwipe =		GetOrCreateCurrentStateDelegate<Action<SwipeDirection>>("HoldAfterSwipe", target, DoNothingHoldAfterSwipe);
		state.AnyInput = 			GetOrCreateCurrentStateDelegate<Action>("AnyInput", target, DoNothing);

		if (state.EnterState != null) state.EnterState();
	}

	private T GetOrCreateCurrentStateDelegate<T>(string methodSuffix, object target, T Default) where T : class {
		T currentStateDelegate = GetOrCreateStateDelegate<T>(state.currentState, methodSuffix, target, Default);
		return currentStateDelegate;
	}

	private T GetOrCreateStateDelegate<T>(Enum state, string methodSuffix, object target, T Default) where T : class {
		Delegate stateDelegate;
		Dictionary<string, Delegate> stateDelegatesDict = GetOrCreateStateDelegatesDict(state);

		if (!stateDelegatesDict.TryGetValue(methodSuffix, out stateDelegate)) {
			string methodName = GetFullMethodName(state, methodSuffix);
			stateDelegate = WhitTools.CreateDelegate<T>(methodName, target, Default);
			stateDelegatesDict[methodSuffix] = stateDelegate;
		}

		return stateDelegate as T;
	}

	private string EnumToString(Enum e) {
		string s;
		if (enumToStringDict.TryGetValue(e, out s)) {
			return s;
		}
		else {
			s = e.ToString();
			enumToStringDict.Add(e, s);
			return s;
		}

	}

	private Dictionary<string, Delegate> GetOrCreateStateDelegatesDict(Enum state) {
		Dictionary<string, Delegate> stateDelegateDict;
		if (!allStateDelegateDicts.TryGetValue(state, out stateDelegateDict)) {
			allStateDelegateDicts[state] = stateDelegateDict = new Dictionary<string, Delegate>();
		}
		return stateDelegateDict;
	}

	private string GetFullMethodName(Enum state, string methodSuffix) {
		string fullMethodName = EnumToString(state) + "_" + methodSuffix;
		return fullMethodName;
	}

	private void PreUpdateState() {
		preUpdateStateDelegate();
	}

	private void PostUpdateState() {
		postUpdateStateDelegate();
	}

	private void OnLeftSwipe(Vector2 endPosition) {state.LeftSwipe(endPosition);}
	private void OnRightSwipe(Vector2 endPosition) {state.RightSwipe(endPosition);}
	private void OnUpSwipe(Vector2 endPosition) {state.UpSwipe(endPosition);}
	private void OnDownSwipe(Vector2 endPosition) {state.DownSwipe(endPosition);}
	private void OnSwipe(Vector2 endPosition, Vector2 direction, float magnitude) {state.Swipe(endPosition, direction, magnitude);}
	private void HandleLeftTouchDown() {state.LeftTouchDown();}
	private void HandleLeftTouchUp() {state.LeftTouchUp();}
	private void HandleRightTouchDown() {state.RightTouchDown();}
	private void HandleRightTouchUp() {state.RightTouchUp();}
	private void OnTap(Vector2 position) {state.Tap(position);}
	private void OnTouchUp(Vector2 position) {state.TouchUp(position);}
	private void OnTouchHold(Vector2 position) {state.TouchHold(position);}
	private void OnTouchDown(Vector2 position) {state.TouchDown(position);}
	private void OnLongPress(Vector2 position) {state.LongPress(position);}
	private void OnHoldAfterSwipe(SwipeDirection swipeDirection) {state.HoldAfterSwipe(swipeDirection);}
	private void OnAnyInput() {state.AnyInput();}

	private void SubscribeToInputCallbacks() {
		InputManager.instance.SignalTap += OnTap;
		InputManager.instance.SignalLeftSwipe += OnLeftSwipe;
		InputManager.instance.SignalRightSwipe += OnRightSwipe;
		InputManager.instance.SignalUpSwipe += OnUpSwipe;
		InputManager.instance.SignalDownSwipe += OnDownSwipe;
		InputManager.instance.SignalSwipe += OnSwipe;
		InputManager.instance.SignalTouchDown += OnTouchDown;
		InputManager.instance.SignalTouchUp += OnTouchUp;
		InputManager.instance.SignalTouchHold += OnTouchHold;
		InputManager.instance.SignalLongPress += OnLongPress;
		InputManager.instance.SignalHoldAfterSwipe += OnHoldAfterSwipe;
		InputManager.instance.SignalAnyInput += OnAnyInput;
	}

	private void UnsubscribeFromInputCallbacks() {
		InputManager.instance.SignalTap -= OnTap;
		InputManager.instance.SignalLeftSwipe -= OnLeftSwipe;
		InputManager.instance.SignalRightSwipe -= OnRightSwipe;
		InputManager.instance.SignalUpSwipe -= OnUpSwipe;
		InputManager.instance.SignalDownSwipe -= OnDownSwipe;
		InputManager.instance.SignalSwipe -= OnSwipe;
		InputManager.instance.SignalTouchDown -= OnTouchDown;
		InputManager.instance.SignalTouchHold -= OnTouchHold;
		InputManager.instance.SignalTouchUp -= OnTouchUp;
		InputManager.instance.SignalLongPress -= OnLongPress;
		InputManager.instance.SignalHoldAfterSwipe -= OnHoldAfterSwipe;
		InputManager.instance.SignalAnyInput -= OnAnyInput;
	}

	static void DoNothing() {}
	static void DoNothingVector2(Vector2 position) {}
	static void DoNothingSwipe(Vector2 endPosition, Vector2 direction, float magnitude) {}
	static void DoNothingHoldAfterSwipe(SwipeDirection direction) {}
}
