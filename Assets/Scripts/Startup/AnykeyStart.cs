using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnykeyStart : MonoBehaviour
{
    public Animator animator;
    public string FirstScene;
    bool played = false;
    void Update()
    {
        if (played) return;
        if (Input.GetMouseButton(0))
        {
            SndPlayer.Play("UI_Buttons_Pack2\\Button_2_Pack2");
            played = true;
            animator.Play("StartGame", 0, 0.0f);
        }
    }
    public void LoadFirstScene()
    {
        Loading.Run(FirstScene);
    }
}
