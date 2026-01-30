using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
//using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.UIElements;

public class Control_UI_Helper : MonoBehaviour
{
    public GameObject ControlUI;
    public GameObject ControlUIBackground;
    public GameObject MinimizeButtonCheckmark;

    public Animator animator;
    public Animator female_animator;
    public Animator male_animator;

    public GameObject advance_trial_popup;
    public GameObject save_and_exit_popup;

    public GameObject supportive_anims;
    public GameObject unsupportive_anims;

    public AnimationClip[] extra_idle_anims;
    private List<AnimationClip> extra_idle_anims_shuffled;
    private DateTime last_extra_anim_time = DateTime.Now;

    public GameObject IO_Helper_obj;

    public CinemachineDollyCart path_follower;
    public GameObject NPC;
    public CinemachinePath path;
    private bool is_done_walking = false;
    private bool has_turned_around = false;

    private AnimatorOverrideController overrider;

    private bool isMenuMinimized = false;

    // Start is called before the first frame update
    void Start()
    {
        reshuffle_extra_idle_anims();
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
            animator.SetBool("isWalking", false);
            path_follower.m_Speed = 0;
            // advance trial step
            IO_Helper_obj.GetComponent<IO_Helper>().AdvanceTrial();
        }

        // turn around but only once, after done with the animation
        if (is_done_walking && animator.GetCurrentAnimatorStateInfo(0).IsName("Idle") && !has_turned_around)
        {
            NPC.transform.Rotate(0, 180, 0);
            has_turned_around = true;
        }

        // adjust speed based on whether or not the agent needs to walk
        if (animator.GetBool("isWalking") && animator.GetCurrentAnimatorStateInfo(0).IsName("Walking"))
        {
            path_follower.m_Speed = 0.75f;
        }

        // make the npc do extra random actions while they're just standing around
        if (IO_Helper_obj.GetComponent<IO_Helper>().current_trial_step <= 2 &&
            animator.GetCurrentAnimatorStateInfo(0).IsName("Idle") &&
            (DateTime.Now - last_extra_anim_time) >= TimeSpan.FromSeconds(6.0f)) {


            if (extra_idle_anims_shuffled.Count == 0)
            {
                reshuffle_extra_idle_anims();
            }
            AnimationClip random_anim = extra_idle_anims_shuffled[0];
            extra_idle_anims_shuffled.RemoveAt(0);

            StartOneshotAnimation(random_anim);
            last_extra_anim_time = DateTime.Now;
        }
    }

    void reshuffle_extra_idle_anims()
    {
        extra_idle_anims_shuffled = extra_idle_anims.ToList<AnimationClip>();
        for (int i = extra_idle_anims_shuffled.Count() - 1; i >= 1; i--)
        {
            int j = UnityEngine.Random.Range(0, i);
            AnimationClip temp = extra_idle_anims_shuffled[i];
            extra_idle_anims_shuffled[i] = extra_idle_anims_shuffled[j];
            extra_idle_anims_shuffled[j] = temp;
        }
    }

    public void toggleWalking()
    {
        // toggle animation
        bool isCurrWalking = animator.GetBool("isWalking");
        animator.SetBool("isWalking", !isCurrWalking);
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
    void OnToggleMenu(bool state)
    {
        ToggleMenu();
    }

    // in order to toggle the menu, move the whole UI downwards by the size of the background
    public void ToggleMenu()
    {
        // canvas scale in both UI parents
        float scale = 1.5f;
        float height = 0.25f;
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
