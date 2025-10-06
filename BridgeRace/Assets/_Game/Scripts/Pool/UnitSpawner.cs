using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSpawner : MonoBehaviour
{
	public Transform[] botSpawnPoints;
	public Transform[] brickSpawnPoints;

	[SerializeField] private float spacing = 2f;
	void Update()
    {
		SpawnBot();
		SpawnBrick();
    }

	private void SpawnBrick()
	{
		
	}

	private void SpawnBot()
	{

	}
}
