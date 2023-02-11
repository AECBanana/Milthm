using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
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
    bool pressing = false;
    DateTime pressTime = DateTime.MinValue;

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
            NextSong.PreSong = PreSong;
            PreSong.NextSong = NextSong;
        }
        catch
        {
            DialogController.Show("É¾³ýÊ§°Ü£¡", "Æ×ÃæÉ¾³ýÊ§°ÜÁË£¬ÎØÎØ¡£");
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
        pressing = false;
        if ((DateTime.Now - pressTime).TotalSeconds >= 0.5)
        {
            DeleteMode();
            pressTime = DateTime.MinValue;
            return;
        }
        if (pressTime == DateTime.MinValue)
        {
            return;
        }
        Preview.SongItem = this;
        Preview.Show(Beatmap);
        pressTime = DateTime.MinValue;
        PlayerPrefs.SetString("LastSong", Beatmap);
    }

    public void TouchDown()
    {
        if (pressTime == DateTime.MinValue)
        {
            pressTime = DateTime.Now;
            if (Input.GetMouseButton(1))
                pressTime -= TimeSpan.FromMinutes(1);
        }
        pressing = true;
        if ((DateTime.Now - pressTime).TotalSeconds >= 0.5 && !RemoveBtn.activeSelf)
        {
            Debug.Log("Occurred.");
            DeleteMode();
            pressTime = DateTime.MinValue;
            return;
        }
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
        if (RemoveBtn.activeSelf && Input.anyKey && EventSystem.current.currentSelectedGameObject != RemoveBtn && !pressing)
        {
            RemoveBtn.SetActive(false);
            animator.SetBool("MouseIn", false);
            animator.SetBool("DeleteMode", false);
        }
    }
}
