using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class XR_UI_Helper : MonoBehaviour
{
    public GameObject XR_UI;
    public Transform xr_target_panel_transform;
    public Transform xr_camera_transform;
    public GameObject[] panels;
    public Collider[] menu_colliders;

    // Start is called before the first frame update
    void Start()
    {
        //XR_UI.GetComponent<LookAtSquare>().SetNewMenuCollider(box_colliders[0]);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMenuCloseButton();
    }

    public void DisplayXRUIPanelFromTrialNumber(int trialNumber)
    {
       int panel_index = trialNumber;
        
        foreach  (GameObject panel in panels)
        {
            panel.SetActive(false);
        }

        GameObject selected_panel = panels[panel_index];

        selected_panel.SetActive(true);
        selected_panel.transform.position = xr_target_panel_transform.position;
        selected_panel.transform.LookAt(xr_camera_transform);

        // canvases are backwards so rotate 180
        selected_panel.transform.Rotate(0, 180, 0);
    }

    private void UpdateMenuCloseButton()
    {
        float progress = XR_UI.GetComponent<LookAtSquare>().GetLookingProgress();

        // get image
    }

}
