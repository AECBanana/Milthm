using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Video;

/// <summary>
/// 谱面加载器
/// </summary>
public class BeatmapLoader : MonoBehaviour
{
    public static BeatmapLoader Instance;
    /// <summary>
    /// 谱面延迟
    /// </summary>
    private static float Delay = 0f;
    public static float FlowSpeed = 1f;
    public static float Scale = 1f;
    
    public AudioSource Audio;
    public Image Bg1, Bg2;
    public static BeatmapModel Playing;
    public static string PlayingUID;
    public static int PlayingIndex;
    private static int TaskState;
    private readonly List<LineController> lines = new ();

    private void Start()
    {
        Instance = this;
        // 载入正在游玩的谱面，设置背景
        Load(PlayingUID, Playing);
        Bg1.sprite = SongResources.Illustration[PlayingUID][Playing.IllustrationFile];
        Bg2.sprite = Bg1.sprite;
        Audio.clip = SongResources.Songs[PlayingUID];
    }

    public void GenerateNote(GameObject notePrefab, int index, BeatmapModel.NoteData note, BeatmapModel map, string uid)
    {
        var snd = "";
        if (!GameSettings.NoCustomSnd)
            snd = note.Snd;
        var go = Instantiate(notePrefab, lines[note.Line].transform);
        var controller = go.GetComponent<HitObject>();
        controller.Index = index;
        controller.Snd = snd;
        controller.Line = lines[note.Line];
        controller.From = map.ToRealTime(note).Item1 + Delay;
        controller.To = map.ToRealTime(note).Item2 + Delay;
        var from = map.ToRealTime(note).Item1;
        lines[note.Line].HitObjects.Add(controller);
        go.SetActive(false);
        var noteController = controller;
        lines[note.Line].RemainingNote++;
        // 音效处理
        if (uid != "" && !string.IsNullOrEmpty(snd))
        {
            if (!SongResources.HitSnd.ContainsKey(note.Snd))
            {
                string f = "file:///" + SongResources.Path[uid].Replace("\\", "//") + "//" + note.Snd.Replace(" ", "%20").Replace("#", "%23");
                Debug.Log("Loading snd: " + f);
                string extension = Path.GetExtension(note.Snd).ToLower();
                AudioType type = AudioType.UNKNOWN;
                if (extension == ".mp3")
                    type = AudioType.MPEG;
                else if (extension == ".ogg")
                    type = AudioType.OGGVORBIS;
                else if (extension == ".wav")
                    type = AudioType.WAV;
                else if (extension == ".aiff")
                    type = AudioType.AIFF;

                TaskState++;

                if (type != AudioType.UNKNOWN)
                {
                    var handler = new DownloadHandlerAudioClip(f, type);
                    var request = new UnityWebRequest(f, "GET", handler, null);
                    request.disposeDownloadHandlerOnDispose = true;
                    request.timeout = 3;
                    SongResources.HitSnd.Add(note.Snd, null);
                    request.SendWebRequest().completed += (obj) =>
                    {
                        if (handler.isDone && handler.audioClip != null)
                            SongResources.HitSnd[note.Snd] = handler.audioClip;
                        else
                            Debug.Log("Unable to load: " + note.Snd);
                        TaskState--;
                    };
                }
            }
        }
        // 加入待击打列表
        if (HitJudge.HitList.Count == 0)
        {
            HitJudge.HitList.Add(new List<HitObject>() { noteController });
        }
        else
        {
            // 多押处理
            if (HitJudge.HitList[^1][0].From == from + Delay)
            {
                HitJudge.HitList[^1].Add(noteController);
            }
            else
            {
                HitJudge.HitList.Add(new List<HitObject>() { noteController });
            }
        }
    }

