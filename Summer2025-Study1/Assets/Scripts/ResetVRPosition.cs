using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetVRPosition : MonoBehaviour
{
    public Transform starting_transform;
    public Transform xr_rig;
    public Transform xr_head;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ResetPos()
    {
        // do 5 times because it works only after the first one??? no idea why but it works
        for (int i = 0; i < 5; i++)
        {
            // reset position to origin point
            Vector3 offset = xr_head.position - xr_rig.position;
            xr_rig.position = starting_transform.position - offset;
            xr_rig.position = new Vector3(xr_rig.position.x, 0, xr_rig.position.z);

            float difference = starting_transform.eulerAngles.y - xr_head.eulerAngles.y;
            xr_rig.Rotate(0f, difference, 0f, Space.World);
        }
    }
}
