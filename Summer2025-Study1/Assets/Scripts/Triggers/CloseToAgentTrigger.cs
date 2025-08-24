using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseToAgentTrigger : MonoBehaviour
{
    public IO_Helper IO;
    public XR_UI_Helper XR_UI_Helper;

    private bool has_been_triggered = false;

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
        // step 2 goes to 3
        if (IO.current_trial_step == 2 && other.CompareTag("VRHead") && !has_been_triggered)
        {
            XR_UI_Helper.DisplayXRUIPanelFromTrialNumber(0);
            XR_UI_Helper.SetNextPanelCloseToWalkAgent();
            has_been_triggered = true;
        }
    }
}
