using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cam_Control : MonoBehaviour
{
    public float sens_x;
    public float sens_y;

    public Transform orientation;

    float xRotation;
    float yRotation;

    // Start is called before the first frame update
    void Start()
    {
        // lock cursor to center of screen and hide it
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        // get distance to move cam
        float mouse_x = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sens_x;
        float mouse_y = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sens_y;

        // this might look dumb but it works
        yRotation += mouse_x;
        xRotation -= mouse_y;

        // can't look more than straight up or down (no front or back flips)
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // rotate cam
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);

    }
}
