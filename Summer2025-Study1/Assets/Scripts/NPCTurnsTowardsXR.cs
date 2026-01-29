using System.Collections;
using UnityEngine;

public class NPCTurnsTowardsXR : MonoBehaviour
{
    public GameObject NPC;
    public GameObject XR_User;

    public Control_UI_Helper control_ui;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void TurnNPCToXR()
    {
        //Debug.Log(NPC.transform.rotation.eulerAngles);
        //float angle = Quaternion.Angle(NPC.transform.rotation, XR_User.transform.rotation);

        Vector3 targetDir = XR_User.transform.position - transform.position;
        Vector3 forward = transform.forward;

        // get angle for npc to turn towards xr user
        float signedAngle = Vector3.SignedAngle(forward, targetDir, Vector3.up);

        // left
        if (signedAngle < 0)
        {
            control_ui.animator.SetTrigger("turnLeft");
        } else
        // right
        {
            control_ui.animator.SetTrigger("turnRight");
        }

        // actually turn the NPC here

    }

    private IEnumerable TurnNPC()
    {
        
    }

}
