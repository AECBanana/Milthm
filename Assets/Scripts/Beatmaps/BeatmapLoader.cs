using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 谱面加载器
/// </summary>
public class BeatmapLoader : MonoBehaviour
{
    public static float FlowSpeed;
    public static BeatmapLoader Instance;
    /// <summary>
    /// 谱面延迟
    /// </summary>
    public static float Delay = 0f;
    public AudioSource Audio;
    public Image Bg1, Bg2;
    public static BeatmapModel Playing;
    public static string PlayingUID;
    public static int PlayingIndex;
    List<LineController> lines = new List<LineController>();

    private void Start()
    {
        Instance = this;
        // 载入正在游玩的谱面，设置背景
        Load(Playing);
        Bg1.sprite = SongResources.Illustration[PlayingUID][Playing.IllustrationFile];
        Bg2.sprite = Bg1.sprite;
        Audio.clip = SongResources.Songs[PlayingUID];
    }

    /// <summary>
    /// 取得note所在轨道序号
    /// </summary>
    /// <param name="note">note</param>
    /// <returns>轨道序号</returns>
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
    /// 载入谱面
    /// </summary>
    /// <param name="map">谱面数据模型</param>
    public void Load(BeatmapModel map)
    {
        // 销毁已有轨道
        for(int j = 0;j < LineController.Lines.Count; j++)
        {
            Destroy(LineController.Lines[j].gameObject);
        }
        LineController.Lines.Clear();
        // 初始化
        HitJudge.JudgeMode = PlayerPrefs.GetInt("JudgeMode", 0);
        HitJudge.JudgeArea = Camera.main.ViewportToWorldPoint(new Vector3(180f / 1920f, 0, 0)).x - Camera.main.ViewportToWorldPoint(Vector3.zero).x;
        float flowspeed = PlayerPrefs.GetFloat("FlowSpeed", 0.5f),
              scale = PlayerPrefs.GetFloat("Scale", 0.0f);
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
        // 生成轨道
        float space = Camera.main.ViewportToWorldPoint(new Vector3(200f / 1920f, 0, 0)).x - Camera.main.ViewportToWorldPoint(Vector3.zero).x;
        float x = -space * (map.LineList.Count - 1) / 2;
        foreach (BeatmapModel.LineData l in map.LineList)
        {
            GameObject go = Instantiate(line);
            LineController controller = go.GetComponent<LineController>();
            controller.FlowSpeed = l.FlowSpeed * 0.9f;
            controller.Direction = l.Direction;
            if (l.Direction == BeatmapModel.LineDirection.Right)
                go.transform.localEulerAngles = new Vector3(0, 0, 0);
            else if (l.Direction == BeatmapModel.LineDirection.Up)
                go.transform.localEulerAngles = new Vector3(0, 0, 90);
            else if (l.Direction == BeatmapModel.LineDirection.Left)
                go.transform.localEulerAngles = new Vector3(0, 0, 180);
            else if (l.Direction == BeatmapModel.LineDirection.Down)
                go.transform.localEulerAngles = new Vector3(0, 0, 270);

            go.transform.localEulerAngles = new Vector3(0, 0, 90);
            go.transform.localPosition = new Vector3(x, -4f, 0);
            x += space;

            go.transform.localScale = new Vector3(0.3f * (1.0f + scale), 0.3f * (1.0f + scale), 0.3f * (1.0f + scale));
            go.SetActive(true);
            lines.Add(controller);
        }
        // 载入所有note
        int i = 0;
        foreach (BeatmapModel.NoteData note in map.NoteList)
        {
            float from; MonoBehaviour noteController;
            // 如果开始时间和结束时间一致，为tap
            if (note.FromBeat == note.ToBeat)
            {
                GameObject go = Instantiate(tap, lines[note.Line].transform);
                TapController controller = go.GetComponent<TapController>();
                controller.Line = lines[note.Line];
                controller.Index = i;
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
            // 加入待击打列表
            if (HitJudge.HitList.Count == 0)
            {
                HitJudge.HitList.Add(new List<MonoBehaviour>() { noteController });
            }
            else
            {
                // 多押处理
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

        // 根据轨道序号排序击打列表
        for(int j = 0;j < HitJudge.HitList.Count; j++)
        {
            HitJudge.HitList[j].Sort((x, y) => GetLineIndex(x).CompareTo(GetLineIndex(y)));
        }

        Debug.Log("待击打列表：" + HitJudge.HitList.Count);

        // 开始游戏

        Audio.time = 0;
        AudioUpdate.StartTime = System.DateTime.Now;
        AudioUpdate.Started = false;
        Audio.Stop();

        GamePlayLoops.Instance.SummaryAni.Play("PlayEnterAni", 0, 0.0f);
    }
}