    public void LoadSettings(string uid, BeatmapModel map)
    {
        var cam = Camera.main.transform;
        Vector3 camRotation = cam.localEulerAngles, camPos = cam.localPosition;
        camRotation.x = Mods.Data[Mod.Mirror] && Mods.Data[Mod.Vertical] ? 180 : 0;
        camRotation.y = Mods.Data[Mod.Mirror] && !Mods.Data[Mod.Vertical] ? 180 : 0;
        camRotation.z = Mods.Data[Mod.Vertical] ? 90 : 0;
        camPos.z = Mods.Data[Mod.Mirror] ? 10 : -10;
        cam.localEulerAngles = camRotation;
        cam.localPosition = camPos;
        int range = PlayerPrefs.GetInt("JudgeRange", 1);
        HitJudge.JudgeRange = range;
        if (range == 0)
        {
            GameSettings.Perfect2 = 0.04f; GameSettings.Perfect = 0.08f; GameSettings.Good = 0.16f; GameSettings.Bad = 0.18f; 
        }
        else if (range == 1)
        {
            GameSettings.Perfect2 = 0.03f; GameSettings.Perfect = 0.06f; GameSettings.Good = 0.12f; GameSettings.Bad = 0.135f;
        }
        else if (range == 2)
        {
            GameSettings.Perfect2 = 0.02f; GameSettings.Perfect = 0.04f; GameSettings.Good = 0.09f; GameSettings.Bad = 0.1f;
        }
        GameSettings.NoCustomSnd = bool.Parse(PlayerPrefs.GetString("NoCustomSnd", "False"));
        if (uid != "")
            SongResources.HitSnd = new Dictionary<string, AudioClip>();
        if (GameSettings.NoCustomSnd || string.IsNullOrEmpty(map.SndSet))
            GameSettings.HitSnd = "milthm";
        else
            GameSettings.HitSnd = map.SndSet;
        GameSettings.NoPerfect = bool.Parse(PlayerPrefs.GetString("NoPerfect", "False"));
        HitJudge.JudgeMode = PlayerPrefs.GetInt("JudgeMode", Application.platform == RuntimePlatform.Android ? 1 : 0);
        //HitJudge.JudgeArea = Camera.main.ViewportToWorldPoint(new Vector3(180f / 1920f, 0, 0)).x - Camera.main.ViewportToWorldPoint(Vector3.zero).x;
        float flowspeed = PlayerPrefs.GetFloat("FlowSpeed", Application.platform == RuntimePlatform.Android ? 0.25f : 0.5f);
        Scale = PlayerPrefs.GetFloat("Scale", Application.platform == RuntimePlatform.Android ? 0.45f : 0.0f);
        FlowSpeed = Mathf.Pow(0.5f + flowspeed, 1.2f);
        Delay = PlayerPrefs.GetFloat("Delay") / 1000f;
    }
    
