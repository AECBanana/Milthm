using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SongPreviewController : MonoBehaviour
{
    public static string LastBeatmap;
    public static SongPreviewController Instance;
    public GameObject DifficultyPrefab;
    public Animator PreviewPanel;
    public TMP_Text Title, Description, From;
    public Image Illustration, Background, FakeCover;
    public AudioSource BGM;
    public AudioHighPassFilter Filter;
    public SongItemController SongItem;
    public static bool LeftBtn = false, RightBtn = false;

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
        if (Input.GetKeyUp(KeyCode.LeftArrow) || LeftBtn)
            s = 1;
        else if (Input.GetKeyUp(KeyCode.RightArrow) || RightBtn)
            s = 2;
        if (s > 0)
        {
            LeftBtn = false; RightBtn = false;
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
        List<BeatmapModel> maps = new List<BeatmapModel>();
        foreach(BeatmapModel map in SongResources.Beatmaps[uid])
        {
            maps.Add(map);
        }
        maps.Sort((x, y) => x.NoteList.Count.CompareTo(y.NoteList.Count));
        foreach (BeatmapModel map in maps)
        {
            GameObject go = Instantiate(DifficultyPrefab, scroll);
            DifficultyController controller = go.GetComponent<DifficultyController>();
            controller.uid = uid;
            controller.index = SongResources.Beatmaps[uid].FindIndex(x => x == map);
            controller.Title.text = map.Difficulty + (map.DifficultyValue == -1f ? "" : " (" + map.DifficultyValue + ")");
            go.SetActive(true);
            list.Add(controller);
        }

        int lastPlay = PlayerPrefs.GetInt(uid + ".lastPlay");
        if (lastPlay < 0 || lastPlay >= SongResources.Beatmaps[uid].Count) lastPlay = 0;

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
