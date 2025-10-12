using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotIdleState : IState<Bot>
{
	public void OnEnter(Bot t)
	{
		t.ChangeAnim();
	}

	public void OnExecute(Bot t)
	{
		// if (GameManager.Ins.IsGameState(EGameState.Playing) {
		// t.ChangeState(new BotSeekingBrickState()) 
		// }
	}

	public void OnExit(Bot t)
	{
		throw new System.NotImplementedException();
	}
}
