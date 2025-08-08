using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Movement : MonoBehaviour
{

    // set a good speed for the player
    public float moveSpeed;

    // player's head cam orientation
    public Transform orientation;

    private float xInput;
    private float yInput;
    private float vertical_input;
    private Vector3 moveDirection;
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.drag = 10;
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    void MovePlayer()
    {
        // get a vector of what direction to move in
        moveDirection = orientation.forward * yInput + orientation.right * xInput;
        // normalize direction vector, multiply by movement speed
        // apply the force to the rigid body
        rb.AddForce(moveDirection.normalized * moveSpeed, ForceMode.Force);

        // vertical force
        rb.AddForce(orientation.up.normalized * moveSpeed * vertical_input, ForceMode.Force);
    }

    void OnWalk(InputValue value)
    {
        xInput = value.Get<Vector2>()[0];
        yInput = value.Get<Vector2>()[1];
    }

    void OnMoveVertically(InputValue value)
    {
        vertical_input = value.Get<float>();
    }

}
