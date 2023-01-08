using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BeatmapLoader : MonoBehaviour
{
    public AudioSource Audio;
    List<LineController> lines = new List<LineController>();
    
    public void Load(BeatmapModel map)
    {
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

            go.transform.localEulerAngles = new Vector3(0, 0, 90);
            go.transform.localPosition = new Vector3(x, -4f, 0);
            x += (6f) / 3;

            go.SetActive(true);
            lines.Add(controller);
        }
        foreach(BeatmapModel.NoteData note in map.NoteList)
        {
            if (note.FromBeat == note.ToBeat)
            {
                GameObject go = Instantiate(tap, lines[note.Line].transform);
                TapController controller = go.GetComponent<TapController>();
                controller.Line = lines[note.Line];
                controller.Key = note.SpecificKey;
                if (note.SpecificKey == KeyCode.None)
                    controller.transform.GetChild(0).gameObject.SetActive(false);
                else
                    controller.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("KeySets\\" + (char)('A' + note.SpecificKey - KeyCode.A));
                controller.Time = map.ToRealTime(note).Item1;
                if (map.NoteList.FindAll(x => map.ToRealTime(x).Item1 == controller.Time).Count >= 2)
                    controller.GetComponent<SpriteRenderer>().sprite = controller.DoubleSprite;
                lines[note.Line].HitObjects.Add(controller);
                lines[note.Line].ObjectRenders.Add((controller.GetComponent<SpriteRenderer>(), controller.transform.GetChild(0).GetComponent<SpriteRenderer>()));
                go.SetActive(false);
            }
            else
            {
                GameObject go = Instantiate(hold, lines[note.Line].transform);
                HoldController controller = go.GetComponent<HoldController>();
                controller.Key = note.SpecificKey;
                if (note.SpecificKey == KeyCode.None)
                    controller.transform.GetChild(0).gameObject.SetActive(false);
                else
                    controller.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("KeySets\\" + (char)('A' + note.SpecificKey - KeyCode.A));
                controller.Line = lines[note.Line];
                controller.From = map.ToRealTime(note).Item1;
                controller.To = map.ToRealTime(note).Item2;
                if (map.NoteList.FindAll(x => map.ToRealTime(x).Item1 == controller.From).Count >= 2)
                    controller.GetComponent<SpriteRenderer>().sprite = controller.DoubleSprite;
                lines[note.Line].HitObjects.Add(controller);
                lines[note.Line].ObjectRenders.Add((controller.GetComponent<SpriteRenderer>(), controller.transform.GetChild(0).GetComponent<SpriteRenderer>()));
                go.SetActive(false);
            }
        }
    }
}
