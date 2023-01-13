using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class SongListLoader : MonoBehaviour
{
    public enum LoadStatus
    {
        Success, Duplicated, NotSupported, Failed
    }
    public static SongListLoader Instance;

    public static LoadStatus Load(string path, Action<string> LoadComplete)
    {
        List<BeatmapModel> maps = new List<BeatmapModel>();
        
        foreach(string file in Directory.GetFiles(path))
        {
            if (Path.GetExtension(file).ToLower() == ".milthm")
            {
                BeatmapModel map = BeatmapModel.Read(file);
                maps.Add(map);
            }
        }

        if (maps.Count == 0)
            return LoadStatus.Failed;

        string uid = maps[0].BeatmapUID;

        if (SongResources.Beatmaps.ContainsKey(uid))
        {
            return LoadStatus.Duplicated;
        }

        SongResources.Path.Add(uid, path);
        SongResources.Beatmaps.Add(uid, maps);
        SongResources.Illustration.Add(uid, new Dictionary<string, Sprite>());

        // ÔØÈëÇú»æ
        foreach(BeatmapModel map in maps)
        {
            if (!SongResources.Illustration[uid].ContainsKey(map.IllustrationFile))
            {
                SongResources.Illustration[uid].Add(map.IllustrationFile, null);
                string file = "file:///" + path.Replace("\\", "//") + "//" + map.IllustrationFile;
                var handler = new DownloadHandlerTexture();
                var request = new UnityWebRequest(file, "GET", handler, null);
                request.SendWebRequest().completed += (obj) =>
                {
                    if (handler.texture != null)
                    {
                        Sprite sprite = Sprite.Create(handler.texture, new Rect(0, 0, handler.texture.width, handler.texture.height), Vector2.one * 0.5f);
                        SongResources.Illustration[uid][map.IllustrationFile] = sprite;
                    }
                    else
                    {
                        Debug.Log("µ¼ÈëÇú»æÊ§°Ü£º" + file);
                    }
                    if (LoadComplete != null && !SongResources.Illustration[uid].Values.ToList().Contains(null))
                        LoadComplete(uid);
                };
            }
        }

        return LoadStatus.Success;
    }

    public GameObject SongItem, DragTip;

    private void Awake()
    {
        Instance = this;
        foreach(string dir in Directory.GetDirectories(SongResources.DataPath))
        {
            if (!dir.EndsWith(".") && !dir.EndsWith(".."))
            {
                Load(dir, LoadToUI);
            }
        }
    }

    public void LoadToUI(string uid)
        => LoadToUI(uid, false);

    public void LoadToUIWithAni(string uid)
        => LoadToUI(uid, true);

    public void LoadToUI(string uid, bool playAni)
    {
        GameObject item = Instantiate(SongItem, SongItem.transform.parent);
        SongItemController controller = item.GetComponent<SongItemController>();
        controller.Beatmap = uid;
        int lastPlay = PlayerPrefs.GetInt(uid + ".lastPlay");
        if (lastPlay < 0 || lastPlay >= SongResources.Illustration[uid].Count) lastPlay = 0;
        BeatmapModel map = SongResources.Beatmaps[uid][lastPlay];
        controller.Illustration.sprite = SongResources.Illustration[uid][map.IllustrationFile];
        controller.Description.text = map.Title + " - " + map.Beatmapper;
        DragTip.transform.SetAsLastSibling();
        item.SetActive(true);
        if (playAni)
            item.GetComponent<Animator>().Play("SongItemShow", 0, 0.0f);
    }

    private void OnDestroy()
    {
        Instance = null;
    }
}
