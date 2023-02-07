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
/// ���������
/// </summary>
public class BeatmapLoader : MonoBehaviour
{
    public static float FlowSpeed;
    public static BeatmapLoader Instance;
    /// <summary>
    /// �����ӳ�
    /// </summary>
    public static float Delay = 0f;
    public AudioSource Audio;
    public Image Bg1, Bg2;
    public static BeatmapModel Playing;
    public static string PlayingUID;
    public static int PlayingIndex;
    public static int TaskState;
    List<LineController> lines = new List<LineController>();

    private void Start()
    {
        Instance = this;
        // ����������������棬���ñ���
        Load(PlayingUID, Playing);
        Bg1.sprite = SongResources.Illustration[PlayingUID][Playing.IllustrationFile];
        Bg2.sprite = Bg1.sprite;
        Audio.clip = SongResources.Songs[PlayingUID];
    }

    /// <summary>
    /// ȡ��note���ڹ�����
    /// </summary>
    /// <param name="note">note</param>
    /// <returns>������</returns>
    public int GetLineIndex(MonoBehaviour note)
    {
        if (note is TapController tap)
        {
            return tap.Line.Index;
        }
        else if (note is HoldController hold)
        {
            return hold.Line.Index;
        }
        return 0;
    }

    /// <summary>
    /// ��������
    /// </summary>
    /// <param name="map">��������ģ��</param>
    public void Load(string uid, BeatmapModel map)
    {
        TaskState = 1;
        // �������й��
        for(int j = 0;j < LineController.Lines.Count; j++)
        {
            if (LineController.Lines[j] != null)
                Destroy(LineController.Lines[j].gameObject);
        }
        LineController.Lines.Clear();
        LineController.UnhitLines.Clear();
        // ��ʼ��
        GameSettings.NoCustomSnd = bool.Parse(PlayerPrefs.GetString("NoCustomSnd", "False"));
        if (uid != "")
            SongResources.HitSnd = new Dictionary<string, AudioClip>();
        if (GameSettings.NoCustomSnd || string.IsNullOrEmpty(map.SndSet))
            GameSettings.HitSnd = "milthm";
        else
            GameSettings.HitSnd = map.SndSet;
        GameSettings.NoPerfect = bool.Parse(PlayerPrefs.GetString("NoPerfect", "False"));
        HitJudge.NoDead = bool.Parse(PlayerPrefs.GetString("NoDead", "False"));
        HitJudge.JudgeMode = PlayerPrefs.GetInt("JudgeMode", Application.platform == RuntimePlatform.Android ? 1 : 0);
        HitJudge.JudgeArea = Camera.main.ViewportToWorldPoint(new Vector3(180f / 1920f, 0, 0)).x - Camera.main.ViewportToWorldPoint(Vector3.zero).x;
        float flowspeed = PlayerPrefs.GetFloat("FlowSpeed", Application.platform == RuntimePlatform.Android ? 0.25f : 0.5f),
              scale = PlayerPrefs.GetFloat("Scale", Application.platform == RuntimePlatform.Android ? 0.45f : 0.0f);
        FlowSpeed = Mathf.Pow(0.5f + flowspeed, 1.2f);
        Playing = map;
        Delay = PlayerPrefs.GetFloat("Delay") / 1000f;
        DebugInfo.Output("Delay", Delay.ToString() + "s");
        GameObject line = Resources.Load<GameObject>("Line"),
                   tap = Resources.Load<GameObject>("Tap"),
                   hold = Resources.Load<GameObject>("Hold");
        Audio.time = 0;
        GamePlayLoops.Instance.SummaryAni.Play("PlayEnterAni", 0, 0.0f);
        HitJudge.Result = new HitJudge.ResultData
        {
            FullCombo = map.NoteList.Count
        };
        HitJudge.HitList = new List<List<MonoBehaviour>>();
        HitJudge.CaptureOnce = new List<int>();
        HitJudge.BindNotes = new Dictionary<int, MonoBehaviour>();
        for (KeyCode key = KeyCode.A; key <= KeyCode.Z; key++)
            HitJudge.BindNotes.Add((int)key, null);
        lines.Clear();
        // ���ɹ��
        float orSpace = 200f + Mathf.Max(1f - (map.LineList.Count - 4.0f) / 4.0f, 0f) * 100f;
        float space = Camera.main.ViewportToWorldPoint(new Vector3(orSpace / 1920f, 0, 0)).x - Camera.main.ViewportToWorldPoint(Vector3.zero).x;
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
            go.transform.localPosition = new Vector3(x, -3.5f, 0);
            x += space;

            go.transform.localScale = new Vector3(0.3f * (1.0f + scale), 0.3f * (1.0f + scale), 0.3f * (1.0f + scale));
            go.SetActive(true);
            lines.Add(controller);
            LineController.Lines.Add(controller);
        }
        // ��������note
        int i = 0;
        foreach (BeatmapModel.NoteData note in map.NoteList)
        {
            float from; MonoBehaviour noteController;
            string snd = "";
            if (!GameSettings.NoCustomSnd)
                snd = note.Snd;
            // �����ʼʱ��ͽ���ʱ��һ�£�Ϊtap
            if (note.FromBeat == note.ToBeat)
            {
                GameObject go = Instantiate(tap, lines[note.Line].transform);
                TapController controller = go.GetComponent<TapController>();
                controller.Line = lines[note.Line];
                controller.Index = i;
                controller.Snd = snd;
                from = map.ToRealTime(note).Item1;
                controller.Time = from + Delay;
                if (map.NoteList.FindAll(x => map.ToRealTime(x).Item1 == from).Count >= 2)
                    controller.GetComponent<SpriteRenderer>().sprite = controller.DoubleSprite;
                lines[note.Line].HitObjects.Add(controller);
                go.SetActive(false);
                noteController = controller;
            }
            else
            {
                GameObject go = Instantiate(hold, lines[note.Line].transform);
                HoldController controller = go.GetComponent<HoldController>();
                controller.Index = i;
                controller.Snd = snd;
                controller.Line = lines[note.Line];
                controller.From = map.ToRealTime(note).Item1 + Delay;
                controller.To = map.ToRealTime(note).Item2 + Delay;
                from = map.ToRealTime(note).Item1;
                if (map.NoteList.FindAll(x => map.ToRealTime(x).Item1 == from).Count >= 2)
                    controller.GetComponent<SpriteRenderer>().sprite = controller.DoubleSprite;
                lines[note.Line].HitObjects.Add(controller);
                go.SetActive(false);
                noteController = controller;
            }
            lines[note.Line].RemainingNote++;
            // ��Ч����
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
            // ����������б�
            if (HitJudge.HitList.Count == 0)
            {
                HitJudge.HitList.Add(new List<MonoBehaviour>() { noteController });
            }
            else
            {
                // ��Ѻ����
                if (HitJudge.HitList[^1][0] is TapController mtap)
                {
                    if (mtap.Time == from + Delay)
                    {
                        HitJudge.HitList[^1].Add(noteController);
                    }
                    else
                    {
                        HitJudge.HitList.Add(new List<MonoBehaviour>() { noteController });
                    }
                }
                else if (HitJudge.HitList[^1][0] is HoldController mhold)
                {
                    if (mhold.From == from + Delay)
                    {
                        HitJudge.HitList[^1].Add(noteController);
                    }
                    else
                    {
                        HitJudge.HitList.Add(new List<MonoBehaviour>() { noteController });
                    }
                }
            }
            i++;
        }

        // ���ݹ�������������б�
        for(int j = 0;j < HitJudge.HitList.Count; j++)
        {
            HitJudge.HitList[j].Sort((x, y) => GetLineIndex(x).CompareTo(GetLineIndex(y)));
        }

        Debug.Log("�������б�" + HitJudge.HitList.Count);

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
