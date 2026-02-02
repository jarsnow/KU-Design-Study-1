using UnityEngine;

public class SetUIActive : MonoBehaviour
{

    public GameObject StartUI;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartUI.SetActive(true);
        Debug.developerConsoleEnabled = true;
        Debug.developerConsoleVisible = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
