using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BeatmapLoader : MonoBehaviour
{
    public static BeatmapLoader Instance;
    public static float Delay = 0f;
    public Animator DangerAni;
    public GameObject DeadScreen;
    public AudioSource Audio;
    List<LineController> lines = new List<LineController>();

    private void Awake()
    {
        Instance = this;
    }

    public void Load(BeatmapModel map)
    {
        Delay = PlayerPrefs.GetFloat("Delay") / 1000f;
        DebugInfo.Output("Delay", Delay.ToString() + "s");
        GameObject line = Resources.Load<GameObject>("Line"),
                   tap = Resources.Load<GameObject>("Tap"),
                   hold = Resources.Load<GameObject>("Hold");
        float x = -3f;
        foreach (BeatmapModel.LineData l in map.LineList)
        {
            GameObject go = Instantiate(line);
            LineController controller = go.GetComponent<LineController>();
            controller.FlowSpeed = l.FlowSpeed;
            controller.Direction = l.Direction;
            controller.KeyOverride = l.KeyOverride;
            if (l.Direction == BeatmapModel.LineDirection.Right)
                go.transform.localEulerAngles = new Vector3(0, 0, 0);
            else if (l.Direction == BeatmapModel.LineDirection.Up)
                go.transform.localEulerAngles = new Vector3(0, 0, 90);
            else if (l.Direction == BeatmapModel.LineDirection.Left)
                go.transform.localEulerAngles = new Vector3(0, 0, 180);
            else if (l.Direction == BeatmapModel.LineDirection.Down)
                go.transform.localEulerAngles = new Vector3(0, 0, 270);

            if (l.KeyOverride != KeyCode.None)
            {
                controller.KeyTip.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("KeySets\\" + (char)('A' + l.KeyOverride - KeyCode.A));
            }
            else
            {
                controller.KeyTip.gameObject.SetActive(false);
            }

            go.transform.localEulerAngles = new Vector3(0, 0, 90);
            go.transform.localPosition = new Vector3(x, -4f, 0);
            x += (6f) / 3;

            go.SetActive(true);
            lines.Add(controller);
        }
        int i = 0;
        HitJudge.Result = new HitJudge.ResultData
        {
            FullCombo = map.NoteList.Count
        };
        HitJudge.HitList = new List<List<MonoBehaviour>>();
        HitJudge.CaptureOnce = new List<KeyCode>();
        foreach (BeatmapModel.NoteData note in map.NoteList)
        {
            float from; MonoBehaviour noteController;
            if (note.FromBeat == note.ToBeat)
            {
                GameObject go = Instantiate(tap, lines[note.Line].transform);
                TapController controller = go.GetComponent<TapController>();
                controller.Line = lines[note.Line];
                controller.Key = note.SpecificKey;
                controller.Index = i;
                if (note.SpecificKey == KeyCode.None)
                    controller.transform.GetChild(0).gameObject.SetActive(false);
                else
                    controller.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("KeySets\\" + (char)('A' + note.SpecificKey - KeyCode.A));
                from = map.ToRealTime(note).Item1;
                controller.Time = from + Delay;
                if (map.NoteList.FindAll(x => map.ToRealTime(x).Item1 == from).Count >= 2)
                    controller.GetComponent<SpriteRenderer>().sprite = controller.DoubleSprite;
                lines[note.Line].HitObjects.Add(controller);
                lines[note.Line].ObjectRenders.Add((controller.GetComponent<SpriteRenderer>(), controller.transform.GetChild(0).GetComponent<SpriteRenderer>()));
                go.SetActive(false);
                noteController = controller;
            }
            else
            {
                GameObject go = Instantiate(hold, lines[note.Line].transform);
                HoldController controller = go.GetComponent<HoldController>();
                controller.Key = note.SpecificKey;
                controller.Index = i;
                if (note.SpecificKey == KeyCode.None)
                    controller.transform.GetChild(0).gameObject.SetActive(false);
                else
                    controller.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("KeySets\\" + (char)('A' + note.SpecificKey - KeyCode.A));
                controller.Line = lines[note.Line];
                controller.From = map.ToRealTime(note).Item1 + Delay;
                controller.To = map.ToRealTime(note).Item2 + Delay;
                from = map.ToRealTime(note).Item1;
                if (map.NoteList.FindAll(x => map.ToRealTime(x).Item1 == from).Count >= 2)
                    controller.GetComponent<SpriteRenderer>().sprite = controller.DoubleSprite;
                lines[note.Line].HitObjects.Add(controller);
                lines[note.Line].ObjectRenders.Add((controller.GetComponent<SpriteRenderer>(), controller.transform.GetChild(0).GetComponent<SpriteRenderer>()));
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

        Debug.Log("´ý»÷´òÁÐ±í£º" + HitJudge.HitList.Count);
    }
}
