using System.Collections;
using System.Collections.Generic;
//using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;


public class XR_UI_Helper : MonoBehaviour
{
    public GameObject XR_UI;
    public Transform UI_target_pivot_point;
    public Transform xr_target_panel_transform;
    public Transform xr_camera_transform;
    public GameObject[] panels;

    private int current_panel_index = 0;

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
        current_panel_index = trialNumber;

        CloseAllPanels();
        GameObject selected_panel = panels[current_panel_index];

        selected_panel.SetActive(true);

        // test to make sure the panel appears horizontal when the user is looking up/down and next trial step occurs
        Vector3 new_angles = UI_target_pivot_point.eulerAngles;
        new_angles.x = 0;
        UI_target_pivot_point.eulerAngles = new_angles;
       
        selected_panel.transform.position = xr_target_panel_transform.position;
        selected_panel.transform.LookAt(xr_camera_transform);

        // canvases are backwards so rotate 180
        selected_panel.transform.Rotate(0, 180, 0);
    }

    private void UpdateMenuCloseButton()
    {
        float progress = (float) XR_UI.GetComponent<LookAtSquare>().GetLookingProgress();

        // update ring overlay that displays progress
        GameObject selected_panel = panels[current_panel_index];
        GameObject outline_image = selected_panel.transform.Find("MenuExitArea/Canvas/OutlineImage").gameObject;
        outline_image.GetComponent<Image>().fillAmount = progress;

        if (progress == 1)
        {
            CloseAllPanels();
        }

    }

    private void CloseAllPanels()
    {
        foreach  (GameObject panel in panels)
        {
            panel.SetActive(false);
        }
    }

}
