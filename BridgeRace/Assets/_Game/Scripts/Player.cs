using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
public class Player : MonoBehaviour
{
	[Header("Refs")]
	[SerializeField] private GameObject playerBrickPrefabs;
	[SerializeField] private Rigidbody rbPlayer;
	[SerializeField] private Transform tfPlayerBrick;
	[SerializeField] private FixedJoystick fJoyStick;
	[SerializeField] private MeshRenderer mrPlayer;
	[SerializeField] private MeshRenderer mrPlayerBrick;

	[Header("Movement")]
	[SerializeField] private float speed = 5f;

	[Header("Gameplay")]
	public EColorType colorType;

	void Update()
	{
		HandleMovement();
	}

	private void HandleMovement()
	{
		rbPlayer.velocity = new Vector3(fJoyStick.Horizontal * speed, rbPlayer.velocity.y, fJoyStick.Vertical * speed);

		if (fJoyStick.Horizontal != 0 || fJoyStick.Vertical != 0)
		{
			transform.rotation = Quaternion.LookRotation(new Vector3(fJoyStick.Horizontal, 0, fJoyStick.Vertical));
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		Addbrick(other);
	}

	private void Addbrick(Collider col)
	{

	}
	public void SetColor(Material mat, EColorType type)
	{
		colorType = type;
		if (mrPlayer != null && mrPlayerBrick != null && mat != null)
		{
			mrPlayer.material = mat;
			mrPlayerBrick.material = mat;
			colorType = type;
		}
	}
}
