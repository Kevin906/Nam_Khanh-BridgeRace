using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    [SerializeField] private GameObject playerBrickPrefabs;
    [SerializeField] private Transform tfPlayerModel;
    [SerializeField] private Transform tfPlayerBrick;
    [SerializeField] private Vector3 direction;
    [SerializeField] private float speed = 5f;

    public float Speed
    {
        get => speed;
        set => speed = value;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void HandleMovement()
    {
        if(Input.GetMouseButtonDown(0))
        {

        }

        if(Input.GetMouseButtonUp(0))
        {

        }
    }
}
