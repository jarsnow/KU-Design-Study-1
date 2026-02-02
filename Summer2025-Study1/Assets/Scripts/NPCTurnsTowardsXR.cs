using System.Collections;
using UnityEngine;

public class NPCTurnsTowardsXR : MonoBehaviour
{
    public GameObject NPC;
    public GameObject XR_User;

    public Control_UI_Helper control_ui;
    private float signedAngle;

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
        signedAngle = Vector3.SignedAngle(forward, targetDir, Vector3.up);

        // left
        Debug.Log(signedAngle);
        if (signedAngle > 0)
        {
            Debug.Log("left turn");
            control_ui.animator.SetTrigger("turnLeft");
        } else
        // right
        {
            Debug.Log("right turn");
            control_ui.animator.SetTrigger("turnRight");
        }

        // actually turn the NPC here
        TurnNPC();
    }

    private IEnumerable TurnNPC()
    {
        // turn over 1.5 seconds, step of .025 seconds
        const float total_time = 1.5f;
        const float time_step = .025f;
        int iters = (int)(total_time / time_step);
        for (int i = 0; i < iters; i++)
        {
            yield return new WaitForSeconds(.025f);
            float turn_step = signedAngle / iters;
            Vector3 rotation = new Vector3(0, turn_step, 0);
            Quaternion addedRotation = Quaternion.Euler(rotation);
            Debug.Log("turned");
            NPC.transform.rotation = addedRotation * NPC.transform.rotation;
        }
    }   
}
