using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetVRPosition : MonoBehaviour
{
    public Transform starting_transform;
    public Transform xr_rig;
    public Transform xr_camera;
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
        Debug.Log(starting_transform.position);
        Debug.Log(xr_camera.position);
        xr_rig.localPosition = starting_transform.position - xr_camera.localPosition;
        Debug.Log(xr_rig.position);
        xr_rig.position = new Vector3(xr_rig.position.x, 0, xr_rig.position.z);
        Debug.Log(xr_rig.position);
        //xr_rig.rotation = Quaternion.Inverse(starting_transform.rotation) * xr_camera.rotation;
        //xr_rig.eulerAngles = new Vector3(0, xr_rig.eulerAngles.y, 0);
    }
}
