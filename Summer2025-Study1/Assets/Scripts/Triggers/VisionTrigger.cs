using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisionTrigger : MonoBehaviour
{
    public GameObject XRUI;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("VRSightLine")){
            return;
        }

        XRUI.GetComponent<LookAtSquare>().SetUserLookingAtBox(true);
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("VRSightLine")){
            return;
        }

        XRUI.GetComponent<LookAtSquare>().SetUserLookingAtBox(false);
    }

    void OnTriggerStay(Collider other)
    {
    }
}
