using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using static Unity.VisualScripting.Member;

public class SongPreviewController : MonoBehaviour
{
    public static string LastBeatmap;
    public static SongPreviewController Instance;
    public GameObject DifficultyPrefab;
    public Animator PreviewPanel;
    public Text Title, Description, From;
    public Image Illustration, Background, FakeCover;
    public AudioSource BGM;
    public AudioHighPassFilter Filter;
    public SongItemController SongItem;

    private void Awake()
    {
        Instance = this;
    }

    public void Hide()
    {
        Filter.enabled = true;
    }

    public void Update()
    {
        int s = 0;
        if (Input.GetKeyUp(KeyCode.LeftArrow))
            s = 1;
        else if (Input.GetKeyUp(KeyCode.RightArrow))
            s = 2;
        if (s > 0)
        {
            FakeCover.sprite = Background.sprite;
            FakeCover.gameObject.SetActive(false);
            FakeCover.gameObject.SetActive(true);
            GameObject go = Instantiate(PreviewPanel.gameObject, PreviewPanel.transform.parent);
            go.SetActive(true);
            Animator ani = go.GetComponent<Animator>();
            if (s == 1)
            {
                Show(SongItem.PreSong.Beatmap);
                SongItem = SongItem.PreSong;
                ani.Play("SongToRight", 0, 0.0f);
                PreviewPanel.Play("SongFromLeft", 0, 0.0f);
            }
            else if (s == 2)
            {
                Show(SongItem.NextSong.Beatmap);
                SongItem = SongItem.NextSong;
                ani.Play("SongToLeft", 0, 0.0f);
                PreviewPanel.Play("SongFromRight", 0, 0.0f);
            }
        }
    }

    public void Show(string uid)
    {
        Transform scroll = DifficultyPrefab.transform.parent;
        for (int i = 0; i < scroll.childCount; i++)
        {
            if (scroll.GetChild(i).gameObject.activeSelf)
                Destroy(scroll.GetChild(i).gameObject);
        }
        List<DifficultyController> list = new List<DifficultyController>();
        foreach(BeatmapModel map in SongResources.Beatmaps[uid])
        {
            GameObject go = Instantiate(DifficultyPrefab, scroll);
            DifficultyController controller = go.GetComponent<DifficultyController>();
            if (PlayerPrefs.GetString(uid + ".grade") == "")
            {
                controller.Grade.gameObject.SetActive(false);
                controller.Score.gameObject.SetActive(false);
                controller.Accuracy.gameObject.SetActive(false);
            }
            else
            {

            }
            controller.uid = uid;
            controller.index = list.Count;
            controller.Title.text = map.Difficulty + (map.DifficultyValue == -1f ? "" : " (" + map.DifficultyValue + ")");
            go.SetActive(true);
            list.Add(controller);
        }

        int lastPlay = PlayerPrefs.GetInt(uid + ".lastPlay");
        if (lastPlay < 0 || lastPlay >= SongResources.Illustration[uid].Count) lastPlay = 0;

        list[lastPlay].Back.sprite = list[lastPlay].ActiveSprite;
        DifficultyController.Active = list[lastPlay];

        BeatmapModel m = SongResources.Beatmaps[uid][lastPlay];

        Illustration.sprite = SongResources.Illustration[uid][m.IllustrationFile];
        Background.sprite = Illustration.sprite;

        Title.text = m.Title;
        Description.text = "[Çú]" + m.Composer + " [Æ×]" + m.Beatmapper + " [ÃÀ]" + m.Illustrator;

        BGM.clip = SongResources.Songs[uid];
        if (m.PreviewTime != -1)
            BGM.time = m.PreviewTime;
        BGM.Play();


        Filter.enabled = false;

        string source = "";

        if (m.Source != "")
            source += " / " + m.Source;
        if (m.GameSource != "")
            source += " / " + m.GameSource;

        if (source.Length > 0)
        {
            From.text = "From " + source.Substring(3);
        }
        else
        {
            From.text = "";
        }

        gameObject.SetActive(true);
    }
}
