using SFB;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OpenFile : MonoBehaviour
{

    // attach to the input field of the one with the button (SaveFilePath
    public TMP_InputField input;

    public void ButtonPressed()
    {
        // use file browser plugin to open the system default file browser, have the user select a folder
        string[] paths = StandaloneFileBrowser.OpenFolderPanel("Open Folder", "", false);
        // set text inside the input field to the selected file path
        input.text = paths[0];
    }
}
