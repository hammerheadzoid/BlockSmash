using UnityEngine;
using System.Collections;

public class PanelScrollControl : MonoBehaviour {

    private bool currentstate = false;

    public void togglepanelslide(Animator anim) {
        if (currentstate == false)
        {
            currentstate = true;
            anim.SetBool("isDisplayed", currentstate);
        }
        else {
            currentstate = false;
            anim.SetBool("isDisplayed", currentstate);
        }
    }
}
