using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetVRPosition : MonoBehaviour
{
    public Transform starting_transform;
    public GameObject xr_rig;
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
        xr_rig.transform.position = starting_transform.position;
        xr_rig.transform.rotation = starting_transform.rotation;
    }
}
