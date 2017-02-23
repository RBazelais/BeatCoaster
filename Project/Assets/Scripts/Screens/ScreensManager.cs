using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ScreensManager : MonoBehaviour {
	public static ScreensManager instance;

	public Action<ScreenBase> SignalShowScreen;

	private Canvas canvas;
	private Stack<ScreenBase> screenStack;
	private List<ScreenBase> allScreens;

	private bool initialized = false;

	public static void Create(Canvas canvas) {
		if (instance != null) Debug.LogError("already created!");

		instance = WhitTools.CreateGameObjectWithComponent<ScreensManager>("Screens Manager");
		instance.Initialize(canvas);
	}

	public void PopAllScreens() {
		int screenCount = screenStack.Count;
		for (int i = 0; i < screenCount; i++) {
			Pop();
		}
	}

	public void PopAllButFirstScreen() {
		int screenCount = screenStack.Count;
		if (screenCount > 1) {
			for (int i = 1; i < screenCount; i++) {
				Pop();
			}
		}
	}

	private void Initialize(Canvas canvas) {
		if (initialized) Debug.LogError("already initialized!");
		this.canvas = canvas;
		InitializeScreens();
		initialized = true;
	}

	private void InitializeScreens() {
		screenStack = new Stack<ScreenBase>();
		allScreens = canvas.GetComponentsInChildren<ScreenBase>(true).ToList<ScreenBase>();
		foreach (ScreenBase screen in allScreens) screen.gameObject.SetActive(false);
	}

	private void Update() {
		if (!initialized) return;

		if (screenStack.Count > 0) GetCurrentScreen().OnUpdate();
	}

	public void GoToScreen<T>() where T : ScreenBase {
		PopAllScreens();
		PushScreen<T>();
	}

	public void PushScreen<T>() where T : ScreenBase {
		if (screenStack.Count > 0) {
			ScreenBase currentScreen = GetCurrentScreen();
			currentScreen.OnScreenWillPushAbove();
		}
		ScreenBase screen = GetScreen<T>();
		screenStack.Push(screen);

		screen.OnPush();
		if (SignalShowScreen != null) SignalShowScreen(screen);
	}

	public void Pop() {
		ScreenBase screen = screenStack.Pop();
		screen.OnPop();
		if (screenStack.Count > 0) {
			ScreenBase currentScreen = GetCurrentScreen();
			currentScreen.OnScreenAbovePopped();
			if (SignalShowScreen != null) SignalShowScreen(currentScreen);
		}
	}

	public bool ScreenExistsUnderCurrentScreen() {
		return screenStack.Count > 1;
	}

	public ScreenBase GetScreenUnderCurrentScreen() {
		return screenStack.ElementAt(screenStack.Count - 1);
	}

	public bool HasCurrentScreen() {
		return screenStack.Count > 0;
	}

	public ScreenBase GetCurrentScreen() {
		if (screenStack.Count == 0) return null;
		return screenStack.Peek();
	}

	private T GetScreen<T>() where T : ScreenBase {
		IEnumerable<T> screensOfType = allScreens.OfType<T>();
		if (screensOfType.Count() == 0) Debug.LogError("there are no screens of type " + typeof(T).ToString() + "!");
		if (screensOfType.Count() > 1) Debug.LogError("there is more than one screen of type " + typeof(T).ToString() + "!");

		T screen = screensOfType.ElementAt<T>(0);
		return screen;
	}
}