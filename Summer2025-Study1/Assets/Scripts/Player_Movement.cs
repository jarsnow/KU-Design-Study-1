using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Movement : MonoBehaviour
{

    // set a good speed for the player
    public float moveSpeed;

    // player's head cam orientation
    public Transform orientation;

    float horizontalInput;
    float verticalInput;
    Vector3 moveDirection;
    Rigidbody rb;

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
        GetInput();
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    void MovePlayer()
    {
        // get a vector of what direction to move in
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        // normalize direction vector, multiply by movement speed
        // apply the force to the rigid body
        rb.AddForce(moveDirection.normalized * moveSpeed, ForceMode.Force);
    }

    void GetInput()
    {
        // get input
        // (vertical is W/S)
        // (horizontal is A/D)
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
    }

}
