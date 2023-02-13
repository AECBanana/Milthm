using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

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
    public Animator animator;

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

    private void Update()
    {
        if (RemoveBtn.activeSelf && Input.anyKey && EventSystem.current.currentSelectedGameObject != RemoveBtn)
        {
            RemoveBtn.SetActive(false);
            animator.SetBool("MouseIn", false);
            animator.SetBool("DeleteMode", false);
        }
    }
}
