using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;

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
    public Control_UI_Helper Control_UI_Helper;
    public IO_Helper IO_Helper;

    public CanvasGroup canvas_group;
    public AudioSource audio_source;

    private int current_panel_index = 0;
    private bool next_panel_close_walks_agent = false;

    private TimeSpan time_for_panel_fade_in = TimeSpan.FromSeconds(0.5f);
    private DateTime time_panel_opened = DateTime.MaxValue;

    private TimeSpan time_after_panel_closing_to_walk_agent = TimeSpan.FromSeconds(3);
    private DateTime time_start = DateTime.MaxValue;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMenuCloseButton();
        UpdatePanelOpacity();
        WalkAfterDelay();
    }

    private void UpdatePanelOpacity()
    {
        float progress = (DateTime.Now - time_panel_opened).Ticks / (float) time_for_panel_fade_in.Ticks;
        canvas_group.alpha = progress;
    }

    private void WalkAfterDelay() {
        if (DateTime.Now - time_start > time_after_panel_closing_to_walk_agent)
        {
            // start walking, advance trial step
            Control_UI_Helper.toggleWalking();
            IO_Helper.AdvanceTrial();

            // disable
            time_start = DateTime.MaxValue;
        }
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

        // play sound
        audio_source.Play();

        // reset opacity
        time_panel_opened = DateTime.Now;
    }

    public void SetNextPanelCloseToWalkAgent()
    {
        next_panel_close_walks_agent = true;
    }

    private void UpdateMenuCloseButton()
    {
        float progress = (float) XR_UI.GetComponent<LookAtSquare>().GetLookingProgress();

        // update ring overlay that displays progress
        GameObject selected_panel = panels[current_panel_index];
        GameObject outline_image;

        try
        {
            outline_image = selected_panel.transform.Find("MenuExitArea/Canvas/OutlineImage").gameObject;
            outline_image.GetComponent<Image>().fillAmount = progress;
        } catch (NullReferenceException e){
            // do nothing, there is one popup that is empty
        }

        if (progress == 1)
        {
            CloseAllPanels();
        }

    }

    private void CloseAllPanels()
    {
        XR_UI.GetComponent<LookAtSquare>().ResetProgress();
        foreach (GameObject panel in panels)
        {
            panel.SetActive(false);
        }

        if (next_panel_close_walks_agent)
        {
            time_start = DateTime.Now;
            next_panel_close_walks_agent = false;
        }
    }

}
