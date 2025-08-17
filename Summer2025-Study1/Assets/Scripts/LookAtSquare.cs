using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtSquare : MonoBehaviour
{
    public BoxCollider box_collider;
    public CapsuleCollider XR_cylinder_collider;

    private TimeSpan looking_progress_time;
    private TimeSpan time_needed_to_close_menu = TimeSpan.FromSeconds(3);

    private DateTime last_update_time = DateTime.MinValue;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (last_update_time == DateTime.MinValue)
        {
            last_update_time = DateTime.Now;
            return;
        }

        TimeSpan time_since_last_update = DateTime.Now - last_update_time;

        if (IsUserLookingAtBox())
        {
            looking_progress_time += time_since_last_update;
            if (looking_progress_time > time_needed_to_close_menu)
            {
                looking_progress_time = time_needed_to_close_menu;
            }
        }
        else
        {
            looking_progress_time -= time_since_last_update;
            if (looking_progress_time < TimeSpan.Zero)
            {
                looking_progress_time = TimeSpan.Zero;
            }
        }

        last_update_time = DateTime.Now;
    }

    private bool IsUserLookingAtBox()
    {
        Debug.Log((box_collider.bounds.Intersects(XR_cylinder_collider.bounds)));

        return false;
    }

    public void SetNewMenuCollider(BoxCollider new_box)
    {
        ResetProgress();
        box_collider = new_box;
    }

    public double GetLookingProgress()
    {
        return looking_progress_time.TotalSeconds / time_needed_to_close_menu.TotalSeconds;
    }

    private void ResetProgress()
    {
        looking_progress_time = TimeSpan.Zero;
    }

}
