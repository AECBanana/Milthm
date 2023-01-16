using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class BeatmapLoader : MonoBehaviour
{
    public static BeatmapLoader Instance;
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
        Load(Playing);
        Bg1.sprite = SongResources.Illustration[PlayingUID][Playing.IllustrationFile];
        Bg2.sprite = Bg1.sprite;
        Audio.clip = SongResources.Songs[PlayingUID];
    }

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

    public void Load(BeatmapModel map)
    {
        for(int j = 0;j < LineController.Lines.Count; j++)
        {
            Destroy(LineController.Lines[j]);
        }
        LineController.Lines.Clear();
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
        float x = -2f * (map.LineList.Count - 1) / 2;
        foreach (BeatmapModel.LineData l in map.LineList)
        {
            GameObject go = Instantiate(line);
            LineController controller = go.GetComponent<LineController>();
            controller.FlowSpeed = l.FlowSpeed;
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
            x += 2f;

            go.SetActive(true);
            lines.Add(controller);
        }
        int i = 0;
        foreach (BeatmapModel.NoteData note in map.NoteList)
        {
            float from; MonoBehaviour noteController;
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
            if (HitJudge.HitList.Count == 0)
            {
                HitJudge.HitList.Add(new List<MonoBehaviour>() { noteController });
            }
            else
            {
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

        for(int j = 0;j < HitJudge.HitList.Count; j++)
        {
            HitJudge.HitList[j].Sort((x, y) => GetLineIndex(x).CompareTo(GetLineIndex(y)));
        }

        Debug.Log("´ý»÷´òÁÐ±í£º" + HitJudge.HitList.Count);

        Audio.time = 0;
        AudioUpdate.StartTime = System.DateTime.Now;
        AudioUpdate.Started = false;
        Audio.Stop();

        GamePlayLoops.Instance.SummaryAni.Play("PlayEnterAni", 0, 0.0f);
    }
}
