using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
public class NewBehaviourScript : MonoBehaviour
{
    [SerializeField] private GameObject playerBrickPrefabs;
    [SerializeField] private Rigidbody rbPlayer;
    [SerializeField] private FixedJoystick fJoyStick;
    [SerializeField] private Vector3 direction;
    [SerializeField] private Vector2 startTouch;
    [SerializeField] private float speed = 5f;

    // Update is called once per frame
    void Update()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        rbPlayer.velocity = new Vector3(fJoyStick.Horizontal * speed, rbPlayer.velocity.y, fJoyStick.Vertical * speed);

        if(fJoyStick.Horizontal != 0 || fJoyStick.Vertical != 0)
        {
            transform.rotation = Quaternion.LookRotation(new Vector3(fJoyStick.Horizontal, 0, fJoyStick.Vertical));
        }
    }
}
