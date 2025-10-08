using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brick : GameUnit
{
	[SerializeField] private Renderer brickRenderer;

	private void Awake()
	{
		if (brickRenderer == null)
			brickRenderer = GetComponent<Renderer>();
	}

	public void SetColor(Color color)
	{
		if (brickRenderer != null)
		{
			brickRenderer.material.color = color;
		}
	}

	private void OnEnable()
	{

	}

	private void OnDisable()
	{

	}
}
