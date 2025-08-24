using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
//using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.UIElements;

public class Control_UI_Helper : MonoBehaviour
{
    public GameObject ControlUI;
    public GameObject ControlUIBackground;
    public GameObject MinimizeButtonCheckmark;

    private Animator animator;
    public Animator female_animator;
    public Animator male_animator;

    public GameObject advance_trial_popup;
    public GameObject save_and_exit_popup;

    public GameObject supportive_anims;
    public GameObject unsupportive_anims;

    public GameObject IO_Helper_obj;

    public CinemachineDollyCart path_follower;
    public CinemachinePath path;
    private bool is_done_walking = false;

    private AnimatorOverrideController overrider;

    private bool isMenuMinimized = false;

    // Start is called before the first frame update
    void Start()
    {
        animator = female_animator;
    }

    // Update is called once per frame
    void Update()
    {
        // check if the NPC needs to stop walking
        float path_progress = path_follower.m_Position / path.PathLength;
        if(path_progress >= .99999 && !is_done_walking)
        {
            is_done_walking = true;
            toggleWalking();
            // advance trial step
            IO_Helper_obj.GetComponent<IO_Helper>().AdvanceTrial();
        }
    }
    public void toggleWalking()
    {
        // toggle animation
        bool isCurrWalking = animator.GetBool("isWalking");
        animator.SetBool("isWalking", !isCurrWalking);
        // toggle speed to 0/1 (might seem odd because isCurrWalking was before it was toggled)
        path_follower.m_Speed = isCurrWalking ? 0 : 1.5f;
    }

    public void updateActiveNPCModel(string npc_gender)
    {
        if (npc_gender == "female")
        {
            animator = female_animator;
        } else
        {
            animator = male_animator;
        }
    }

    public void updateAnimationMenu(string supportive_state)
    {
        if (supportive_state == "supportive")
        {
            supportive_anims.SetActive(true);
            unsupportive_anims.SetActive(false);
        }
        else
        {
            supportive_anims.SetActive(false);
            unsupportive_anims.SetActive(true);
        }
    }

    public void StartOneshotAnimation(AnimationClip anim)
    {
        overrider = new AnimatorOverrideController(animator.runtimeAnimatorController);
        animator.runtimeAnimatorController = overrider;

        // override active animation
        overrider["mixamo.com"] = anim;

        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Active Animation"))
        {
            animator.CrossFadeInFixedTime("Active Animation", 0.4f);
        }
    }

    // function referenced by input action system
    void OnToggleMenu()
    {
        ToggleMenu();
    }

    // in order to toggle the menu, move the whole UI downwards by the size of the background
    public void ToggleMenu()
    {
        // canvas scale in both UI parents
        float scale = 1.5f;
        int height = (int) ControlUIBackground.GetComponent<RectTransform>().rect.height;
        if (!isMenuMinimized)
        {
            // move UI down
            Vector3 downwards_move = new Vector3(0, -height * scale, 0);
            ControlUI.transform.Translate(downwards_move);

            // set checkmark z rotation to 180 (upside down)
            MinimizeButtonCheckmark.transform.eulerAngles = new Vector3(0, 0, 180);
        }
        else
        {
            // move UI up
            Vector3 upwards_move = new Vector3(0, height * scale, 0);
            ControlUI.transform.Translate(upwards_move);

            // set checkmark z rotation to 0 (right-side up)
            MinimizeButtonCheckmark.transform.eulerAngles = new Vector3(0, 0, 0);
        }

        isMenuMinimized = !isMenuMinimized;
    }

    public void AdvanceTrialPopupPressed()
    {
        advance_trial_popup.SetActive(false);
    }
    public void SaveAndExitPopupPressed(bool wasYesPressed)
    {
        save_and_exit_popup.SetActive(false);

        if (wasYesPressed)
        {
            IO_Helper_obj.GetComponent<IO_Helper>().EndSession();
            // quit when running the real thing
            Application.Quit();
            // quit when in play mode (editor)
            //EditorApplication.isPlaying = false;
        }
    }

    public void ShowPopup(GameObject popup_to_show)
    {
        popup_to_show.SetActive(true);
    }

}
