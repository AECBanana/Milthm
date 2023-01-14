using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SongItemController : MonoBehaviour
{
    public string Beatmap;
    public Image Illustration;
    public Text Description;
    public SongPreviewController Preview;
    public SongItemController NextSong;
    public SongItemController PreSong;
    Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void Touch()
    {
        Preview.SongItem = this;
        Preview.Show(Beatmap);
    }

    public void MouseIn()
    {
        animator.SetBool("MouseIn", true);
    }

    public void MouseOut()
    {
        animator.SetBool("MouseIn", false);
    }
}
