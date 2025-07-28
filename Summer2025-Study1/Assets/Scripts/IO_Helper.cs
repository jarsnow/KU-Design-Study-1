using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices; // needed for file IO
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor.Experimental;

public class IO_Helper : MonoBehaviour
{
    // assign these two to what it says
    public Transform user_cam_position;
    public Transform user_cam_orientation;
    // should be the head of the NPC
    public Transform target_obj;

    // assign to members of UI
    public TMP_InputField subject_name_input;
    public TMP_InputField subject_id_input;
    public TMP_InputField session_number_input;
    public TMP_InputField experimenter_name_input;
    public TMP_InputField save_folder_input;
    public Toggle append_toggle;
    public TMP_Dropdown session_type_input;
    public TMP_Text warning_text;
    public GameObject canvas;
    public GameObject skintone_parent;

    public TMP_Text trial_number_display;
    public TMP_Text gaze_value_display;
    public TMP_Text distance_value_display;

    public GameObject control_ui;
    public GameObject male_model;
    public GameObject female_model;

    public Material[] male_faces;
    public Material[] female_faces;
    public Material[] skintones;

    private string selected_model = "female";
    private UInt16 selected_skintone = 0;

    private DateTime start_time;

    private bool isExperimentStarted;
    // determines amount to round to for all files
    private const UInt16 digits_to_round = 2;

    // FOV is actually different horizontally than vertically for the quest 3, maybe fix later if needed
    private const UInt16 FOV = 110;

    private double moving_gaze_sum;
    private double moving_dist_sum;

    private DateTime last_poll_time;

    private UInt16 current_trial_step = 0;
    

    // similar to SkintoneButtonClicked
    public void ModelButtonClicked(GameObject button_parent)
    {
        string calling_button = button_parent.name;
        selected_model = calling_button; // record last clicked model
        // other_button is male if the button pressed was the female button, etc.
        string other_button = calling_button == "Female" ? "Male" : "Female";

        // enable background for the clicked button
        GameObject background_to_enable = GameObject.Find(calling_button).transform.Find("SelectedBackground").gameObject;
        background_to_enable.SetActive(true);
        // disable other background
        GameObject bg_to_disable = GameObject.Find(other_button).transform.Find("SelectedBackground").gameObject;
        bg_to_disable.SetActive(false);
    }

    // when a button is clicked
    // 1: record it's input
    // 2: disable the rest of the backgrounds
    // 3: enable it's corresponding selected background
    public void SkintoneButtonClicked(GameObject button_parent)
    {
        string calling_button = button_parent.name;
        // 1: get the number attached to each (Color0, Color1)
        selected_skintone = UInt16.Parse(calling_button[5].ToString());
        // 2: disable all the selectedbackgrounds
        for (int i = 0; i <= 7; i++)
        {
            // SelectedBackground is a child of all button parent game objects
            // and is that blue box around the button
            GameObject background_color = GameObject.Find("Color" + i.ToString()).transform.Find("SelectedBackground").gameObject;
            background_color.SetActive(false);
        }
        // enable the blue box around the button that was just clicked
        GameObject color_to_add = button_parent.transform.Find("SelectedBackground").gameObject;
        color_to_add.SetActive(true);
    }

    void Start()
    {
        // clear placeholder warning from the UI
        warning_text.text = "";
        control_ui.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        updateControlUIStats();
        // only write when game has started
        if (isExperimentStarted)
        {
            RecordData();
        }

    }

    void updateModelSkintone(string selected_model, int skintone)
    {
        Debug.Log(selected_model);
        Debug.Log(skintone);
        Material face_material = (selected_model == "Male") ? male_faces[skintone] : female_faces[skintone];
        Material skin_material = skintones[skintone];
        GameObject target_model = (selected_model == "Male") ? male_model : female_model;

        // set skin
        target_model.transform.Find("Wolf3D_Body").GetComponent<SkinnedMeshRenderer>().material = skin_material;
        // set face
        target_model.transform.Find("Wolf3D_Head").GetComponent<SkinnedMeshRenderer>().material = face_material;
    }

    void updateControlUIStats()
    {
        trial_number_display.text = "Trial Step: " + current_trial_step;
        gaze_value_display.text = "Gaze: " + GetGaze();
        distance_value_display.text = "Distance: " + GetDistance();
    }

    // simply write to file
    // a new line is not included when writing to the file
    void WriteToFile(string file_path, string line)
    {
        try {
            File.AppendAllText(file_path, line);
        }
        catch (IOException e)
        {
            Debug.Log(e.Message);
            if (e.Message.Contains("Sharing violation"))
            {
                Debug.Log("Data is not being recorded. Please close out of any programs with any of the .csv files open to continue recording data.");
            }
            //Application.Quit();
        }
    }

    public void HandleButtonPressed()
    {
        // clean up white space around input
        CleanUIInput();

        // display warning if there are any, clear if there are none
        List<string> warnings = GetWarningsForUI();
        warning_text.text = warnings.Count > 0 ? warnings[0] : "";

        // if there are no warnings, start experiment
        if(warnings.Count == 0)
        {
            StartExperiment();
        }
    }

