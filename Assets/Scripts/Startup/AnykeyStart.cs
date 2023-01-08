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
            played = false;
            animator.Play("StartGame", 0, 0.0f);
        }
    }
    public void LoadFirstScene()
    {
        Loading.Run(FirstScene);
    }
}
