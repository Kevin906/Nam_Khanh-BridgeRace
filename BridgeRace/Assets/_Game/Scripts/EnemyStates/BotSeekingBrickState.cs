using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;

public class BotSeekingBrickState : IState<Bot>
{
	
	public void OnEnter(Bot t)
	{
		// t.ChangeAnim();
		// Tim vien gach cung mau
		// Vector3 brickPos = LevelManager.Ins.Level.Stage.FindBrickWithColor(t.colorType);
		// t.SetDestination(brickPos);
	}

	public void OnExecute(Bot t)
	{
		// if (t.ReachedDestination()) {
		// t.ChangeState(new BotSeekingBrick()
		// }

		// if (t.IsEnoughBrickToBuild()) {
		// t.ChangeState(new BotBuildBridgeState()) 
		// }
	}

	public void OnExit(Bot t)
	{
		throw new System.NotImplementedException();
	}
}
