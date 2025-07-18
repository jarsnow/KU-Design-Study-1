using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.UIElements;

public class Control_UI_Helper : MonoBehaviour
{
    public GameObject ControlUI;
    public GameObject ControlUIBackground;
    public GameObject MinimizeButtonCheckmark;

    public Animator animator;

    private AnimatorOverrideController overrider;

    private GameObject last_anim_progress_fill_bar;
    private float last_anim_progress_amount;
    public GameObject[] fill_bars;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        // code for animation progress
        //float anim_progress = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
        //RectTransform last_clicked_progress_bar = last_anim_progress_fill_bar.GetComponent<RectTransform>();
        //last_clicked_progress_bar.localScale = new Vector3(anim_progress, 1, 1);
    }

    public void recordFillBar(GameObject fill_bar)
    {
        last_anim_progress_fill_bar = fill_bar;
    }

    public void StartOneshotAnimation(AnimationClip anim)
    {
        overrider = new AnimatorOverrideController(animator.runtimeAnimatorController);
        animator.runtimeAnimatorController = overrider;

        // override active animation
        overrider["mixamo.com"] = anim;

        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Active Animation"))
        {
            animator.CrossFadeInFixedTime("Active Animation", 0.1f);
        }
    }

    // in order to toggle the menu, move the whole UI downwards by the size of the background
    public void ToggleMenu(bool toggle_state)
    {

        int height = (int) ControlUIBackground.GetComponent<RectTransform>().rect.height;

        if (toggle_state)
        {
            // move UI down
            Vector3 downwards_move = new Vector3(0, -height, 0);
            ControlUI.transform.Translate(downwards_move);

            // set checkmark z rotation to 180 (upside down)
            MinimizeButtonCheckmark.transform.eulerAngles = new Vector3(0, 0, 180);
        }
        else
        {
            // move UI up
            Vector3 upwards_move = new Vector3(0, height, 0);
            ControlUI.transform.Translate(upwards_move);

            // set checkmark z rotation to 0 (right-side up)
            MinimizeButtonCheckmark.transform.eulerAngles = new Vector3(0, 0, 0);
        }

    }
}
