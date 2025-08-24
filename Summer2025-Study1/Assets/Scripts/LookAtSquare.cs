using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtSquare : MonoBehaviour
{
    public CapsuleCollider XR_sight_line;

    private TimeSpan looking_progress_time;
    private TimeSpan time_needed_to_close_menu = TimeSpan.FromSeconds(3);

    private DateTime last_update_time = DateTime.MinValue;

    private bool is_user_looking_at_box;

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

        if (is_user_looking_at_box)
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

    public void SetUserLookingAtBox(bool val)
    {
        is_user_looking_at_box = val;
    }

    public double GetLookingProgress()
    {
        return looking_progress_time.TotalSeconds / time_needed_to_close_menu.TotalSeconds;
    }

    public void ResetProgress()
    {
        is_user_looking_at_box = false;
        looking_progress_time = TimeSpan.Zero;
    }

}
