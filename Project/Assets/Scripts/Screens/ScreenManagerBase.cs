using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class ScreenManagerBase : MonoBehaviour, IScreenCallbacks {
	[SerializeField] protected ScreenBase screen;

	private StateMachine stateMachine;

	public virtual void OnInitialize() {
		stateMachine = new StateMachine();
		stateMachine.Initialize(this, PreUpdateState, PostUpdateState);
		stateMachine.DisableInput();
	}

	public void OnUpdate() {
		stateMachine.Update();
	}

	public virtual void OnScreenWillLoseFocus() {
		stateMachine.DisableInput();
	}

	public virtual void OnScreenGainedFocus() {
		stateMachine.EnableInput();	
	}

	public virtual void OnCleanUp() {
		
	}

	protected void SetState(Enum state) {
		stateMachine.SetState(state);
	}

	protected virtual void PreUpdateState() {
		
	}

	protected virtual void PostUpdateState() {
		
	}
}
