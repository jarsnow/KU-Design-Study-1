using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrialTriggerManager : MonoBehaviour
{
    public GameObject trial_step_button;
    public GameObject agent_trigger_popup;
    public GameObject end_of_room_trigger_popup;
    public GameObject start_of_room_trigger_popup;
    public GameObject agent_end_of_bridge_popup;

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
        trial_step_button.SetActive(false);
        CloseAllPopups();
        switch (new_trial_step)
        {
            case 2: // 3 starts when user approaches agent
                agent_trigger_popup.SetActive(true);
                break;

            case 3: // 4 starts when agent reaches the end of the bridge
                agent_end_of_bridge_popup.SetActive(true);
                break;

            case 4: // 5 starts when user reaches the end of the room
                end_of_room_trigger_popup.SetActive(true);
                break;

            case 5: // 6 starts when user reaches the end of the room
                start_of_room_trigger_popup.SetActive(true);
                break;

            default:
                trial_step_button.SetActive(true);
                break;
        }
    }

    private void CloseAllPopups()
    {
        agent_trigger_popup.SetActive(false);
        end_of_room_trigger_popup.SetActive(false);
        start_of_room_trigger_popup.SetActive(false);
        agent_end_of_bridge_popup.SetActive(false);
    }


}
