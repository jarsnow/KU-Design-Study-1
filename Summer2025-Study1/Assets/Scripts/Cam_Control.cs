using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Cam_Control : MonoBehaviour
{

    public Transform orientation;
    private bool isCameraLocked;

    float xRotation;
    float yRotation;

    // Start is called before the first frame update
    void Start()
    {
        isCameraLocked = false;
        // lock cursor to center of screen and hide it
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
    }

    void OnCameraMove(InputValue value)
    {
        if (isCameraLocked)
        {
            return;
        }
        // get distance to move cam
        float mouse_x = value.Get<Vector2>()[0] * (float) 0.02;
        float mouse_y = value.Get<Vector2>()[1] * (float) 0.02;

        // this might look dumb but it works
        yRotation += mouse_x;
        xRotation -= mouse_y;

        // can't look more than straight up or down (no front or back flips)
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // rotate cam
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }


    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Escape))
        //{
        //    toggleCameraLock();
        //}


    }

    public void toggleCameraLock()
    {
        isCameraLocked = !isCameraLocked;
    }

    // needed for interfacing with Input System
    public void OnToggleLock(InputAction.CallbackContext context)
    {
        Debug.Log("escape pressed");
        toggleCameraLock();
    }
}
