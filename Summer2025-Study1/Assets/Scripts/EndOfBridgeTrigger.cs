using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndOfBridgeTrigger : MonoBehaviour
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
        // step 4 goes to 5
        if (IO.current_trial_step == 4 && other.CompareTag("VRHead"))
        {
            IO.AdvanceTrial();
        }
    }
}
