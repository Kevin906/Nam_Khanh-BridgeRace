using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brick : GameUnit
{
	[SerializeField] private Renderer brickRenderer;

	public EColorType colorType;

	public void SetColor(Material mat, EColorType colorType)
	{
		if (brickRenderer != null && mat != null)
		{
			brickRenderer.material = mat;
		}
		this.colorType = colorType;
	}

	private void OnEnable()
	{

	}

	private void OnDisable()
	{

	}
}
