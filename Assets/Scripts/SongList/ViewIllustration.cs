using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewIllustration : MonoBehaviour
{
    public Animator animator;
    public void Show()
    {
        animator.SetBool("MouseIn", true);
    }
    public void Hide()
    {
        animator.SetBool("MouseIn", false);
    }
}
