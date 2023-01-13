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
    public Text Title, Description, From;
    public Image Illustration;
    public AudioSource BGM;
    public AudioHighPassFilter Filter;
    AudioClip clip;

    private void Awake()
    {
        Instance = this;
    }

    public void Hide()
    {
        Filter.enabled = true;
    }

    public void Show(string uid)
    {
        if (clip != null && LastBeatmap != uid)
        {
            BGM.clip = null;
            clip.UnloadAudioData();
            clip = null;
        }
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

        Title.text = m.Title;
        Description.text = "ÒôÀÖ£º" + m.Composer + " Æ×Ãæ£º" + m.Beatmapper + " Çú»æ£º" + m.Illustrator;

        string file = "file:///" + SongResources.Path[uid].Replace("\\", "//") + "//" + m.AudioFile;
        string extension = Path.GetExtension(m.AudioFile).ToLower();
        AudioType type = AudioType.UNKNOWN;
        if (extension == ".mp3")
            type = AudioType.MPEG;
        else if (extension == ".ogg")
            type = AudioType.OGGVORBIS;
        else if (extension == ".wav")
            type = AudioType.WAV;
        else if (extension == ".aiff")
            type = AudioType.AIFF;

        if (type != AudioType.UNKNOWN && clip == null)
        {
            var handler = new DownloadHandlerAudioClip(file, type);
            var request = new UnityWebRequest(file, "GET", handler, null);
            request.SendWebRequest().completed += (obj) =>
            {
                if (handler.audioClip != null)
                {
                    clip = handler.audioClip;
                    BGM.clip = clip;
                    BGM.Play();
                    if (m.PreviewTime != -1)
                        BGM.time = m.PreviewTime;
                }
            };
        }

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
