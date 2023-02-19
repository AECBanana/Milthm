using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class PVController : MonoBehaviour
{
    [Serializable]
    public struct SongData{
        public string Artist;
        public string Title;
        public AudioClip Music;
        public Sprite Illustration;
        public bool Original;
    }
    [SerializeField]
    public List<SongData> songs;

    public AudioSource audioSource;
    public Image panel, oldPanel;
    public TMP_Text artist, title;
    public GameObject origin;
    private bool ends = false;
    private DateTime endTime;
    private int index = -1;

    private void Awake()
    {
        panel.sprite = songs[0].Illustration;
        ChangeIllustration();
        ChangeSong();
    }

    private void ChangeIllustration()
    {
        oldPanel.sprite = panel.sprite;
        panel.sprite = songs[index + 1].Illustration;
        if (index + 1 == 0) return;
        oldPanel.gameObject.SetActive(false);
        oldPanel.gameObject.SetActive(true);
    }
    
    private void ChangeSong()
    {
        index++;
        audioSource.clip = songs[index].Music;
        audioSource.Play();
        origin.gameObject.SetActive(false);
        origin.gameObject.SetActive(songs[index].Original);
    }

    private string AniStr(string str, double p)
    {
        int len = (int)(str.Length * (1 - p));
        string ret = "";
        for (int i = 0; i < len; i++)
        {
            if (i == len - 1)
            {
                ret += (char)((int)str[i] + Random.Range(-10, 10));
            }
            else
            {
                ret += str[i];
            }
        }

        return ret;
    }
    private void Update()
    {
        if (!audioSource.isPlaying && index < songs.Count - 1)
        {
            ChangeSong();
        }

        if (audioSource.time >= audioSource.clip.length - 1f && !ends && index < songs.Count - 1)
        {
            ends = true;
            if (index == songs.Count - 2)
                GetComponent<Animator>().Play("EndAni", 0, 0.0f);
            endTime = DateTime.Now;
            origin.GetComponent<Animator>().Play("OriginHide", 0, 0.0f);
            if (index != songs.Count - 2)
                ChangeIllustration();
        }

        if (ends)
        {
            double pro = Math.Min(1f, (DateTime.Now - endTime).TotalSeconds / 2f);
            if (pro <= 0.5)
            {
                artist.text = AniStr(songs[index].Artist, pro / 0.5);
                title.text = AniStr(songs[index].Title, pro / 0.5);
            }
            else
            {
                artist.text = AniStr(songs[index].Artist, 1 - (pro - 0.5) / 0.5);
                title.text = AniStr(songs[index].Title, 1 - (pro - 0.5) / 0.5);
            }

            if (pro >= 1)
                ends = false;
        }
        else
        {
            artist.text = songs[index].Artist;
            title.text = songs[index].Title;
        }
    }
}
