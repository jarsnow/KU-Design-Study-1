using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class Disable_Mirroring : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        XRSettings.gameViewRenderMode = GameViewRenderMode.None;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
