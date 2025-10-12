using UnityEngine;

public class Bot : GameUnit
{
	[SerializeField] private Renderer rdBot;
	[SerializeField] private Renderer rbBotBrick;
	public EColorType colorType = EColorType.Default;

	private StateMachine<Bot> stateMachine;
	private float moveProgress = 0f;

	private void Start()
	{
		stateMachine = new StateMachine<Bot>();
		stateMachine.ChangeState(new BotIdleState(), this);
	}

	private void Update()
	{
		stateMachine.Update(this);
	}

	public void ChangeState(IState<Bot> newState)
	{
		stateMachine.ChangeState(newState, this);
	}

	public void SetColor(Material mat, EColorType type)
	{
		if (rbBotBrick != null && rdBot != null && mat != null)
		{
			rdBot.material = mat;
			rbBotBrick.material = mat;
			colorType = type;
		}
	}

	public void ChangeAnim(string animName = "Idle")
	{
		Debug.Log($"[Anim] {animName}");
	}

	public bool MoveToTarget(Vector3 target, float speed = 1f)
	{
		moveProgress += Time.deltaTime * speed;
		if (moveProgress >= 1f)
		{
			moveProgress = 0f;
			return true;
		}
		return false;
	}

	public bool IsEnoughBrickToBuild()
	{
		return Random.value > 0.8f;
	}
}
