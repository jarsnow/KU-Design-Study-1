using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartOfBridgeTrigger : MonoBehaviour
{
    public IO_Helper IO;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnTriggerStay(Collider other)
    {
        // step 5 goes to 6
        if (IO.current_trial_step == 5 && other.CompareTag("VRHead"))
        {
            IO.AdvanceTrial();
        }
    }
}
