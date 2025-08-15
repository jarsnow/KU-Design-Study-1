using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class XR_UI_Helper : MonoBehaviour
{
    public Transform xr_target_panel_transform;
    public Transform xr_camera_transform;
    public GameObject[] panels;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DisplayXRUIPanelFromTrialNumber(int trialNumber)
    {
        int panel_index = trialNumber - 1;
        
        foreach  (GameObject panel in panels)
        {
            panel.SetActive(false);
        }

        GameObject selected_panel = panels[panel_index];

        selected_panel.SetActive(true);
        selected_panel.transform.position = xr_target_panel_transform.position;
        selected_panel.transform.LookAt(xr_camera_transform);
    }
}
