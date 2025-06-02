using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Walk_Test : MonoBehaviour
{
    // set to whatever the name in the animator is
    const string isWalking = "isWalking";

    public Animator animator;
    public CinemachineDollyCart path_follower;
    // Start is called before the first frame update
    void Start()
    {
        // don't move, so set walking to false and walking speed to zero
        animator.SetBool(isWalking, false);
        path_follower.m_Speed = 0;
    }

    // Update is called once per frame
    void Update()
    {
        // TESTING
        // toggle walking / idle on space input
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // toggle animation
            bool isCurrWalking = animator.GetBool(isWalking);
            animator.SetBool(isWalking, !isCurrWalking);
            // toggle speed to 0/1 (might seem odd because isCurrWalking was before it was toggled)
            path_follower.m_Speed = isCurrWalking ? 0 : 1;
        }
    }
}
