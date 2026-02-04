using System.Collections;
using UnityEngine;

public class NPCTurnsTowardsXR : MonoBehaviour
{
    public GameObject NPC;
    public Transform target;

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
        control_ui.animator.SetTrigger("turnLeft");

        StartCoroutine(TurnNPC());
    }

    private IEnumerator TurnNPC()
    {
        yield return new WaitForSeconds(1);
    }   
}
