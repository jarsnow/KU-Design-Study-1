using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.CompilerServices; // needed for file IO

public class IO_Helper : MonoBehaviour
{
    public Transform user_cam_position;
    public Transform user_cam_orientation;
    public Transform target_obj;
    private DateTime start_time;
    private string output_path;

    void Start()
    {
        // record starting time
        start_time = System.DateTime.Now;

        // get file path for output csv
        output_path = "C:\\Users\\John\\Desktop\\test.csv";
        // ensure file isn't already existing

        if (File.Exists(output_path))
        {
            Debug.LogError("File already exists at: " + output_path);
        }

        // file doesn't exist, make it
        // write column names
        string column_names = "ppid,time,system time,distance,gaze\n";
        File.WriteAllText(output_path, column_names);
    }

    // Update is called once per frame
    void Update()
    {
        // modify maybe
        const int digits_to_round = 6;

        // write to csv in order of
        // ppid, time, system time, distance, gaze
        string line_str = "";

        // ppid
        // TODO: implement with user input screen
        uint test_ppid = 123;
        uint ppid = test_ppid;
        line_str += ppid + ",";

        // time
        // represents the time since the start of the session in seconds
        TimeSpan time_diff = System.DateTime.Now - start_time;
        double seconds_passed = (double) time_diff.TotalSeconds;
        seconds_passed = Math.Round(seconds_passed, digits_to_round);
        line_str += seconds_passed + ",";

        // system time
        // represents the current system time
        string format = "HH:mm:ss";
        string current_time = System.DateTime.Now.ToString(format);
        line_str += current_time + ",";

        // distance
        double dist = GetDistance();
        dist = Math.Round(dist, digits_to_round);
        line_str += dist + ",";

        // gaze
        double gaze = GetGaze();
        gaze = Math.Round(gaze, digits_to_round);
        // don't need a comma because it's the last value
        line_str += gaze;

        // add a new line
        line_str += "\n";

        // write a new line to file
        File.AppendAllText(output_path, line_str);
    }

        // [0, 1] value representing eye contact (0 for fully turned away, 1 for direct eye contact)
    private double GetGaze()
    {
        // vector3D that describes the vector between the player's camera and the NPC's head
        Vector3 target_vector = target_obj.position - user_cam_position.position;
        target_vector.Normalize();

        // vector that describes the rotation of the camera
        Vector3 rotation_vector = user_cam_orientation.forward;
        rotation_vector.Normalize();

        // dot product is the eye contact value
        // dot product has a range of [-1, 1]
        double gaze = Vector3.Dot(target_vector, rotation_vector);

        // turn into range of [0,1]
        gaze = (gaze + 1) / 2;

        return gaze;
    }

    private double GetDistance()
    {
        Vector3 dist_vector = target_obj.position - user_cam_position.position;
        return (double) dist_vector.magnitude;
    }
}
