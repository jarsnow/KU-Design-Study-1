using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrialTriggerManager : MonoBehaviour
{
    public GameObject trial_step_button;
    public GameObject agent_trigger_popup;
    public GameObject end_of_room_trigger_popup;
    public GameObject start_of_room_trigger_popup;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateTrialStepControlUIFromTrialStep(int new_trial_step)
    {
        switch (new_trial_step)
        {
            case 2: // 3 starts when user approaches agent
                trial_step_button.SetActive(false);
                agent_trigger_popup.SetActive(true);
                break;

            case 4: // 5 starts when user reaches the end of the room
                trial_step_button.SetActive(false);
                end_of_room_trigger_popup.SetActive(true);
                break;

            case 5: // 6 starts when user reaches the end of the room
                trial_step_button.SetActive(false);
                start_of_room_trigger_popup.SetActive(true);
                break;

            default:
                trial_step_button.SetActive(true);

                agent_trigger_popup.SetActive(false);
                end_of_room_trigger_popup.SetActive(false);
                start_of_room_trigger_popup.SetActive(false);
                break;
        }
    }


}
