using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SongItemController : MonoBehaviour
{
    public string Beatmap;
    public Image Illustration;
    public TMP_Text Description, Beatmapper, Count;
    public SongPreviewController Preview;
    public SongItemController NextSong;
    public SongItemController PreSong;
    public GameObject OutDate;
    public GameObject RemoveBtn;
    Animator animator;
    DateTime pressTime;

    public void DeleteSong()
    {
        try
        {
            Directory.Delete(SongResources.Path[Beatmap], true);
            animator.Play("SongItemDelete", 0, 0.0f);
            SongResources.Path.Remove(Beatmap);
            SongResources.Songs.Remove(Beatmap);
            SongResources.Illustration.Remove(Beatmap);
            SongResources.Beatmaps.Remove(Beatmap);
        }
        catch
        {
            DialogController.Show("ɾ��ʧ�ܣ�", "����ɾ��ʧ���ˣ����ء�");
        }
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void DeleteMode()
    {
        RemoveBtn.SetActive(true);
        animator.SetBool("DeleteMode", true);
    }

    public void TouchUp()
    {
        if ((DateTime.Now - pressTime).TotalSeconds >= 0.5)
        {
            DeleteMode();
            return;
        }
        Preview.SongItem = this;
        Preview.Show(Beatmap);
    }

    public void TouchDown()
    {
        pressTime = DateTime.Now;
        if (Input.GetMouseButton(1))
            pressTime -= TimeSpan.FromMinutes(1);
    }

    public void MouseIn()
    {
        if (RemoveBtn.activeSelf)
            return;
        animator.SetBool("MouseIn", true);
    }

    public void MouseOut()
    {
        if (RemoveBtn.activeSelf)
            return;
        animator.SetBool("MouseIn", false);
    }

    private void Update()
    {
        if (RemoveBtn.activeSelf && Input.GetMouseButtonUp(0))
        {
            RemoveBtn.SetActive(false);
            animator.SetBool("MouseIn", false);
            animator.SetBool("DeleteMode", false);
        }
    }
}