    /// <summary>
    /// 载入谱面
    /// </summary>
    /// <param name="map">谱面数据模型</param>
    public void Load(string uid, BeatmapModel map)
    {
        TaskState = 1;
        // 销毁已有轨道
        for(int j = 0;j < LineController.Lines.Count; j++)
        {
            if (LineController.Lines[j] != null)
                Destroy(LineController.Lines[j].gameObject);
        }
        LineController.Lines.Clear();
        LineController.UnhitLines.Clear();
        // 初始化
        LoadSettings(uid, map);
        Playing = map;
        GameObject line = Resources.Load<GameObject>("Line"),
                   tap = Resources.Load<GameObject>("Tap"),
                   hold = Resources.Load<GameObject>("Hold"),
                   drag = hold = Resources.Load<GameObject>("Drag");
        Audio.time = 0;
        GamePlayLoops.Instance.SummaryAni.Play("PlayEnterAni", 0, 0.0f);
        HitJudge.Result = new HitJudge.ResultData
        {
            FullCombo = map.NoteList.Count
        };
        HitJudge.HitList = new List<List<HitObject>>();
        HitJudge.CaptureOnce = new List<int>();
        HitJudge.BindNotes = new Dictionary<int, HitObject>();
        for (KeyCode key = KeyCode.A; key <= KeyCode.Z; key++)
            HitJudge.BindNotes.Add((int)key, null);
        lines.Clear();
        // 生成轨道
        float orSpace;
        if (Mods.Data[Mod.Vertical])
            orSpace = 1200f / (map.LineList.Count - 1);
        else
            orSpace = 200f + Mathf.Max(1f - (map.LineList.Count - 4.0f) / 4.0f, 0f) * 100f;
        GameSettings.ScreenW = Mathf.Abs(Camera.main.ViewportToWorldPoint(Vector3.one).x - Camera.main.ViewportToWorldPoint(Vector3.zero).x);
        GameSettings.ScreenH = Mathf.Abs(Camera.main.ViewportToWorldPoint(Vector3.one).y - Camera.main.ViewportToWorldPoint(Vector3.zero).y);
        GameSettings.WFactor = GameSettings.ScreenW / 1920f;
        GameSettings.HFactor = GameSettings.ScreenH / 1080f;
        Debug.Log("Play Screen Size: " + GameSettings.ScreenW + "x" + GameSettings.ScreenH);
        float space = orSpace * GameSettings.WFactor;
        float x = -space * (map.LineList.Count - 1) / 2;
        foreach (BeatmapModel.LineData l in map.LineList)
        {
            GameObject go = Instantiate(line);
            LineController controller = go.GetComponent<LineController>();
            controller.FlowSpeed = l.FlowSpeed * 0.9f;
            controller.Direction = l.Direction;
            controller.Index = lines.Count;
            if (l.Direction == BeatmapModel.LineDirection.Right)
                go.transform.localEulerAngles = new Vector3(0, 0, 0);
            else if (l.Direction == BeatmapModel.LineDirection.Up)
                go.transform.localEulerAngles = new Vector3(0, 0, 90);
            else if (l.Direction == BeatmapModel.LineDirection.Left)
                go.transform.localEulerAngles = new Vector3(0, 0, 180);
            else if (l.Direction == BeatmapModel.LineDirection.Down)
                go.transform.localEulerAngles = new Vector3(0, 0, 270);

            go.transform.localEulerAngles = new Vector3(0, 0, 90);
            go.transform.localPosition = new Vector3(x, -350 * GameSettings.HFactor, 0);
            x += space;

            go.transform.localScale = new Vector3(0.3f * (1.0f + Scale), 0.3f * (1.0f + Scale), 0.3f * (1.0f + Scale));
            go.SetActive(true);
            lines.Add(controller);
            LineController.Lines.Add(controller);
        }
        // 载入所有note
        Delay -= map.SongOffset;
        int i = 0;
        foreach (var note in map.NoteList)
        {
            if (Mods.Data[Mod.Bed])
            {
                BeatmapModel.NoteData tNote = new()
                {
                    BPM = note.BPM,
                    From = note.From,
                    To = note.From,
                    Line = note.Line,
                    Type = 1
                };
                GenerateNote(drag, i, tNote, map, uid);
                i++;
                continue;
            }
            var notePrefab = note.Type switch
            {
                0 => (note.FromBeat == note.ToBeat ? hold : tap),
                1 => drag,
                _ => null
            };
            GenerateNote(notePrefab, i, note, map, uid);
            i++;
        }

        // 根据轨道序号排序击打列表
        foreach (var t in HitJudge.HitList)
        {
            t.Sort((x, y) => x.Line.Index.CompareTo(y.Line.Index));
            if (t.Count > 1)
            {
                foreach (var note in t)
                {
                    note.GetComponent<SpriteRenderer>().sprite = note.DoubleSprite;
                }
            }
        }

        Debug.Log("待击打列表：" + HitJudge.HitList.Count);

        TaskState--;

        Audio.time = 0;
        AudioUpdate.StartTime = System.DateTime.Now;
        AudioUpdate.Started = false;
        Audio.Stop();

        GamePlayLoops.Instance.SummaryAni.Play("PlayEnterAni", 0, 0.0f);
    }

    private void Update()
    {
        if (TaskState == 0)
            return;
        Audio.time = 0;
        AudioUpdate.StartTime = System.DateTime.Now;
        AudioUpdate.Started = false;
        Audio.Stop();
    }
}