    void StartExperiment()
    {
        isExperimentStarted = true;
        // set session number to 1 (the placeholder) if the user's input is empty
        session_number_input.text = session_number_input.text == "" ? "1" : session_number_input.text;

        // disable starting experiment UI
        // do not disable the UI as a whole because it needs to run the Update() function still
        canvas.SetActive(false);
        control_ui.SetActive(true);

        // start recording data
        // record experiment starting time
        start_time = System.DateTime.Now;
        last_poll_time = start_time;

        // create empty files
        WriteHeadersIfFilesNonexistent();

        string supportive_state = session_type_input.options[session_type_input.value].text.ToLower();
        control_ui.GetComponent<Control_UI_Helper>().updateAnimationMenu(supportive_state);

        updateModelSkintone(selected_model, selected_skintone);

        // turn on NPC models
        if (selected_model == "female")
        {
            female_model.SetActive(true);
            male_model.SetActive(false);
            control_ui.GetComponent<Control_UI_Helper>().updateActiveNPCModel("female");
            control_ui.GetComponent<Control_UI_Helper>().updateActiveNPCModel("female");
        }
        else
        {
            female_model.SetActive(false);
            male_model.SetActive(true);
            control_ui.GetComponent<Control_UI_Helper>().updateActiveNPCModel("male");
        }
    }

    void CleanUIInput()
    {
        // trim leading and trailing whitespaces from every input if needed
        subject_name_input.text = subject_name_input.text.Trim();
        subject_id_input.text = subject_id_input.text.Trim();
        session_number_input.text = session_number_input.text.Trim();
        experimenter_name_input.text = experimenter_name_input.text.Trim();
    }

    List<string> GetWarningsForUI()
    {
        List<string> warnings = new List<string>();

        if (subject_name_input.text == "")
        {
            warnings.Add("The subject's name is missing.");
        }

        if (subject_id_input.text == "")
        {
            warnings.Add("The subject's ID is missing.");
        }

        // ignore the warning for missing session number, it will be defaulted to 0

        if (experimenter_name_input.text == "")
        {
            warnings.Add("The experimenter's name is missing.");
        }

        if (save_folder_input.text == "")
        {
            warnings.Add("The save folder location is missing.");
        }

        return warnings;
    }

    void WriteHeadersIfFilesNonexistent()
    {
        string folder_path = save_folder_input.text;

        string raw_path = folder_path + "\\raw.csv";
        // make raw.csv if not existing
        if (!File.Exists(raw_path))
        {
            // write column names
            string raw_column_names =
                "ppid," +
                "time," +
                "system time," +
                "distance," +
                "gaze," +
                "\n";
            File.WriteAllText(raw_path, raw_column_names);
        }

        string average_path = folder_path + "\\average.csv";
        // make averages.csv if not existing
        if (!File.Exists(average_path))
        {
            // write column names
            string average_column_names =
                "ppid," +
                "time," +
                "system time," +
                "moving average distance," +
                "moving average gaze" +
                "\n";
            File.WriteAllText(average_path, average_column_names);
        }

        string results_path = folder_path + "\\results.csv";
        // make results.csv if not existing
        if (!File.Exists(results_path))
        {
            // write column names
            string results_column_names =
"subject name," +
"subject ID," +
"session," +
"ppid," +
"experimenter name," +
"file path," +
"trial number," +
"start time," +
"end time," +
"model," +
"skintone," +
"session type," +
"trial average distance," +
"trial median distance," +
"trial standard deviation distance," +
"trial average gaze," +
"trial median gaze," +
"trial standard gaze distance," +
"global average distance," +
"global median distance," +
"global standard deviation distance," +
"global average gaze," +
"global median gaze," +
"global standard deviation gaze" +
"\n";
            File.WriteAllText(results_path, results_column_names);
        }
    }


    void RecordData()
    {
        // write to csv in order of
        // ppid, time, system time, distance, gaze
        string line_str = "";

        // ppid
        string ppid = subject_id_input.text + "_" + subject_name_input.text;
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
        string folder_path = save_folder_input.text;
        string raw_path = folder_path + "\\raw.csv";
        WriteToFile(raw_path, line_str);

    }

    // [0, 1] linear value representing eye contact (0 for NPC isn't in the user's FOV, 1 for direct eye contact)
    private double GetGaze()
    {
        // vector3D that describes the vector between the player's camera and the NPC's head
        Vector3 agentLocation = user_cam_position.position;
        Vector3 subjectLocation = target_obj.position;
        Vector3 directionVector = subjectLocation - agentLocation;
        directionVector.Normalize();

        // Get the forward vector of the subject.
        Vector3 subjectForwardVector = user_cam_orientation.forward;

        // Calculate the angle between the subject's forward vector and the direction vector.
        float angleBetween = Vector3.Angle(subjectForwardVector, directionVector);
        float halfFOV = FOV / 2f;

        // Determine gaze score based on the angle between the vectors
        if (angleBetween >= halfFOV)
        {
            return 0.0f;
        }
        else
        {
            return Math.Round(1.0f - (angleBetween / halfFOV), digits_to_round);
        }
    }

    public void AdvanceTrial()
    {
        current_trial_step++;
    }

    private double GetDistance()
    {
        Vector3 dist_vector = target_obj.position - user_cam_position.position;
        return (double) Math.Round(dist_vector.magnitude, digits_to_round);
    }
}
