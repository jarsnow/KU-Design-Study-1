using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices; // needed for file IO
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor.Experimental;
using UnityEditor;
using System.Linq;

public class IO_Helper : MonoBehaviour
{
    public GameObject XRUI;

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
    private DateTime trial_zero_start_time;
    private DateTime last_trial_start_time;

    private bool isExperimentStarted;
    // determines amount to round to for all files
    private const UInt16 digits_to_round = 2;

    // FOV is actually different horizontally than vertically for the quest 3, maybe fix later if needed
    private const UInt16 FOV = 110;

    private double moving_gaze_sum;
    private double moving_dist_sum;
    private UInt16 moving_frames_recorded;

    // trial averages, median for gaze / distance
    private double trial_dist_sum;
    private List<double> trial_dist_values = new List<double>();

    private double trial_gaze_sum;
    private List<double> trial_gaze_values = new List<double>();

    // session averages, median for gaze / distance
    private double session_dist_sum;
    private List<double> session_dist_values = new List<double>();

    private double session_gaze_sum;
    private List<double> session_gaze_values = new List<double>();

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
        //Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
        // clear placeholder warning from the UI
        warning_text.text = "";
        control_ui.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isExperimentStarted)
        {
            return;
        }

        updateControlUIStats();

        // recording begins only after trial step 0
        if (current_trial_step > 0)
        {
            RecordData();
        }

        // record data for trial zero
        if (current_trial_step == 0)
        {
            RecordTrialZeroData();
        }
    }

    void updateModelSkintone(string selected_model, int skintone)
    {
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

        // record start time for trial zero
        trial_zero_start_time = System.DateTime.Now;
        last_trial_start_time = System.DateTime.Now;

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



    private void RecordData()
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

        // add values to averages
        moving_dist_sum += dist;
        moving_gaze_sum += gaze;
        moving_frames_recorded++;

        // trial info
        trial_dist_sum += dist;
        trial_dist_values.Add(dist);

        trial_gaze_sum += gaze;
        trial_gaze_values.Add(gaze);

        // session info
        session_dist_sum += dist;
        session_dist_values.Add(dist);

        session_gaze_sum += gaze;
        session_gaze_values.Add(gaze);

        TimeSpan time_passed_since_last_poll = System.DateTime.Now - last_poll_time;
        if (time_passed_since_last_poll.TotalSeconds > 60)
        {
            RecordAverages();
        }
    }

    // does not actually write anything to files
    // data collection should only start after trial zero
    private void RecordTrialZeroData()
    {
        double dist = GetDistance();
        double gaze = GetGaze();

        // trial info
        trial_dist_sum += dist;
        trial_dist_values.Add(dist);

        trial_gaze_sum += gaze;
        trial_gaze_values.Add(gaze);

        // session info
        session_dist_sum += dist;
        session_dist_values.Add(dist);

        session_gaze_sum += gaze;
        session_gaze_values.Add(gaze);
    }

    // should record averages of distance and gaze every 60 seconds
    private void RecordAverages()
    {
        // write to csv in order of
        // ppid, time, system time, moving avg distance, moving avg gaze
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

        // distance avg of the last 60 seconds
        double distance_avg = Math.Round((double) moving_dist_sum / moving_frames_recorded, digits_to_round);
        double gaze_avg = Math.Round((double) moving_gaze_sum / moving_frames_recorded, digits_to_round);

        line_str += distance_avg.ToString() + ",";
        line_str += gaze_avg.ToString() + "\n";

        // write a new line to file
        string folder_path = save_folder_input.text;
        string average_path = folder_path + "\\average.csv";
        WriteToFile(average_path, line_str);

        // reset for next 60 seconds
        moving_dist_sum = 0;
        moving_gaze_sum = 0;
        moving_frames_recorded = 0;
        last_poll_time = System.DateTime.Now;
    }

    private void RecordTrialResults()
    {
        string line_str = "";
        int trial_frames_recorded = trial_dist_values.Count();
        int session_frames_recorded = session_dist_values.Count();

        // subject name
        string subject_name = subject_name_input.text;
        line_str += subject_name + ",";

        // subject id
        string subject_id = subject_id_input.text;
        line_str += subject_id + ",";

        // session
        string session_num = session_number_input.text;
        line_str += session_num + ",";

        // ppid
        string ppid = subject_id_input.text + "_" + subject_name_input.text;
        line_str += ppid + ",";

        // experimenter name
        string experimenter_name = experimenter_name_input.text;
        line_str += experimenter_name + ",";

        // file path
        string file_path = save_folder_input.text;
        line_str += file_path + ",";

        // trial number
        line_str += current_trial_step.ToString() + ",";

        // start time
        TimeSpan trial_start_time = last_trial_start_time - trial_zero_start_time;
        string trial_start = Math.Round(trial_start_time.TotalSeconds, digits_to_round).ToString();
        line_str += trial_start + ",";

        // end time
        TimeSpan time_since_session_start = System.DateTime.Now - trial_zero_start_time;
        string time_since_start = Math.Round(time_since_session_start.TotalSeconds, digits_to_round).ToString();
        line_str += time_since_start + ",";

        // model
        line_str += selected_model + ",";

        // skintone
        string skintone = selected_skintone.ToString();
        line_str += skintone + ",";

        // session type
        string session_type = session_type_input.options[session_type_input.value].text.ToLower();
        line_str += session_type + ",";

        // trial average distance
        double trial_average_distance = Math.Round(((double) trial_dist_sum / trial_frames_recorded), digits_to_round);
        line_str += trial_average_distance.ToString() + ",";

        // trial median distance
        line_str += GetMedian(trial_dist_values).ToString() + ",";

        // trial standard deviation distance
        line_str += GetStandardDeviationWithAverage(trial_dist_values, trial_average_distance).ToString() + ",";

        // trial average gaze
        double trial_average_gaze = Math.Round(((double) trial_gaze_sum / trial_frames_recorded), digits_to_round);
        line_str += trial_average_gaze.ToString() + ",";

        // trial median gaze
        line_str += GetMedian(trial_gaze_values).ToString() + ",";

        // trial standard deviation gaze
        line_str += GetStandardDeviationWithAverage(trial_gaze_values, trial_average_gaze).ToString() + ",";

        // session average distance
        double session_average_distance = Math.Round(((double) session_dist_sum / session_frames_recorded), digits_to_round);
        line_str += session_average_distance.ToString() + ",";

        // session median distance
        line_str += GetMedian(session_dist_values).ToString() + ",";

        // session standard deviation distance
        line_str += GetStandardDeviationWithAverage(session_dist_values, session_average_distance).ToString() + ",";

        // session average gaze
        double session_average_gaze = Math.Round(((double) session_gaze_sum / session_frames_recorded), digits_to_round);
        line_str += session_average_gaze.ToString() + ",";

        // session median gaze
        line_str += GetMedian(session_gaze_values).ToString() + ",";

        // session standard deviation gaze
        line_str += GetStandardDeviationWithAverage(session_gaze_values, session_average_gaze).ToString() + "\n";

        // write a new line to file
        string folder_path = save_folder_input.text;
        string raw_path = folder_path + "\\results.csv";
        WriteToFile(raw_path, line_str);

        // reset for next trial
        trial_dist_sum = 0;
        trial_gaze_sum = 0;
        trial_dist_values.Clear();
        trial_gaze_values.Clear();
    }

    private double GetMedian(List<Double> values)
    {
        double result;
        int count = values.Count();

        if (count == 0)
        {
            throw new Exception("tried to get median on an empty list");
        }

        if (count % 2 == 1)
        {
            result = values[count / 2];
        } else
        {
            result = (values[(count / 2) - 1] + values[count / 2]) / 2;
        }
        return result;
    }
    private double GetStandardDeviationWithAverage(List<Double> values, double average)
    {
        if (values.Count() == 0)
        {
            throw new Exception("tried to get standard deviation on an empty list");
        }

        double sum_of_squares = 0;
        foreach (double val in values)
        {
            sum_of_squares += Math.Pow(val - average, 2);
        }
        return Math.Sqrt(sum_of_squares / values.Count());
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
        RecordTrialResults();

        // only do these when recording starts
        if (current_trial_step == 0)
        {
            // record experiment starting time
            start_time = System.DateTime.Now;
            last_poll_time = start_time;
        }

        // NPC starts walking after trial step 1 (2 on the spec sheet)
        if (current_trial_step == 1)
        {
            control_ui.GetComponent<Control_UI_Helper>().toggleWalking();
        }

        last_trial_start_time = System.DateTime.Now;
        current_trial_step++;

        XRUI.GetComponent<XR_UI_Helper>().DisplayXRUIPanelFromTrialNumber(current_trial_step);
    }

    // record the remaining values for averages
    // does results need to be recorded?
    public void EndSession()
    {
        RecordAverages();
    }

    private double GetDistance()
    {
        Vector3 dist_vector = target_obj.position - user_cam_position.position;
        return (double) Math.Round(dist_vector.magnitude, digits_to_round);
    }
}
