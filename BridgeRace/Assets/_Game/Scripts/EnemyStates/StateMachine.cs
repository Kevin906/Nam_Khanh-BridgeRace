using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine<T>
{
	private IState<T> currentState;

	public void ChangeState(IState<T> newState, T owner)
	{
		if (currentState != null)
			currentState.OnExit(owner);

		currentState = newState;

		if (currentState != null)
			currentState.OnEnter(owner);
	}

	public void Update(T owner)
	{
		if (currentState != null)
			currentState.OnExecute(owner);
	}
}

